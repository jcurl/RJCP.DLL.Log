namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;

    /// <summary>
    /// Factory for creating a <see cref="DltTraceDecoder"/>.
    /// </summary>
    public class DltTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly bool m_Online;

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
        /// Creates a new instance of a <see cref="DltTraceDecoder" />.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltTraceDecoder" /> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            return new DltTraceDecoder(m_Online);
        }
    }
}
