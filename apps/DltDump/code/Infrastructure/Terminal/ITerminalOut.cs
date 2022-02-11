namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    /// <summary>
    /// IOutputWriter that allows writing to a specific output.
    /// </summary>
    public interface ITerminalOut
    {
        /// <summary>
        /// Writes the line without a line feed.
        /// </summary>
        /// <param name="line">The line.</param>
        void Write(string line);

        /// <summary>
        /// Writes the line of the specified format without a line feed.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        void Write(string format, params object[] args);

        /// <summary>
        /// Writes the line followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="line">The line to be written.</param>
        void WriteLine(string line);

        /// <summary>
        /// Writes the line followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        void WriteLine(string format, params object[] args);
    }
}
