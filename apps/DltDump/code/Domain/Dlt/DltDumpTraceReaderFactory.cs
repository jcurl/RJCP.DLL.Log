namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// DLT Trace Reader Factory which constructs a trace reader object.
    /// </summary>
    public class DltDumpTraceReaderFactory : IDltDumpTraceReaderFactory
    {
        private readonly DltDumpTraceDecoderFactory m_DecoderFactory = new DltDumpTraceDecoderFactory();

        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        public InputFormat InputFormat
        {
            get { return m_DecoderFactory.InputFormat; }
            set { m_DecoderFactory.InputFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
        public bool OnlineMode
        {
            get { return m_DecoderFactory.OnlineMode; }
            set { m_DecoderFactory.OnlineMode = value; }
        }

        /// <summary>
        /// Gets or sets the output stream to use when instantiating.
        /// </summary>
        /// <value>The output stream.</value>
        /// <remarks>
        /// When instantiating via <see cref="CreateAsync(string)"/>, the <see cref="IOutputStream.SupportsBinary"/> is
        /// used to determine if this object should be injected or not. If this object is <see langword="null"/>, then
        /// no <see cref="IOutputStream"/> is used.
        /// </remarks>
        public IOutputStream OutputStream
        {
            get { return m_DecoderFactory.OutputStream; }
            set { m_DecoderFactory.OutputStream = value; }
        }

        private sealed class DltTraceReaderFactoryExt : TraceReaderFactory<DltTraceLineBase>
        {
            public DltTraceReaderFactoryExt(DltDumpTraceDecoderFactory decoderFactory) : base(decoderFactory) { }
        }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            return new DltTraceReaderFactoryExt(m_DecoderFactory).CreateAsync(stream);
        }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(string fileName)
        {
            return new DltTraceReaderFactoryExt(m_DecoderFactory).CreateAsync(fileName);
        }

        // TODO: Implement a new `TracePacketReaderFactory<DltTraceLineBase>` similar to `TraceReaderFactory` using the
        // `DltDumpTraceDecoderFactory m_DecoderFactory`. The `TracePacketReaderFactory` instantiates a
        // `TracePacketReader` which knows how to handle an `IPacket`. An `IPacket` would be a bit slower as it must
        // choose (or instantiate) the decoder based on the channel for the data received.

        /// <summary>
        /// Creates an <see cref="ITraceReader{DltTraceLineBase}"/> from a packet interface.
        /// </summary>
        /// <param name="packet">The packet reader interface.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(IPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
