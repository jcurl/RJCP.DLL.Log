namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to modify the pass through range for log messages for all not explicit set Context IDs.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetDefaultLogLevelRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultLogLevelRequest"/> class.
        /// </summary>
        /// <param name="logLevel">The new log level.</param>
        public SetDefaultLogLevelRequest(LogLevel logLevel) : this(logLevel, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultLogLevelRequest"/> class.
        /// </summary>
        /// <param name="logLevel">The new log level.</param>
        /// <param name="comInterface">The COM interface.</param>
        public SetDefaultLogLevelRequest(LogLevel logLevel, string comInterface)
        {
            LogLevel = logLevel;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x11.</value>
        public override int ServiceId { get { return 0x11; } }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Gets the communication interface.
        /// </summary>
        /// <value>The communication interface.</value>
        public string ComInterface { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ComInterface))
                return string.Format("[set_default_log_level] {0}", LogLevelExtension.GetDescription(LogLevel));

            return string.Format("[set_default_log_level] {0} {1}", LogLevelExtension.GetDescription(LogLevel), ComInterface);
        }
    }
}
