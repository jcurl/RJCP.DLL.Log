namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes non-verbose payloads.
    /// </summary>
    public class NonVerboseDltDecoder : INonVerboseDltDecoder
    {
        /// <summary>
        /// Gets the frame map, which maps identifiers into frames consisting of arguments to construct a
        /// <see cref="DltTraceLine"/>.
        /// </summary>
        /// <value>The frame map.</value>
        public IFrameMap FrameMap { get { return null; } }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer that should be decoded.</param>
        /// <param name="lineBuilder">
        /// The line builder providing information from the standard header, and where the decoded packets will be
        /// placed.
        /// </param>
        /// <returns>The length of all the decoded verbose arguments in the buffer.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            try {
                NonVerboseDltArg arg;
                if (buffer.Length < 4) {
                    lineBuilder.SetMessageId(0);
                    arg = new NonVerboseDltArg(Array.Empty<byte>());
                } else {
                    int messageId = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    lineBuilder.SetMessageId(messageId);
                    arg = new NonVerboseDltArg(buffer[4..].ToArray());
                }
                lineBuilder.AddArgument(arg);
                return buffer.Length;
            } catch (Exception ex) {
                Log.Dlt.TraceException(ex, nameof(Decode), "Exception while decoding");
                return -1;
            }
        }
    }
}
