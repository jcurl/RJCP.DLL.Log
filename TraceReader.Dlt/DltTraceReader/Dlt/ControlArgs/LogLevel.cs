namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Specifies the log level for control messages.
    /// </summary>
    /// <remarks>
    /// For individual lines, see the appropriate message <see cref="DltType"/> with <c>LOG_*</c> methods.
    /// </remarks>
    public enum LogLevel
    {
        /// <summary>
        /// The log level is undefined.
        /// </summary>
        Undefined = -2,

        /// <summary>
        /// The log level is unspecified, and represents the default value.
        /// </summary>
        Default = -1,

        /// <summary>
        /// All messages are blocked.
        /// </summary>
        Block = 0,

        /// <summary>
        /// Fatal log level.
        /// </summary>
        Fatal = 1,

        /// <summary>
        /// Error log level.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Warning log level.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// Information log level.
        /// </summary>
        Info = 4,

        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug = 5,

        /// <summary>
        /// Verbose log level.
        /// </summary>
        Verbose = 6
    }
}
