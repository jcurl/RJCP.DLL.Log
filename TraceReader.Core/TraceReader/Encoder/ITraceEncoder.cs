namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using RJCP.Core;

    /// <summary>
    /// Interface for packet based trace encoder
    /// </summary>
    /// <typeparam name="TLine">The type of trace line the encoder accepts.</typeparam>
    public interface ITraceEncoder<in TLine> : IDisposable where TLine : ITraceLine
    {
        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        Result<int> Encode(Span<byte> buffer, TLine line);
    }
}
