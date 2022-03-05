namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Resources;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// DLT Trace Reader Factory which constructs a trace reader object.
    /// </summary>
    public class DltDumpTraceReaderFactory : IDltTraceReaderFactory
    {
        private readonly DltFileTraceReaderFactory m_DltFileFactory = new DltFileTraceReaderFactory();
        private readonly DltSerialTraceReaderFactory m_DltSerialFactory = new DltSerialTraceReaderFactory();
        private readonly DltTraceReaderFactory m_DltNetworkFactory = new DltTraceReaderFactory();

        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        public InputFormat InputFormat { get; set; }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            switch (InputFormat) {
            case InputFormat.File:
                return m_DltFileFactory.CreateAsync(stream);
            case InputFormat.Serial:
                return m_DltSerialFactory.CreateAsync(stream);
            case InputFormat.Network:
                return m_DltNetworkFactory.CreateAsync(stream);
            default:
                throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
            }
        }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(string fileName)
        {
            switch (InputFormat) {
            case InputFormat.File:
                return m_DltFileFactory.CreateAsync(fileName);
            case InputFormat.Serial:
                return m_DltSerialFactory.CreateAsync(fileName);
            case InputFormat.Network:
                return m_DltNetworkFactory.CreateAsync(fileName);
            default:
                throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
            }
        }
    }
}
