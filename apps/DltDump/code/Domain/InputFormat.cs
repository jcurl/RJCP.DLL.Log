namespace RJCP.App.DltDump.Domain
{
    /// <summary>
    /// Defines the format for reading the input stream.
    /// </summary>
    public enum InputFormat
    {
        /// <summary>
        /// File input stream with a storage header.
        /// </summary>
        File,

        /// <summary>
        /// File input stream with a DLS\1 header.
        /// </summary>
        Serial,

        /// <summary>
        /// File input stream starting with a standard header.
        /// </summary>
        Network
    }
}
