namespace RJCP.App.DltDump.Infrastructure.IO
{
    /// <summary>
    /// The result of an asynchronous read operation.
    /// </summary>
    public readonly struct PacketReadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReadResult"/> struct.
        /// </summary>
        /// <param name="received">The number of bytes received.</param>
        /// <param name="channel">The channel the data was received on.</param>
        public PacketReadResult(int received, int channel)
        {
            ReceivedBytes = received;
            Channel = channel;
        }

        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        /// <value>The number of bytes received.</value>
        public int ReceivedBytes { get; }

        /// <summary>
        /// Gets an identifier representing this channel.
        /// </summary>
        /// <value>The channel.</value>
        /// <remarks>The channel number received is unique for this particular <see cref="IPacket"/> instance.</remarks>
        public int Channel { get; }
    }
}
