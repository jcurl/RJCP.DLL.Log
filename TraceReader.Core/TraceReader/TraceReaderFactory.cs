namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Decoder;

    /// <summary>
    /// An abstract factory for common operations to instantiate a <see cref="ITraceReader{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    public abstract class TraceReaderFactory<T> : ITraceReaderFactory<T> where T : class, ITraceLine
    {
        private const int FileSystemCaching = 262144;

        /// <summary>
        /// Creates an <see cref="ITraceReader{T}" /> from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object for log file enumeration.</returns>
        public Task<ITraceReader<T>> CreateAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            ITraceDecoder<T> decoder = GetDecoder();
            return Task.FromResult<ITraceReader<T>>(new TraceReader<T>(stream, decoder));
        }

        /// <summary>
        /// Creates an <see cref="ITraceReader{T}"/> from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object for log file enumeration.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="fileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">The <paramref name="fileName"/> is invalid.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="fileName"/> is a non-file device.</exception>
        /// <exception cref="FileNotFoundException">The <paramref name="fileName"/> is not found.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller doesn't have the required permissions.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file is in use, or insufficient permissions.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// </exception>
        public Task<ITraceReader<T>> CreateAsync(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read,
                FileSystemCaching, FileOptions.Asynchronous | FileOptions.SequentialScan);

            return CreateAsync(fileStream);
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="ITraceDecoder{T}"/> that knows how to decode the stream.</returns>
        protected abstract ITraceDecoder<T> GetDecoder();
    }
}
