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

        private TraceReaderFactory<DltTraceLineBase> GetFactory()
        {
            switch (InputFormat) {
            case InputFormat.File:
                return new DltFileTraceReaderFactory();
            case InputFormat.Serial:
                return new DltSerialTraceReaderFactory(OnlineMode);
            case InputFormat.Network:
                return new DltTraceReaderFactory(OnlineMode);
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
