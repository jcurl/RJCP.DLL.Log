namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;
    using Dlt.NonVerbose;

    /// <summary>
    /// Factory for creating a <see cref="DltSerialTraceDecoder"/> object.
    /// </summary>
    public class DltSerialTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;
        private readonly IFrameMap m_Map;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceDecoderFactory"/> class.
        /// </summary>
        public DltSerialTraceDecoderFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        public DltSerialTraceDecoderFactory(bool online)
        {
            m_Online = online;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltSerialTraceDecoderFactory(IFrameMap map)
        {
            m_Map = map;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialTraceDecoderFactory"/> class.
        /// </summary>
        /// <param name="online">
        /// Set the <see cref="DltTraceLineBase.TimeStamp"/> to the time the message is decoded.
        /// </param>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        public DltSerialTraceDecoderFactory(bool online, IFrameMap map)
        {
            m_Online = online;
            m_Map = map;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="DltSerialTraceDecoder" />.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltSerialTraceDecoder" /> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            if (m_Map is null)
                return new DltSerialTraceDecoder(m_Online);

            return new DltSerialTraceDecoder(m_Online, m_Map);
        }
    }
}
