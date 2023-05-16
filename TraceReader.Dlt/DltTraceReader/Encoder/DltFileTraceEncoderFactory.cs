namespace RJCP.Diagnostics.Log.Encoder
{
    using Dlt;

    /// <summary>
    /// Factory for creating a <see cref="DltFileTraceEncoder"/> with default options.
    /// </summary>
    public sealed class DltFileTraceEncoderFactory : ITraceEncoderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Creates a <see cref="DltTraceEncoder" /> object with defaults.
        /// </summary>
        /// <returns>An instance of a <see cref="DltFileTraceEncoder" />.</returns>
        public ITraceEncoder<DltTraceLineBase> Create()
        {
            return new DltFileTraceEncoder();
        }
    }
}
