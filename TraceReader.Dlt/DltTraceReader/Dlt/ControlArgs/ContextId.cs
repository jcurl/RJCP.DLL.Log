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
    public sealed class ContextId
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
        /// Initializes a new instance of the <see cref="ContextId"/> class.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        public ContextId(string name) : this(name, LogLevel.Block, StatusUndefined, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextId"/> class.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <param name="logLevel">The log level of the context identified by the given <paramref name="name"/>.</param>
        /// <param name="traceStatus">
        /// The trace status of the context identified by the given <paramref name="name"/>.
        /// </param>
        public ContextId(string name, LogLevel logLevel, int traceStatus) : this(name, logLevel, traceStatus, string.Empty) { }

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
        public ContextId(string name, LogLevel logLevel, int traceStatus, string description)
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
        /// <term>DLT_LOG_FATAL ( <see cref="LogLevel.Fatal"/>)</term>
        /// <description>Fatal system errors, should be very rare.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_ERROR ( <see cref="LogLevel.Error"/>)</term>
        /// <description>Errors occurring in a SW-C with impact to correct functionality.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_WARN ( <see cref="LogLevel.Warn"/>)</term>
        /// <description>Log messages where a incorrect behavior can not be ensured.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_INFO ( <see cref="LogLevel.Info"/>)</term>
        /// <description>
        /// Log messages providing information for better understanding the internal behavior of a software.
        /// </description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_DEBUG ( <see cref="LogLevel.Debug"/>)</term>
        /// <description>Should be used for messages which are only for debugging of a software usable.</description>
        /// </item>
        /// <item>
        /// <term>DLT_LOG_VERBOSE ( <see cref="LogLevel.Verbose"/>)</term>
        /// <description>
        /// Log messages with the highest communicative level, here all possible states, information and everything else
        /// can be logged.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public LogLevel LogLevel { get; }

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
            if (LogLevel != LogLevel.Undefined) {
                strBuilder.Append(' ').Append(LogLevel.GetDescription());
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
