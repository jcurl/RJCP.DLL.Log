namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;

    /// <summary>
    /// Interface for managing a terminal.
    /// </summary>
    public interface ITerminal
    {
        /// <summary>
        /// The Standard Error output writer.
        /// </summary>
        /// <value>The standard error output writer.</value>
        ITerminalOut StdErr { get; }

        /// <summary>
        /// The Standard Output, output writer.
        /// </summary>
        /// <value>The standard output, output writer.</value>
        ITerminalOut StdOut { get; }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        int TerminalWidth { get; }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        int TerminalHeight { get; }

        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        ConsoleColor BackgroundColor { get; set; }
    }
}
