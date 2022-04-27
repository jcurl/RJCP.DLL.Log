namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Constructs a <see cref="TraceReader{T}"/> for decoding PCAP traces.
    /// </summary>
    public class DltPcapTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        private readonly IOutputStream m_OutputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltPcapTraceReaderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltPcapTraceReaderFactory(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_OutputStream = outputStream;
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="DltTraceDecoder"/> that knows how to decode the stream.</returns>
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            if (m_OutputStream == null) {
                return new DltPcapTraceDecoder();
            }

            return new DltPcapTraceDecoder(m_OutputStream);
        }
    }
}
