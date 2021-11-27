namespace RJCP.Diagnostics.Log
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface to construct <see cref="ITraceReader{T}"/> objects to decode specific trace file formats.
    /// </summary>
    /// <typeparam name="T">The type of Trace Line that should be returned.</typeparam>
    public interface ITraceReaderFactory<T> where T : class, ITraceLine
    {
        /// <summary>
        /// Creates an <see cref="ITraceReader{T}"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object the factory knows how to create.</returns>
        Task<ITraceReader<T>> CreateAsync(Stream stream);

        /// <summary>
        /// Creates an <see cref="ITraceReader{T}"/> from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object the factory knows how to create.</returns>
        Task<ITraceReader<T>> CreateAsync(string fileName);
    }
}
