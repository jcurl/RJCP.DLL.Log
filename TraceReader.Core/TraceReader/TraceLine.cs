namespace RJCP.Diagnostics.Log
{
    /// <summary>
    /// The simplest type of <see cref="ITraceLine"/>.
    /// </summary>
    public class TraceLine : ITraceLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLine"/> class.
        /// </summary>
        /// <param name="text">The log line text.</param>
        /// <param name="line">The line number.</param>
        /// <param name="position">The position in the stream.</param>
        public TraceLine(string text, int line, long position)
        {
            Text = text;
            Line = line;
            Position = position;
        }

        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>
        /// This is a string, that should normally present the text log that was emitted by the device being logged.
        /// That is, all metadata, such as timestamps, severity and other information is not present in this string.
        /// </value>
        public string Text { get; }

        /// <summary>
        /// The current line in the log file.
        /// </summary>
        /// <value>This is the line number that this line of text was found on.</value>
        public int Line { get; }

        /// <summary>
        /// The position in the file.
        /// </summary>
        /// <value>This is the offset in the file where the line begins.</value>
        public long Position { get; }
    }
}
