namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;

    /// <summary>
    /// Interface for decoding the payload for a control message.
    /// </summary>
    /// <remarks>
    /// Takes the payload of a DLT message after the extended header and decodes the control message. It reads the
    /// service identifier and uses the appropriate decoder for the data based on the service identifier.
    /// </remarks>
    public interface IControlDltDecoder
    {
        /// <summary>
        /// Decodes the DLT control message payload.
        /// </summary>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="lineBuilder">The DLT trace line builder.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        /// <remarks>The result of the decoding is written directly to the <paramref name="lineBuilder"/>.</remarks>
        int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder);
    }
}
