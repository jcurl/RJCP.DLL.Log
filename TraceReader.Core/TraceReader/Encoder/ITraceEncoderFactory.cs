namespace RJCP.Diagnostics.Log.Encoder
{
    /// <summary>
    /// Interface for creating objects of type <see cref="ITraceEncoder{TLine}"/>.
    /// </summary>
    /// <typeparam name="TLine">The type of trace line the encoder can serialize.</typeparam>
    public interface ITraceEncoderFactory<in TLine> where TLine : ITraceLine
    {
        /// <summary>
        /// Creates a <see cref="ITraceEncoder{TLine}"/> object.
        /// </summary>
        /// <returns>An instance of a <see cref="ITraceEncoder{TLine}"/>.</returns>
        ITraceEncoder<TLine> Create();
    }
}
