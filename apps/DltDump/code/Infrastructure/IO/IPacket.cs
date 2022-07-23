namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for reading packet based streams, providing the source of the channel the packet arrived at.
    /// </summary>
    public interface IPacket : IDisposable
    {
        /// <summary>
        /// Opens this instance.
        /// </summary>
        void Open();

        /// <summary>
        /// Reads data from the source asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer where the data is written to.</param>
        /// <returns>
        /// A task that has the <see cref="Tuple"/> result with the number of bytes read, and the channel identifier
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
