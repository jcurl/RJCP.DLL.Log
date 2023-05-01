namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

    public class TestDltTraceReaderFactory : IDltDumpTraceReaderFactory
    {
        /// <summary>
        /// Gets the collection of lines that can be modified which the factory uses to create a reader.
        /// </summary>
        /// <value>The collection of lines to return.</value>
        public ICollection<DltTraceLineBase> Lines { get; } = new List<DltTraceLineBase>();

        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        /// <remarks>This field is not used for the test mocks, as no files or binary data is interpreted.</remarks>
        public InputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the frame map used for decoding non-verbose messages.
        /// </summary>
        /// <value>The frame map used for decoding non-verbose messages.</value>
        public IFrameMap FrameMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
        /// <remarks>This field is not used for the test mocks, as no files or binary data is interpreted.</remarks>
        public bool OnlineMode { get; set; }

        /// <summary>
        /// Gets or sets the output stream to use when instantiating.
        /// </summary>
        /// <value>The output stream.</value>
        /// <remarks>
        /// When instantiating via <see cref="CreateAsync(string)"/>, the <see cref="IOutputStream.SupportsBinary"/> is
        /// used to determine if this object should be injected or not. If this object is <see langword="null"/>, then
        /// no <see cref="IOutputStream"/> is used.
        /// </remarks>
        public IOutputStream OutputStream { get; set; }

        /// <summary>
        /// Gets or sets a flag that will cause an exception at the end of parsing with a
        /// <see cref="LineTraceReader{T}"/>.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> then the next instantiation with <see cref="CreateAsync(Stream)"/> or
        /// <see cref="CreateAsync(string)"/> will create a reader that will raise an exception when all lines are
        /// obtained.
        /// </value>
        public bool TriggerExceptionOnEof { get; set; }

        /// <summary>
        /// Creates a mocked <see cref="ITraceReader{DltTraceLineBase}"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return Task.FromResult(GetTraceReader());
        }

        /// <summary>
        /// Creates a mocked <see cref="ITraceReader{DltTraceLineBase}"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            return Task.FromResult(GetTraceReader());
        }

        /// <summary>
        /// Creates a mocked <see cref="ITraceReader{DltTraceLineBase}"/>.
        /// </summary>
        /// <param name="packet">The packet reader interface.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packet"/> is <see langword="null"/>.</exception>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(IPacket packet)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));
            return Task.FromResult(GetTraceReader());
        }

        private ITraceReader<DltTraceLineBase> GetTraceReader()
        {
            if (OutputStream == null || !OutputStream.SupportsBinary) {
                LineTraceReader<DltTraceLineBase> reader;
                reader = new LineTraceReader<DltTraceLineBase>(Lines);
                if (TriggerExceptionOnEof) {
                    TriggerExceptionOnEof = false;
                    reader.GetLineEvent += (s, e) => {
                        if (e.Line == null)
                            throw new InvalidOperationException("Test Operation after file reading");
                    };
                }
                return reader;
            }

            TriggerExceptionOnEof = false;
            return new BinaryTraceReader<DltTraceLineBase>(OutputStream);
        }
    }
}
