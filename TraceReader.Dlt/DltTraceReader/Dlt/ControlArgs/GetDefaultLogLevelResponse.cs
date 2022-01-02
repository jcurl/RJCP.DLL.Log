namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response for the actual default log level.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetDefaultLogLevelResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDefaultLogLevelResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="logLevel">The log level.</param>
        public GetDefaultLogLevelResponse(int status, int logLevel) : base(status)
        {
            LogLevel = logLevel;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x04.</value>
        public override int ServiceId { get { return 0x04; } }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public int LogLevel { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[get_default_log_level {0}] {1}",
                ToString(Status), LogLevelRequestBase.LogLevelString(LogLevel));
        }
    }
}
