namespace RJCP.Diagnostics.Log.Decoder
{
    using Dlt;

    /// <summary>
    /// Factory for creating a <see cref="DltFileTraceDecoder"/>.
    /// </summary>
    public class DltFileTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="DltFileTraceDecoder"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="DltFileTraceDecoder"/> object.</returns>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            return new DltFileTraceDecoder();
        }
    }
}
