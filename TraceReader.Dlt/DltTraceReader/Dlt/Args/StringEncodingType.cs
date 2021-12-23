namespace RJCP.Diagnostics.Log.Dlt.Args
{
    /// <summary>
    /// Defines the encoding used for strings in DLT arguments.
    /// </summary>
    public enum StringEncodingType
    {
        /// <summary>
        /// Encoding for strings is in ASCII.
        /// </summary>
        /// <remarks>Errors in encoding/decoding result in the usage of the ASCII character '?'.</remarks>
        Ascii = 0,

        /// <summary>
        /// Encoding for strings is in UTF8.
        /// </summary>
        /// <remarks>Errors in encoding/decoding result in the usage of the Unicode character U+FFFD.</remarks>
        Utf8 = 1,
    }
}
