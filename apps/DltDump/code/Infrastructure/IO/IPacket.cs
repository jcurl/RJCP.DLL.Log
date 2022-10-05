namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for reading packet based streams, providing the source of the channel the packet arrived at.
    /// </summary>
    /// <remarks>
    /// The packet reader supports reading from an endpoint that may have multiple channels. An example of this are UDP
    /// packets, where multiple participants may be operating on a multicast address, each participant is called a
    /// channel in this context.
    /// <para>
    /// It might be necessary to perform some operations when a channel is found. The safest way to do this is by first
    /// initializing your data structures to be empty. Register to <see cref="NewChannel"/> which is called when a new
    /// channel is added from within the <see cref="ReadAsync(Memory{byte})"/> method. For channels that already exist,
    /// iterate through the number of channels from 0 to <see cref="ChannelCount"/> (taking care if reading at the same
    /// time as initialisation).
    /// </para>
    /// </remarks>
    public interface IPacket : IDisposable
    {
        /// <summary>
        /// Opens this instance.
        /// </summary>
        void Open();

        /// <summary>
        /// Gets the number of channels that are currently decoded.
        /// </summary>
        /// <value>The channel count.</value>
        int ChannelCount { get; }

        /// <summary>
        /// An event that is called when a new channel is obtained.
        /// </summary>
        event EventHandler<PacketNewChannelEventArgs> NewChannel;

        /// <summary>
        /// Reads data from the source asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer where the data is written to.</param>
        /// <returns>
        /// A task that has the <see cref="PacketReadResult"/> result with the number of bytes read, and the channel identifier
        /// where the data originated. Data always originates from the first value of channel 0, and increments for the
        /// lifetime of the source.
        /// </returns>
        /// <remarks>
        /// This method doesn't take a cancellation token, as in .NET 3.1, we can't cancel.
        /// </remarks>
        ValueTask<PacketReadResult> ReadAsync(Memory<byte> buffer);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}
