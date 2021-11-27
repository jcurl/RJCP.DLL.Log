namespace RJCP.Diagnostics.Log
{
    /// <summary>
    /// Interface for a basic TraceLine from a log file.
    /// </summary>
    public interface ITraceLine
    {
        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>
        /// This is a string, that should normally present the text log that was emitted by the device being logged.
        /// That is, all metadata, such as timestamps, severity and other information is not present in this string.
        /// </value>
        string Text { get; }

        /// <summary>
        /// The current line in the log file.
        /// </summary>
        /// <value>This is the line number that this line of text was found on.</value>
        int Line { get; }

        /// <summary>
        /// The position in the file.
        /// </summary>
        /// <value>This is the offset in the file where the line begins.</value>
        long Position { get; }
    }
}
