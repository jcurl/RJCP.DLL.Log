namespace RJCP.App.DltDump.Services
{
    using System.Diagnostics;
    using Infrastructure.Text;

    /// <summary>
    /// A convenience class for writing formatted information to the console.
    /// </summary>
    public static class Terminal
    {
        /// <summary>
        /// Writes a wrapped line to the console.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLine(string message)
        {
            Log.AppTerminal.TraceEvent(TraceEventType.Information, "{0}", message);
            string[] lines = Format.Wrap(Global.Instance.Terminal.TerminalWidth - 1, message);
            foreach (string line in lines) {
                Global.Instance.Terminal.StdOut.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes a wrapped line to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public static void WriteLine(string format, params object[] args)
        {
            string message = string.Format(format, args);
            WriteLine(message);
        }

        /// <summary>
        /// Writes a wrapped line to the console.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <param name="hangingIndent">The hanging indent.</param>
        /// <param name="message">The message to write.</param>
        public static void WriteLine(int indent, int hangingIndent, string message)
        {
            Log.AppTerminal.TraceEvent(TraceEventType.Information, "{0}", message);
            string[] output = Format.Wrap(Global.Instance.Terminal.TerminalWidth - 1, indent, hangingIndent, message);

            if (output is object) {
                foreach (string line in output) {
                    Global.Instance.Terminal.StdOut.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Writes a wrapped line to the console.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <param name="hangingIndent">The hanging indent.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public static void WriteLine(int indent, int hangingIndent, string format, params object[] args)
        {
            string message = string.Format(format, args);
            WriteLine(indent, hangingIndent, message);
        }

        /// <summary>
        /// Writes the line directly to the console without wrapping.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteDirect(string message)
        {
            Global.Instance.Terminal.StdOut.WriteLine(message);
            Log.AppTerminal.TraceEvent(TraceEventType.Information, "{0}", message);
        }

        /// <summary>
        /// Writes the line directly to the console without wrapping.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public static void WriteDirect(string format, params object[] args)
        {
            string message = string.Format(format, args);
            WriteDirect(message);
        }
    }
}
