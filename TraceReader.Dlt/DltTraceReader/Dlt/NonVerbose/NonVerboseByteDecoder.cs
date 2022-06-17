namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes non-verbose payloads, not interpreting the contents.
    /// </summary>
    public class NonVerboseByteDecoder : INonVerboseDltDecoder
    {
        /// <summary>
        /// Decodes the specified buffer as a verbose payload.
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
