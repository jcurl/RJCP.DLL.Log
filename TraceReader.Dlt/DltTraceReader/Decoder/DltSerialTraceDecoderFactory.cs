namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;

    /// <summary>
    /// Factory for creating a <see cref="DltSerialTraceDecoder"/> object.
    /// </summary>
    public class DltSerialTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;

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
        /// Creates a new instance of a <see cref="DltSerialTraceDecoder" />.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltSerialTraceDecoder" /> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            return new DltSerialTraceDecoder(m_Online);
        }
    }
}
