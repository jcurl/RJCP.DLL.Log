namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltTraceReaderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltTraceReaderFactory(bool online)
        {
            m_Online = online;
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="DltTraceDecoder"/> that knows how to decode the stream.</returns>
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            return new DltTraceDecoder(m_Online);
        }
    }
}
