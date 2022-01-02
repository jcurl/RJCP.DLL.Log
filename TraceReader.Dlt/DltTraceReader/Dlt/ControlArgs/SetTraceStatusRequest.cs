namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to enable or disable trace messages for a given Context ID.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetTraceStatusRequest : ControlRequest
    {
        /// <summary>
        /// Default log level.
        /// </summary>
        public const int LogLevelDefault = -1;

        /// <summary>
        /// Disable logging.
        /// </summary>
        public const int LogLevelDisabled = 0;

        /// <summary>
        /// Enable logging.
        /// </summary>
        public const int LogLevelEnabled = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTraceStatusRequest"/> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <param name="logLevel">
        /// The new log level which is expected to have a value of <see cref="LogLevelDefault"/>,
        /// <see cref="LogLevelEnabled"/> or <see cref="LogLevelDisabled"/>.
        /// </param>
        public SetTraceStatusRequest(string appId, string contextId, int logLevel)
            : this(appId, contextId, logLevel, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTraceStatusRequest"/> class.
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
        public SetTraceStatusRequest(string appId, string contextId, int logLevel, string comInterface)
        {
            ApplicationId = appId ?? string.Empty;
            ContextId = contextId ?? string.Empty;
            ComInterface = comInterface ?? string.Empty;
            TraceStatus = logLevel;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x02.</value>
        public override int ServiceId { get { return 0x02; } }

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
        /// Gets the new trace status.
        /// </summary>
        /// <value>The new trace status.</value>
        public int TraceStatus { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ComInterface)) {
                if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                    return string.Format("[set_trace_status] {0}", ToString(TraceStatus));

                return string.Format("[set_trace_status] {0} {1} ({2})",
                    ToString(TraceStatus), ApplicationId, ContextId);
            }

            if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                return string.Format("[set_trace_status] {0} {1}", ToString(TraceStatus), ComInterface);

            return string.Format("[set_trace_status] {0} {1} ({2}) {3}",
                ToString(TraceStatus), ApplicationId, ContextId, ComInterface);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        private static string ToString(int logLevel)
        {
            switch (logLevel) {
            case LogLevelDefault:
                return "default";
            case LogLevelDisabled:
                return "off";
            case LogLevelEnabled:
                return "on";
            default:
                return string.Format("status={0}", logLevel);
            }
        }
    }
}
