namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

    /// <summary>
    /// A factory to create a <see cref="IPcapTraceDecoder"/> object.
    /// </summary>
    /// <remarks>
    /// Required by the <see cref="Connection"/> object, to create a decoder that can interpret the contents of Ethernet
    /// packet payloads, and the time stamp is derived from the PCAP format, and not the DLT format.
    /// </remarks>
    public class PcapTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly IOutputStream m_OutputStream;
        private readonly IFrameMap m_FrameMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapTraceDecoderFactory"/> class.
        /// </summary>
        public PcapTraceDecoderFactory() : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        public PcapTraceDecoderFactory(IOutputStream outputStream) : this(outputStream, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="map">
        /// The map that can decode non-verbose messages. This is given to the decoder when it is created.
        /// </param>
        public PcapTraceDecoderFactory(IFrameMap map) : this(null, map) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="map">
        /// The map that can decode non-verbose messages. This is given to the decoder when it is created.
        /// </param>
        public PcapTraceDecoderFactory(IOutputStream outputStream, IFrameMap map)
        {
            m_OutputStream = outputStream;
            m_FrameMap = map;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="ITraceDecoder{DltTraceLineBase}"/>.
        /// </summary>
        /// <returns>
        /// A new instance of a <see cref="ITraceDecoder{DltTraceLineBase}"/> object that fulfills the interface
        /// <see cref="IPcapTraceDecoder"/>.
        /// </returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            return new DltPcapNetworkTraceFilterDecoder(m_OutputStream, m_FrameMap);
        }
    }
}
