namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// Factory for creating a <see cref="DltFileTraceDecoder"/>.
    /// </summary>
    public class DltFileTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly IFrameMap m_Map;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceDecoderFactory"/> class.
        /// </summary>
        public DltFileTraceDecoderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltFileTraceDecoderFactory(IFrameMap map)
        {
            m_Map = map;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="DltFileTraceDecoder"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltFileTraceDecoder"/> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            if (m_Map == null)
                return new DltFileTraceDecoder();

            return new DltFileTraceDecoder(m_Map);
        }
    }
}
