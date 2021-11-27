namespace RJCP.Diagnostics.Log.Decoder
{
    using System;

    /// <summary>
    /// Decode lines from a log file as text strings.
    /// </summary>
    public sealed class TextDecoder : TextDecoderBase<TraceLine>
    {
        private int m_Line;

        /// <summary>
        /// Gets the line that was decoded.
        /// </summary>
        /// <param name="line">The line as a memory slice.</param>
        /// <param name="position">The position in the stream where the memory slice starts.</param>
        /// <returns>A new <see cref="ITraceLine"/> derived object.</returns>
        protected override TraceLine GetLine(Span<char> line, long position)
        {
            TraceLine traceLine = new TraceLine(new string(line), m_Line, position);
            m_Line++;
            return traceLine;
        }
    }
}
