namespace RJCP.Diagnostics.Log
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface to construct <see cref="ITraceWriter{T}"/> objects to encode specific trace file formats.
    /// </summary>
    /// <typeparam name="T">The type of Trace Line that should be encoded.</typeparam>
    public interface ITraceWriterFactory<T> where T : ITraceLine
    {
        /// <summary>
        /// Creates an <see cref="ITraceWriter{T}"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceWriter{T}"/> object the factory knows how to create.</returns>
        Task<ITraceWriter<T>> CreateAsync(Stream stream);

        /// <summary>
        /// Creates an <see cref="ITraceWriter{T}"/> from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceWriter{T}"/> object the factory knows how to create.</returns>
        Task<ITraceWriter<T>> CreateAsync(string fileName);
    }
}
