namespace RJCP.Diagnostics.Log.Decoder
{
    /// <summary>
    /// Interface for creating objects of type <see cref="ITraceDecoder{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    public interface ITraceDecoderFactory<out T> where T : class, ITraceLine
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ITraceDecoder{T}"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="ITraceDecoder{T}"/> object.</returns>
        ITraceDecoder<T> Create();
    }
}
