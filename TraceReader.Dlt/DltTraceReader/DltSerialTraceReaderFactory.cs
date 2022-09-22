namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltSerialTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        public DltSerialTraceReaderFactory() : base(new DltSerialTraceDecoderFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltSerialTraceReaderFactory(bool online) : base(new DltSerialTraceDecoderFactory(online)) { }
    }
}
