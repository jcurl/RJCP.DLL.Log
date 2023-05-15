namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Encoder;

    /// <summary>
    /// A DLT Writer factory for writing DLT streams.
    /// </summary>
    public class DltTraceWriterFactory : ITraceWriterFactory<DltTraceLineBase>
    {
        private const int FileSystemCaching = 65536;
        private readonly ITraceEncoderFactory<DltTraceLineBase> m_EncoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceWriterFactory"/> class.
        /// </summary>
        public DltTraceWriterFactory()
            : this(new DltTraceEncoderFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceWriterFactory"/> class.
        /// </summary>
        /// <param name="encoderFactory">The encoder factory for writing different types of streams.</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoderFactory"/> is <see langword="null"/>.</exception>
        protected DltTraceWriterFactory(ITraceEncoderFactory<DltTraceLineBase> encoderFactory)
        {
            if (encoderFactory is null)
                throw new ArgumentNullException(nameof(encoderFactory));
            m_EncoderFactory = encoderFactory;
        }

        /// <summary>
        /// Creates an <see cref="ITraceWriter{DltTraceLineBase}"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceWriter{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <remarks>The <paramref name="stream"/> is not disposed of when the returned encoder is disposed.</remarks>
        public Task<ITraceWriter<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            ITraceEncoder<DltTraceLineBase> encoder = m_EncoderFactory.Create();
            return Task.FromResult<ITraceWriter<DltTraceLineBase>>(new DltTraceWriter(stream, encoder));
        }

        /// <summary>
        /// Creates an <see cref="ITraceWriter{DltTraceLineBase}"/> from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceWriter{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <remarks>When the returned encoder is disposed of, so will the underlying stream be disposed.</remarks>
        public Task<ITraceWriter<DltTraceLineBase>> CreateAsync(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            Stream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read,
                FileSystemCaching, FileOptions.Asynchronous);

            ITraceEncoder<DltTraceLineBase> encoder = m_EncoderFactory.Create();
            return Task.FromResult<ITraceWriter<DltTraceLineBase>>(new DltTraceWriter(fileStream, encoder, true));
        }
    }
}
