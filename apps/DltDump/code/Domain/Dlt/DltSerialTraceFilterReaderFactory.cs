namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltSerialTraceFilterReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        private readonly IOutputStream m_OutputStream;
        private readonly bool m_Online;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltSerialTraceFilterReaderFactory(IOutputStream outputStream, bool online)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_OutputStream = outputStream;
            m_Online = online;
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="DltTraceDecoder"/> that knows how to decode the stream.</returns>
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            return new DltSerialTraceFilterDecoder(m_OutputStream, m_Online);
        }
    }
}
