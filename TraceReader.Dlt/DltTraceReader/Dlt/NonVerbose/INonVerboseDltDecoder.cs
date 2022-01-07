namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;

    /// <summary>
    /// An interface that knows how to decode the DLT payload for non-verbose messages.
    /// </summary>
    public interface INonVerboseDltDecoder
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
        int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder);
    }
}
