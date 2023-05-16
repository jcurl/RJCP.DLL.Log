namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public sealed class DltSerialTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltSerialTraceReaderFactory(IFrameMap map) : base(new DltSerialTraceDecoderFactory(map)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltSerialTraceReaderFactory(bool online, IFrameMap map) : base(new DltSerialTraceDecoderFactory(online, map)) { }
    }
}
