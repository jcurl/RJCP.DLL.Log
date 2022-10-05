namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Instantiate a <see cref="ITraceReader{DltTraceLineBase}"/> specifically for reading from an input stream of
    /// <see cref="IPacket"/>.
    /// </summary>
    public class TracePacketReader : ITraceReader<DltTraceLineBase>
    {
        private sealed class DecoderState
        {
            public DecoderState(ITraceDecoderFactory<DltTraceLineBase> factory)
            {
                Decoder = factory.Create();
            }

            public ITraceDecoder<DltTraceLineBase> Decoder { get; private set; }

            public long Position { get; set; }
        }

        private readonly IPacket m_Packet;
        private readonly ITraceDecoderFactory<DltTraceLineBase> m_DecoderFactory;

        private readonly object m_DecodersLock = new object();
        private readonly List<DecoderState> m_Decoders = new List<DecoderState>();

        private readonly byte[] m_Buffer = new byte[65536];
        private readonly Memory<byte> m_BufferMem;

        private bool m_PacketEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracePacketReader{DltTraceLineBase}"/> class.
        /// </summary>
        /// <param name="packet">The readable packet stream to decode.</param>
        /// <param name="decoderFactory">The decoder factory, which instantiates a decoder on every new channel.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="packet"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="decoderFactory"/> is <see langword="null"/>.
        /// </exception>
        public TracePacketReader(IPacket packet, ITraceDecoderFactory<DltTraceLineBase> decoderFactory)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));
            if (decoderFactory == null) throw new ArgumentNullException(nameof(decoderFactory));

            m_Packet = packet;
            m_DecoderFactory = decoderFactory;
            m_BufferMem = m_Buffer.AsMemory();

            m_Packet.NewChannel += Packet_NewChannel;
            Packet_NewChannel(this, new PacketNewChannelEventArgs(packet.ChannelCount));
        }

        private void Packet_NewChannel(object sender, PacketNewChannelEventArgs e)
        {
            // When we initialise the IPacket, this is called with the number of channels, which initializes them all
            // with decoders we need. When reading, if a new channel is detected, it is added before the
            // `IPacket.ReadAsync` is returned.
            lock (m_DecodersLock) {
                if (m_Decoders.Count < e.ChannelNumber + 1) {
                    for (int i = m_Decoders.Count; i <= e.ChannelNumber; i++) {
                        m_Decoders.Add(new DecoderState(m_DecoderFactory));
                    }
                }
            }
        }

        private IEnumerator<DltTraceLineBase> m_LineEnumerator;

        /// <summary>
        /// Gets the next line from the stream asynchronously.
        /// </summary>
        /// <returns>
        /// The next line from the stream. If the result is <see langword="null"/>, then the stream end has been
        /// reached.
        /// </returns>
        public async Task<DltTraceLineBase> GetLineAsync()
        {
            do {
                if (m_LineEnumerator != null) {
                    if (m_LineEnumerator.MoveNext()) {
                        return m_LineEnumerator.Current;
                    }
                    m_LineEnumerator = null;
                }

                // We have no more data to parse.
                if (m_PacketEnd) return null;

                IEnumerable<DltTraceLineBase> lines;
                PacketReadResult read = await m_Packet.ReadAsync(m_BufferMem);

                if (read.ReceivedBytes <= 0) {
                    lines = new DecoderFlush(m_Decoders);
                    m_PacketEnd = true;
                } else {
                    lines = m_Decoders[read.Channel].Decoder
                        .Decode(
                            m_Buffer.AsSpan(0, read.ReceivedBytes),
                            m_Decoders[read.Channel].Position);
                    m_Decoders[read.Channel].Position += read.ReceivedBytes;
                }

                if (lines == null) return null;
                m_LineEnumerator = lines.GetEnumerator();
            } while (true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and/or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Set to <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Must make sure to remove the event handler, as this may occur asynchronously if two threads are running in 
            // parallel.

            // The results are undefined, if other clients are using the enumeration returned by GetLineAsync(), Usually
            // it will work because they have an object in memory and the GC clears it up. But that depends on the
            // implementation of the decoder.

            lock (m_DecodersLock) {
                m_Packet.NewChannel -= Packet_NewChannel;

                foreach (DecoderState decoder in m_Decoders) {
                    decoder.Decoder.Dispose();
                }
            }
        }

        private sealed class DecoderFlush : IEnumerable<DltTraceLineBase>
        {
            private readonly IEnumerable<DecoderState> m_Decoders;

            public DecoderFlush(IEnumerable<DecoderState> decoderState)
            {
                m_Decoders = decoderState;
            }

            public IEnumerator<DltTraceLineBase> GetEnumerator()
            {
                return new DecoderFlushEnumerator(m_Decoders);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class DecoderFlushEnumerator : IEnumerator<DltTraceLineBase>
        {
            private readonly IEnumerator<DecoderState> m_Decoders;
            private IEnumerator<DltTraceLineBase> m_Lines;
            private bool m_Complete;

            public DecoderFlushEnumerator(IEnumerable<DecoderState> decoderState)
            {
                m_Decoders = decoderState.GetEnumerator();
            }

            public DltTraceLineBase Current
            {
                get
                {
                    return m_Lines?.Current;
                }
            }

            object IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                if (m_Complete) return false;

                if (m_Lines == null) {
                    if (!MoveNextDecoder()) return false;
                }

                while (true) {
                    bool next = m_Lines.MoveNext();
                    if (next) return true;
                    if (!MoveNextDecoder()) return false;
                }
            }

            private bool MoveNextDecoder()
            {
                if (m_Decoders.MoveNext() && m_Decoders.Current != null) {
                    m_Lines = m_Decoders.Current.Decoder.Flush().GetEnumerator();
                    return true;
                }

                m_Complete = true;
                return false;
            }

            public void Reset() { /* Nothing to reset */ }

            public void Dispose() { /* Nothing to dispose */ }
        }
    }
}
