namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// Factory for creating a <see cref="DltTraceDecoder"/>.
    /// </summary>
    public class DltTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;
        private readonly IFrameMap m_Map;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderFactory"/> class.
        /// </summary>
        public DltTraceDecoderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltTraceDecoderFactory(bool online)
        {
            m_Online = online;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltTraceDecoderFactory(IFrameMap map)
        {
            m_Map = map;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltTraceDecoderFactory(bool online, IFrameMap map)
        {
            m_Online = online;
            m_Map = map;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="DltTraceDecoder" />.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltTraceDecoder" /> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            if (m_Map == null)
                return new DltTraceDecoder(m_Online);

            return new DltTraceDecoder(m_Online, m_Map);
        }
    }
}
