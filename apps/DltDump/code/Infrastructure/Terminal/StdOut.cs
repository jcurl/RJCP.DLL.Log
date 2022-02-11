namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;

    /// <summary>
    /// The Standard Output.
    /// </summary>
    public class StdOut : ITerminalOut
    {
        /// <summary>
        /// Writes the specified string line to the standard output stream.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public void Write(string line)
        {
            Console.Write(line);
        }

        /// <summary>
        /// Writes the specified line of the specified format without a line feed.
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
            Console.Write(format, args);
        }

        /// <summary>
        /// Writes the line followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
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
        /// <exception cref="System.FormatException">The format specification in format is invalid.</exception>
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
