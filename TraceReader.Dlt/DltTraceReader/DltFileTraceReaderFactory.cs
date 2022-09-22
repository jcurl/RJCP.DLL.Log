namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltFileTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltFileTraceReaderFactory() : base(new DltFileTraceDecoderFactory()) { }
    }
}
