namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Choose the style of test case for decoding.
    /// </summary>
    public enum DecoderType
    {
        /// <summary>
        /// Decode bytes that is expected to be parsed by a line decoder (using a stream).
        /// </summary>
        Line,

        /// <summary>
        /// Decode bytes using <see cref="VerboseDltDecoder"/> with the default argument decoder.
        /// </summary>
        Packet,

        /// <summary>
        /// Decode bytes using a specific instance of <see cref="IVerboseArgDecoder"/>.
        /// </summary>
        Specialized,
    }
}
