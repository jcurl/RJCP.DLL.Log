namespace RJCP.App.DltDump.Domain.Dlt
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
        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        public InputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
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

        private TraceReaderFactory<DltTraceLineBase> GetFactory()
        {
            if (OutputStream == null || !OutputStream.SupportsBinary) {
                switch (InputFormat) {
                case InputFormat.File:
                    return new DltFileTraceReaderFactory();
                case InputFormat.Serial:
                    return new DltSerialTraceReaderFactory(OnlineMode);
                case InputFormat.Network:
                    return new DltTraceReaderFactory(OnlineMode);
                case InputFormat.Pcap:
                    return new DltPcapTraceReaderFactory();
                default:
                    throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
                }
            }

            switch (InputFormat) {
            case InputFormat.File:
                return new DltFileTraceFilterReaderFactory(OutputStream);
            case InputFormat.Serial:
                return new DltSerialTraceFilterReaderFactory(OutputStream, OnlineMode);
            case InputFormat.Network:
                return new DltNetworkTraceFilterReaderFactory(OutputStream, OnlineMode);
            case InputFormat.Pcap:
                return new DltPcapTraceReaderFactory(OutputStream);
            default:
                throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
            }
        }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            return GetFactory().CreateAsync(stream);
        }

        /// <summary>
        /// Creates a DLT Trace Reader from a stream.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(string fileName)
        {
            return GetFactory().CreateAsync(fileName);
        }
    }
}
