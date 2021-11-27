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
        /// Creates an <see cref="ITraceReader{T}" /> from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object for log file enumeration.</returns>
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
