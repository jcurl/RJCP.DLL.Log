namespace RJCP.App.DltDump.Services
{
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;

    /// <summary>
    /// Describes an event log while parsing FIBEX files.
    /// </summary>
    public class FibexLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FibexLogEntry"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file where the warning occurred.</param>
        /// <param name="warning">The warning that occurred.</param>
        /// <param name="message">The message associated with the warning.</param>
        public FibexLogEntry(string fileName, FibexWarnings warning, string message)
        {
            FileName = fileName;
            Warning = warning;
            Message = message;
        }

        public FibexWarnings Warning { get; }

        public string Message { get; }

        public string FileName { get; }
    }
}
