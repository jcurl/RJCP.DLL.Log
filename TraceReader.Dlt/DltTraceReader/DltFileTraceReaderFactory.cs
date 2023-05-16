namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public sealed class DltFileTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltFileTraceReaderFactory() : base(new DltFileTraceDecoderFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltFileTraceReaderFactory(IFrameMap map) : base(new DltFileTraceDecoderFactory(map)) { }
    }
}
