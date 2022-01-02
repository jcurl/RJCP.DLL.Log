namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Base class for Log Level operation related DLT request payloads.
    /// </summary>
    public abstract class LogLevelRequestBase : ControlRequest
    {
        /// <summary>
        /// All messages are blocked.
        /// </summary>
        public const int LogLevelBlock = 0;

        /// <summary>
        /// Fatal log level.
        /// </summary>
        public const int LogLevelFatal = 1;

        /// <summary>
        /// Error log level.
        /// </summary>
        public const int LogLevelError = 2;

        /// <summary>
        /// Warning log level.
        /// </summary>
        public const int LogLevelWarn = 3;

        /// <summary>
        /// Information log level.
        /// </summary>
        public const int LogLevelInfo = 4;

        /// <summary>
        /// Debug log level.
        /// </summary>
        public const int LogLevelDebug = 5;

        /// <summary>
        /// Verbose log level.
        /// </summary>
        public const int LogLevelVerbose = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLevelRequestBase"/> class.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        protected LogLevelRequestBase(int logLevel)
        {
            LogLevel = unchecked((sbyte)logLevel);
        }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public int LogLevel { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        protected virtual string ToString(int logLevel)
        {
            return LogLevelString(logLevel);
        }

        /// <summary>
        /// Logs the level string.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The string representation of the log level.</returns>
        public static string LogLevelString(int logLevel)
        {
            switch (logLevel) {
            case LogLevelBlock:
                return "block_all";
            case LogLevelFatal:
                return "fatal";
            case LogLevelError:
                return "error";
            case LogLevelWarn:
                return "warning";
            case LogLevelInfo:
                return "info";
            case LogLevelDebug:
                return "debug";
            case LogLevelVerbose:
                return "verbose";
            default:
                return string.Format("log_level={0}", logLevel);
            }
        }
    }
}
