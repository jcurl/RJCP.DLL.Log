namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A DLT decoder that extracts DLT from UDP packets in a PCAP (legacy) file.
    /// </summary>
    public class DltPcapLegacyDecoder : ITraceDecoder<DltTraceLineBase>
    {
        /// <summary>
        /// The number of bytes the PCAP header for a packet consumes (Time Stamp, Captured Lengths).
        /// </summary>
        private const int PcapRecHdrLen = 16;

        private readonly IOutputStream m_OutputStream;
        private readonly byte[] m_Packet = new byte[65536];
        private int m_Length;
        private long m_Position;
        private uint m_ExpectedLength;
        private long m_DiscardLength;

        private PacketDecoder m_Decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapLegacyDecoder"/> class without an output stream or
        /// filter.
        /// </summary>
        public DltPcapLegacyDecoder() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapLegacyDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputStream"/> is <see langword="null"/>.
        /// </exception>
        public DltPcapLegacyDecoder(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_OutputStream = outputStream;
        }

        /// <summary>
        /// Gets the format described by the PCAP file.
        /// </summary>
        /// <value>The format describing the PCAP file.</value>
        public PcapFormat Format { get; private set; }

        private readonly List<DltTraceLineBase> m_List = new List<DltTraceLineBase>();

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// This method shall accept any number of bytes for decoding. It should also consume all data that is received,
        /// so that data which is not processed is buffered locally by the decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltPcapLegacyDecoder));

            if (Format == null) {
                if (m_Length + buffer.Length < PcapFormat.HeaderLength) {
                    // We don't know what the PCAP type is, so store it and wait for the next write.
                    buffer.CopyTo(m_Packet.AsSpan(m_Length));
                    m_Length += buffer.Length;
                    return Array.Empty<DltTraceLineBase>();
                }

                ReadOnlySpan<byte> header;
                if (m_Length != 0) {
                    int hdr = PcapFormat.HeaderLength - m_Length;
                    buffer[0..hdr].CopyTo(m_Packet.AsSpan(m_Length));
                    header = m_Packet.AsSpan(0, PcapFormat.HeaderLength);
                    buffer = buffer[hdr..];
                    Format = new PcapFormat(header);
                    position += hdr;
                    m_Length = 0;
                } else {
                    Format = new PcapFormat(buffer[0..PcapFormat.HeaderLength]);
                    buffer = buffer[PcapFormat.HeaderLength..];
                    position += PcapFormat.HeaderLength;
                }

                if (m_OutputStream == null) {
                    m_Decoder = new PacketDecoder(Format.LinkType);
                } else {
                    m_Decoder = new PacketDecoder(Format.LinkType, m_OutputStream);
                }
            }

            m_List.Clear();
            while (buffer.Length > 0) {
                if (m_DiscardLength > 0) {
                    if (m_DiscardLength >= buffer.Length) {
                        m_DiscardLength -= buffer.Length;
                        return m_List;
                    }
                    int offset = (int)m_DiscardLength;
                    buffer = buffer[offset..];
                    m_DiscardLength = 0;
                    position += offset;
                }

                ReadOnlySpan<byte> packet;
                if (m_Length == 0) {
                    if (buffer.Length < PcapRecHdrLen) {
                        // We copy the partial packet. We do not have enough of the header to calculate the packet size.
                        buffer.CopyTo(m_Packet);
                        m_Length = buffer.Length;
                        m_Position = position;
                        return m_List;
                    }

                    m_ExpectedLength = unchecked((uint)BitOperations.To32Shift(buffer[8..], Format.IsLittleEndian)) + PcapRecHdrLen;
                    if (m_ExpectedLength > m_Packet.Length) {
                        Log.Pcap.TraceEvent(TraceEventType.Verbose,
                            "Skipping excessive packet, position 0x{0:x}, length 0x{1:x} (max 0x{2:x})",
                            position, m_ExpectedLength, m_Packet.Length);

                        // We discard this packet, as it is too big.
                        m_DiscardLength = m_ExpectedLength;
                        m_ExpectedLength = 0;
                        continue;
                    }

                    if (m_ExpectedLength > buffer.Length) {
                        // We copy the partial packet, and collect the rest later.
                        buffer.CopyTo(m_Packet);
                        m_Length = buffer.Length;
                        m_Position = position;
                        return m_List;
                    }

                    // We decode the packet in the buffer
                    packet = buffer[0..(int)m_ExpectedLength];
                    m_Position = position;

                    buffer = buffer[(int)m_ExpectedLength..];
                    position += m_ExpectedLength;
                } else if (m_Length < PcapRecHdrLen) {
                    if (buffer.Length + m_Length < PcapRecHdrLen) {
                        buffer.CopyTo(m_Packet.AsSpan(m_Length));
                        m_Length += buffer.Length;
                        return m_List;
                    }

                    int hdr = PcapRecHdrLen - m_Length;
                    buffer[0..hdr].CopyTo(m_Packet.AsSpan(m_Length));
                    buffer = buffer[hdr..];
                    position += hdr;
                    m_ExpectedLength =
                        unchecked((uint)BitOperations.To32Shift(m_Packet.AsSpan(8), Format.IsLittleEndian)) + PcapRecHdrLen;
                    if (m_ExpectedLength > m_Packet.Length) {
                        Log.Pcap.TraceEvent(TraceEventType.Verbose,
                            "Skipping excessive packet, position 0x{0:x}, length 0x{1:x} (max 0x{2:x})",
                            position - hdr, m_ExpectedLength, m_Packet.Length);

                        // We discard this packet, as it is too big. We've already consumed PcapHdrLen bytes.
                        m_DiscardLength = m_ExpectedLength - PcapRecHdrLen;
                        m_ExpectedLength = 0;
                        m_Length = 0;
                        continue;
                    }

                    m_Length = PcapRecHdrLen;
                    continue;
                } else {
                    if (buffer.Length + m_Length < m_ExpectedLength) {
                        buffer.CopyTo(m_Packet.AsSpan(m_Length));
                        m_Length += buffer.Length;
                        return m_List;
                    }

                    int pkt = (int)m_ExpectedLength - m_Length;
                    buffer[0..pkt].CopyTo(m_Packet.AsSpan(m_Length));
                    buffer = buffer[pkt..];
                    position += pkt;
                    packet = m_Packet.AsSpan(0, m_Length + pkt);
                    m_Length = 0;
                }

                int origLen = BitOperations.To32Shift(packet[12..], Format.IsLittleEndian);
                if (origLen == packet.Length - PcapRecHdrLen) {
                    uint seconds = unchecked((uint)BitOperations.To32Shift(packet, Format.IsLittleEndian));
                    uint subsec = unchecked((uint)BitOperations.To32Shift(packet[4..], Format.IsLittleEndian));
                    DateTime timeStamp = Format.GetTimeStamp(seconds, subsec);
                    m_List.AddRange(m_Decoder.DecodePacket(packet[PcapRecHdrLen..], timeStamp, m_Position + PcapRecHdrLen));
                }
            }

            return m_List;
        }

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Flush()
        {
            return Array.Empty<DltTraceLineBase>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Is <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                if (!m_IsDisposed && m_Decoder != null) {
                    m_Decoder.Dispose();
                }
                m_IsDisposed = true;
            }
        }
    }
}
