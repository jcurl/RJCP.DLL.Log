namespace RJCP.App.DltDump.Services
{
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
    }
}
