namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure.IO;

    /// <summary>
    /// Simulates reading from a "null" packet device.
    /// </summary>
    public sealed class EmptyPacketReceiver : IPacket
    {
        private int m_ChannelCount = 0;

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
            if (handler != null) handler(sender, args);
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            /* Nothing to open */
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
            if (m_ChannelCount == 0) {
                m_ChannelCount = 1;
                OnNewChannel(this, new PacketNewChannelEventArgs(0));
            }

            return
                new ValueTask<PacketReadResult>(new PacketReadResult(0, 0));
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            /* Nothing to close */
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
