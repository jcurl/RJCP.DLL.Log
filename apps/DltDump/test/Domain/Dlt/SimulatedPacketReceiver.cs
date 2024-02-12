namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Infrastructure.IO;

    /// <summary>
    /// Provides a list of predetermined packets for a reader to read.
    /// </summary>
    public sealed class SimulatedPacketReceiver : IPacket
    {
        private readonly IEnumerator<(int channel, byte[] data)> m_PacketEnumerator;
        private bool m_IsOpened;
        private bool m_IsDisposed;
        private int m_ChannelCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedPacketReceiver"/> class.
        /// </summary>
        /// <param name="packets">The packets that should be returned with <see cref="ReadAsync(Memory{byte})"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="packets"/> is <see langword="null"/>.</exception>
        public SimulatedPacketReceiver(IEnumerable<(int, byte[])> packets)
        {
            ArgumentNullException.ThrowIfNull(packets);
            m_PacketEnumerator = packets.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of channels that are currently decoded.
        /// </summary>
        /// <value>The channel count.</value>
        public int ChannelCount
        {
            get { return m_ChannelCount; }
        }

        /// <summary>
        /// An event that is called when a new channel is obtained.
        /// </summary>
        public event EventHandler<PacketNewChannelEventArgs> NewChannel;

        private void OnNewChannel(object sender, PacketNewChannelEventArgs args)
        {
            EventHandler<PacketNewChannelEventArgs> handler = NewChannel;
            if (handler is object) handler(sender, args);
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(SimulatedPacketReceiver));
            if (m_IsOpened) throw new InvalidOperationException("Packet Reader is already open");
            m_IsOpened = true;
        }

        /// <summary>
        /// Reads data from the source asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer where the data is written to.</param>
        /// <returns>
        /// A task that has the <see cref="PacketReadResult"/> result with the number of bytes read, and the channel
        /// identifier where the data originated. Data always originates from the first value of channel 0, and
        /// increments for the lifetime of the source.
        /// </returns>
        /// <remarks>This method doesn't take a cancellation token, as in .NET 3.1, we can't cancel.</remarks>
        public ValueTask<PacketReadResult> ReadAsync(Memory<byte> buffer)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(SimulatedPacketReceiver));
            if (!m_IsOpened) throw new InvalidOperationException("Packet Reader is not open");

            if (!m_PacketEnumerator.MoveNext())
                return new ValueTask<PacketReadResult>(new PacketReadResult(0, 0));

            var (channel, data) = m_PacketEnumerator.Current;
            if (m_ChannelCount <= channel) {
                m_ChannelCount = channel + 1;
                OnNewChannel(this, new PacketNewChannelEventArgs(channel));
            }

            if (buffer.Length < data.Length) {
                data.AsSpan(0, buffer.Length).CopyTo(buffer.Span);
                return new ValueTask<PacketReadResult>(new PacketReadResult(buffer.Length, channel));
            }

            data.CopyTo(buffer);
            return new ValueTask<PacketReadResult>(new PacketReadResult(data.Length, channel));
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(SimulatedPacketReceiver));
            if (!m_IsOpened) throw new InvalidOperationException("Packet Reader is not open");
            m_IsOpened = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            m_IsDisposed = true;
        }
    }
}
