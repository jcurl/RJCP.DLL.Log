namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;

    /// <summary>
    /// The Standard Error Output.
    /// </summary>
    public class StdErr : ITerminalOut
    {
        /// <summary>
        /// Writes the line without a line feed.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public void Write(string line)
        {
            Console.Error.Write(line);
        }

        /// <summary>
        /// Writes the line of the specified format without a line feed.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.FormatException">The format specification in format is invalid.</exception>
        public void Write(string format, params object[] args)
        {
            Console.Error.Write(format, args);
        }

        /// <summary>
        /// Writes the line followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public void WriteLine(string line)
        {
            Console.Error.WriteLine(line);
        }

        /// <summary>
        /// Writes the line followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">The format specification in format is invalid.</exception>
        public void WriteLine(string format, params object[] args)
        {
            Console.Error.WriteLine(format, args);
        }
    }
}
