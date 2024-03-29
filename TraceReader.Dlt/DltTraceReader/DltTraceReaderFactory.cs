﻿namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltTraceReaderFactory() : base(new DltTraceDecoderFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltTraceReaderFactory(bool online) : base(new DltTraceDecoderFactory(online)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltTraceReaderFactory(IFrameMap map) : base(new DltTraceDecoderFactory(map)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltTraceReaderFactory(bool online, IFrameMap map) : base(new DltTraceDecoderFactory(online, map)) { }
    }
}
