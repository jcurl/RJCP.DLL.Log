namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;

    /// <summary>
    /// Arguments for when a new channel is added while reading.
    /// </summary>
    public class PacketNewChannelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketNewChannelEventArgs"/> class.
        /// </summary>
        /// <param name="channelNumber">The new channel number.</param>
        public PacketNewChannelEventArgs(int channelNumber)
        {
            ChannelNumber = channelNumber;
        }

        /// <summary>
        /// Gets the new channel number.
        /// </summary>
        /// <value>The new channel number. This value is zero index based.</value>
        public int ChannelNumber { get; }
    }
}
