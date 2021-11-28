namespace RJCP.Diagnostics.Log
{
    using System;

    /// <summary>
    /// A Trace Line from a log file with a real-time clock time stamp.
    /// </summary>
    public class LogTraceLine : TraceLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLine"/> class.
        /// </summary>
        /// <param name="text">The log line text.</param>
        /// <param name="line">The line number.</param>
        /// <param name="position">The position in the stream.</param>
        public LogTraceLine(string text, int line, long position)
            : base(text, line, position) { }

        /// <summary>
        /// Gets or sets the time stamp for the log line.
        /// </summary>
        /// <value>The log time stamp.</value>
        public DateTime TimeStamp { get; set; }
    }
}
