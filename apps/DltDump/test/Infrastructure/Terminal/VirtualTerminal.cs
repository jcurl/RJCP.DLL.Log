namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;

    /// <summary>
    /// Standard output test class.
    /// </summary>
    public class VirtualTerminal : ITerminal
    {
        /// <summary>
        /// The Standard Error test output writer.
        /// </summary>
        /// <value>The standard error test output writer.</value>
        public ITerminalOut StdErr { get; } = new VirtualStdErr();

        /// <summary>
        /// The Standard Output, test output writer.
        /// </summary>
        /// <value>The standard output, test output writer.</value>
        public ITerminalOut StdOut { get; } = new VirtualStdOut();

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public int TerminalWidth { get { return 80; } }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public int TerminalHeight { get { return 25; } }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public virtual ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public virtual ConsoleColor ForegroundColor { get; set; }
    }
}
