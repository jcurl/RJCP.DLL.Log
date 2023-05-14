namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;

    /// <summary>
    /// Interface for encoding DLT lines to a buffer.
    /// </summary>
    /// <typeparam name="TLine">The type of the line to encode.</typeparam>
    public interface IDltEncoder<in TLine> where TLine : DltTraceLineBase
    {
        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        int Encode(Span<byte> buffer, TLine line);
    }
}
