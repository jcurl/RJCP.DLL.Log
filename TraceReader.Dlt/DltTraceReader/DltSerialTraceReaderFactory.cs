namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltSerialTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        public DltSerialTraceReaderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltSerialTraceReaderFactory(bool online)
        {
            m_Online = online;
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="DltTraceDecoder"/> that knows how to decode the stream.</returns>
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            return new DltSerialTraceDecoder(m_Online);
        }
    }
}
