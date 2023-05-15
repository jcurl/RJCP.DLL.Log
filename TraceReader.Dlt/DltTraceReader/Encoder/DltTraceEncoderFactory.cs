namespace RJCP.Diagnostics.Log.Encoder
{
    using Dlt;

    /// <summary>
    /// Factory for creating a <see cref="DltTraceEncoder"/> with default options.
    /// </summary>
    public class DltTraceEncoderFactory : ITraceEncoderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Creates a <see cref="DltTraceEncoder" /> object with defaults.
        /// </summary>
        /// <returns>An instance of a <see cref="DltTraceEncoder" />.</returns>
        public ITraceEncoder<DltTraceLineBase> Create()
        {
            return new DltTraceEncoder();
        }
    }
}
