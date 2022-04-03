namespace RJCP.App.DltDump.Domain
{
    /// <summary>
    /// Defines the format for reading the output stream.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// Try and determine the format automatically.
        /// </summary>
        Automatic,

        /// <summary>
        /// Write the output to the console.
        /// </summary>
        Console
    }
}
