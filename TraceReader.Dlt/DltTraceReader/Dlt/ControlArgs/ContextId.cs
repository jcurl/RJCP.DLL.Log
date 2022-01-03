namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;
    using System.Text;

    /// <summary>
    /// Context Id info type.
    /// </summary>
    /// <remarks>
    /// This is intended to be a data holding class that should be used by the classes implementing support for the Get
    /// Log Info DLT control message payload. It provides a structure support similar to the one being described in
    /// section 7.7.7.1.5.3 ContextIDsInfoType of the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.
    /// <para>
    /// Besides exposing the name and the optional description of the context ID, it also holds information about the
    /// log level and trace status being set for the context ID it represents.
    /// </para>
    /// </remarks>
    public class ContextId
    {
        /// <summary>
        /// The trace status is undefined.
        /// </summary>
        public const int StatusUndefined = sbyte.MinValue;

        /// <summary>
        /// Default trace status.
        /// </summary>
        public const int StatusDefaultTrace = -1;

        /// <summary>
        /// Off trace status.
        /// </summary>
        public const int StatusOff = 0;

        /// <summary>
        /// On trace status.
        /// </summary>
        public const int StatusOn = 1;

        /// <summary>
        /// The log level is undefined.
        /// </summary>
        public const int LogLevelUndefined = sbyte.MinValue;

        /// <summary>
        /// The log level is unspecified.
        /// </summary>
        public const int LogLevelUnspecified = -1;

        /// <summary>
        /// All messages are blocked.
        /// </summary>
        public const int LogLevelBlock = LogLevelRequestBase.LogLevelBlock;

        /// <summary>
        /// Fatal log level.
        /// </summary>
        public const int LogLevelFatal = LogLevelRequestBase.LogLevelFatal;

        /// <summary>
        /// Error log level.
        /// </summary>
        public const int LogLevelError = LogLevelRequestBase.LogLevelError;

        /// <summary>
        /// Warning log level.
        /// </summary>
        public const int LogLevelWarn = LogLevelRequestBase.LogLevelWarn;

        /// <summary>
        /// Info log level.
        /// </summary>
        public const int LogLevelInfo = LogLevelRequestBase.LogLevelInfo;

        /// <summary>
        /// Debug log level.
        /// </summary>
        public const int LogLevelDebug = LogLevelRequestBase.LogLevelDebug;

        /// <summary>
        /// Verbose log level.
        /// </summary>
        public const int LogLevelVerbose = LogLevelRequestBase.LogLevelVerbose;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextId"/> class.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        public ContextId(string name) : this(name, LogLevelBlock, StatusUndefined, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextId"/> class.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <param name="logLevel">The log level of the context identified by the given <paramref name="name"/>.</param>
        /// <param name="traceStatus">
        /// The trace status of the context identified by the given <paramref name="name"/>.
        /// </param>
        public ContextId(string name, int logLevel, int traceStatus) : this(name, logLevel, traceStatus, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextId"/> class.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <param name="logLevel">The log level of the context identified by the given <paramref name="name"/>.</param>
        /// <param name="traceStatus">
        /// The trace status of the context identified by the given <paramref name="name"/>.
        /// </param>
        /// <param name="description">
        /// The description of the context identified by the given <paramref name="name"/>.
        /// </param>
        public ContextId(string name, int logLevel, int traceStatus, string description)
        {
            Name = name ?? string.Empty;
            LogLevel = logLevel;
            TraceStatus = traceStatus;
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Gets the name of this context ID.
        /// </summary>
        /// <value>The name of this context ID.</value>
        /// <remarks>
        /// According to the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification, the name of a context ID is
        /// expected to have a maximum of 4 characters.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the log level of this context ID.
        /// </summary>
        /// <value>The log level of this context ID.</value>
        /// <remarks>
        /// According to the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification, the possible log levels are:
        /// <list type="bullet">
        /// <item>
        /// <term>DLT_LOG_FATAL ( <see cref="LogLevelFatal"/>)</term>
        /// <description>Fatal system errors, should be very rare.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_ERROR ( <see cref="LogLevelError"/>)</term>
        /// <description>Errors occurring in a SW-C with impact to correct functionality.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_WARN ( <see cref="LogLevelWarn"/>)</term>
        /// <description>Log messages where a incorrect behavior can not be ensured.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_INFO ( <see cref="LogLevelInfo"/>)</term>
        /// <description>
        /// Log messages providing information for better understanding the internal behavior of a software.
        /// </description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_DEBUG ( <see cref="LogLevelDebug"/>)</term>
        /// <description>Should be used for messages which are only for debugging of a software usable.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_VERBOSE ( <see cref="LogLevelVerbose"/>)</term>
        /// <description>
        /// Log messages with the highest communicative level, here all possible states, information and everything else
        /// can be logged.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public int LogLevel { get; }

        /// <summary>
        /// Gets the trace status of this context ID.
        /// </summary>
        /// <value>The trace status of this context ID.</value>
        /// <remarks>
        /// According to the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification, the possible trace status
        /// values are:
        /// <list>
        /// <item>
        /// <term>OFF ( <see cref="StatusOff"/>)</term>
        /// </item>
        /// <item>
        /// <term>ON ( <see cref="StatusOn"/>)</term>
        /// </item>
        /// <item>
        /// <term>DEFAULT ( <see cref="StatusDefaultTrace"/>)</term>
        /// <description>The default trace status for this ECU will be used.</description>
        /// </item>
        /// <item>
        /// <term>Undefined ( <see cref="StatusUndefined"/></term>
        /// <description>The trace status is unknown, not reported.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public int TraceStatus { get; }

        /// <summary>
        /// Gets the description of this context ID.
        /// </summary>
        /// <value>The description of this context ID.</value>
        public string Description { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>The result shall not include the <see cref="Description"/> value.</remarks>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, LogLevel, TraceStatus);
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public void StringAppend(StringBuilder strBuilder)
        {
            if (strBuilder == null) throw new ArgumentNullException(nameof(strBuilder));

            strBuilder.Append(Name);
            if (LogLevel == LogLevelUnspecified) {
                strBuilder.Append(" default");
            } else if (LogLevel != LogLevelUndefined) {
                strBuilder.Append(' ').Append(LogLevelRequestBase.LogLevelString(LogLevel));
            }

            switch (TraceStatus) {
            case StatusOff:
                strBuilder.Append(" off");
                break;
            case StatusOn:
                strBuilder.Append(" on");
                break;
            case StatusDefaultTrace:
                strBuilder.Append(" default");
                break;
            case StatusUndefined:
                break;
            default:
                strBuilder.Append(' ').Append(TraceStatus);
                break;
            }
        }
    }
}
