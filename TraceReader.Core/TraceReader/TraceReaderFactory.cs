namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Decoder;

    /// <summary>
    /// A factory for common operations to instantiate a <see cref="ITraceReader{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    public class TraceReaderFactory<T> : ITraceReaderFactory<T> where T : class, ITraceLine
    {
        private const int FileSystemCaching = 262144;

        private readonly ITraceDecoderFactory<T> m_DecoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceReaderFactory{T}"/> class.
        /// </summary>
        /// <param name="decoderFactory">The decoder factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoderFactory"/> is <see langword="null"/>.</exception>
        protected TraceReaderFactory(ITraceDecoderFactory<T> decoderFactory)
        {
            if (decoderFactory == null)
                throw new ArgumentNullException(nameof(decoderFactory));
            m_DecoderFactory = decoderFactory;
        }

        /// <summary>
        /// Creates an <see cref="ITraceReader{T}" /> from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{T}"/> object for log file enumeration.</returns>
        public Task<ITraceReader<T>> CreateAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            ITraceDecoder<T> decoder = m_DecoderFactory.Create();
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
    }
}
