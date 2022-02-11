namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;

    /// <summary>
    /// The standard console.
    /// </summary>
    public class ConsoleTerminal : ITerminal
    {
        private readonly bool m_IsRedirected;
        private readonly ITerminalOut m_StdOut = new StdOut();
        private readonly ITerminalOut m_StdErr = new StdErr();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTerminal"/> class.
        /// </summary>
        public ConsoleTerminal()
        {
            m_IsRedirected = Console.IsOutputRedirected && Console.IsErrorRedirected;
        }

        /// <summary>
        /// The Standard Output, output writer.
        /// </summary>
        /// <value>The standard output, output writer.</value>
        public ITerminalOut StdOut { get { return m_StdOut; } }

        /// <summary>
        /// The Standard Error output writer.
        /// </summary>
        /// <value>The standard error output writer.</value>
        public ITerminalOut StdErr { get { return m_StdErr; } }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        /// <exception cref="System.IO.IOException">
        /// The handle to the terminal is invalid, e.g. if redirection has occurred.
        /// </exception>
        public int TerminalWidth
        {
            get
            {
                if (!m_IsRedirected) return Console.BufferWidth;
                return 80;
            }
        }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        /// <exception cref="System.IO.IOException">
        /// The handle to the terminal is invalid, e.g. if redirection has occurred.
        /// </exception>
        public int TerminalHeight
        {
            get
            {
                if (!m_IsRedirected) return Console.WindowHeight;
                return 25;
            }
        }

        private ConsoleColor m_BackgroundColorShadow = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        /// <exception cref="ArgumentException">
        /// The color specified in a set operation is not a valid member of <see cref="ConsoleColor"/>.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        /// <remarks>Under Mono v.5.12.0.301 or lower versions setting this property
        /// is not expected to work.</remarks>
        public virtual ConsoleColor BackgroundColor
        {
            get
            {
                if (!m_IsRedirected) return Console.BackgroundColor;
                return m_BackgroundColorShadow;
            }
            set
            {
                if (!m_IsRedirected) {
                    Console.BackgroundColor = value;
                } else {
                    m_BackgroundColorShadow = value;
                }
            }
        }

        private ConsoleColor m_ForegroundColorShadow = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        /// <exception cref="ArgumentException">
        /// The color specified in a set operation is not a valid member of <see cref="ConsoleColor"/>.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        /// <remarks>Under Mono v.5.12.0.301 or lower versions setting this property
        /// is not expected to work.</remarks>
        public virtual ConsoleColor ForegroundColor
        {
            get
            {
                if (!m_IsRedirected) return Console.ForegroundColor;
                return m_ForegroundColorShadow;
            }
            set
            {
                if (!m_IsRedirected) {
                    Console.ForegroundColor = value;
                } else {
                    m_ForegroundColorShadow = value;
                }
            }
        }
    }
}
