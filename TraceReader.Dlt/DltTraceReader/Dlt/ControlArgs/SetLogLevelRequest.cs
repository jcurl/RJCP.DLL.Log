namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to modify the pass through range for log messages for a given Context ID.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetLogLevelRequest : LogLevelRequestBase
    {
        /// <summary>
        /// Default log level.
        /// </summary>
        public const int LogLevelDefault = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetLogLevelRequest"/> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <param name="logLevel">
        /// The new log level which is expected to have a value of <see cref="LogLevelDefault"/>,
        /// <see cref="LogLevelRequestBase.LogLevelBlock"/>, <see cref="LogLevelRequestBase.LogLevelFatal"/>,
        /// <see cref="LogLevelRequestBase.LogLevelError"/>, <see cref="LogLevelRequestBase.LogLevelWarn"/>,
        /// <see cref="LogLevelRequestBase.LogLevelInfo"/>, <see cref="LogLevelRequestBase.LogLevelDebug"/> or
        /// <see cref="LogLevelRequestBase.LogLevelVerbose"/>.
        /// </param>
        public SetLogLevelRequest(string appId, string contextId, int logLevel)
            : this(appId, contextId, logLevel, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetLogLevelRequest"/> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <param name="logLevel">
        /// The new log level which is expected to have a value of <see cref="LogLevelDefault"/>,
        /// <see cref="LogLevelRequestBase.LogLevelBlock"/>, <see cref="LogLevelRequestBase.LogLevelFatal"/>,
        /// <see cref="LogLevelRequestBase.LogLevelError"/>, <see cref="LogLevelRequestBase.LogLevelWarn"/>,
        /// <see cref="LogLevelRequestBase.LogLevelInfo"/>, <see cref="LogLevelRequestBase.LogLevelDebug"/> or
        /// <see cref="LogLevelRequestBase.LogLevelVerbose"/>.
        /// </param>
        /// <param name="comInterface">The communication interface.</param>
        public SetLogLevelRequest(string appId, string contextId, int logLevel, string comInterface)
            : base(logLevel)
        {
            ApplicationId = appId ?? string.Empty;
            ContextId = contextId ?? string.Empty;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x01.</value>
        public override int ServiceId { get { return 0x01; } }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string ApplicationId { get; }

        /// <summary>
        /// Gets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        public string ContextId { get; }

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
            if (string.IsNullOrEmpty(ComInterface)) {
                if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                    return string.Format("[set_log_level] {0}", ToString(LogLevel));

                return string.Format("[set_log_level] {0} {1} ({2})",
                    ToString(LogLevel), ApplicationId, ContextId);
            }

            if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                return string.Format("[set_log_level] {0} {1}", ToString(LogLevel), ComInterface);

            return string.Format("[set_log_level] {0} {1} ({2}) {3}",
                ToString(LogLevel), ApplicationId, ContextId, ComInterface);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        protected override string ToString(int logLevel)
        {
            if (logLevel == LogLevelDefault) return "default";
            return base.ToString(logLevel);
        }
    }
}
