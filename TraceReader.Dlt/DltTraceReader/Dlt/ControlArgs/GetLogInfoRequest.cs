namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;

    /// <summary>
    /// Request information about registered Application IDs, there Context IDs and the corresponding log level.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetLogInfoRequest : ControlRequest
    {
        /// <summary>
        /// Only information about registered Application IDs and Context IDs without log level or trace status
        /// information.
        /// </summary>
        public const int OptionsNoLogNoTrace = 3;

        /// <summary>
        /// Information about registered Application IDs and Context IDs with log level and without trace status
        /// information.
        /// </summary>
        public const int OptionsWithLogNoTrace = 4;

        /// <summary>
        /// Information about registered Application IDs and Context IDs without log level and with trace status
        /// information.
        /// </summary>
        public const int OptionsNoLogWithTrace = 5;

        /// <summary>
        /// Information about registered Application IDs and Context IDs with log level and with trace status
        /// information.
        /// </summary>
        public const int OptionsWithLogWithTrace = 6;

        /// <summary>
        /// Information about registered Application IDs and Context IDs with log level and with trace status
        /// information and all textual descriptions of each Application ID and Context ID.
        /// </summary>
        public const int OptionsFullInfo = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLogInfoRequest"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> is a negative value.</exception>
        /// <remarks>
        /// <paramref name="options"/> is expected to have a value of <see cref="OptionsNoLogNoTrace"/>,
        /// <see cref="OptionsNoLogWithTrace"/>, <see cref="OptionsWithLogNoTrace"/>,
        /// <see cref="OptionsWithLogWithTrace"/> or <see cref="OptionsFullInfo"/>.
        /// </remarks>
        public GetLogInfoRequest(int options, string appId, string contextId) : this(options, appId, contextId, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLogInfoRequest"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <param name="comInterface">The communication interface.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> is a negative value.</exception>
        /// <remarks>
        /// <paramref name="options"/> is expected to have a value of <see cref="OptionsNoLogNoTrace"/>,
        /// <see cref="OptionsNoLogWithTrace"/>, <see cref="OptionsWithLogNoTrace"/>,
        /// <see cref="OptionsWithLogWithTrace"/> or <see cref="OptionsFullInfo"/>.
        /// </remarks>
        public GetLogInfoRequest(int options, string appId, string contextId, string comInterface)
        {
            if (options < 0) throw new ArgumentOutOfRangeException(nameof(options), "Is a negative value");

            Options = options;
            ApplicationId = appId ?? string.Empty;
            ContextId = contextId ?? string.Empty;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x03.</value>
        public override int ServiceId { get { return 0x03; } }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        /// <remarks>
        /// The value of this property is expected to be in the range of <see cref="OptionsNoLogNoTrace"/>,
        /// <see cref="OptionsNoLogWithTrace"/>, <see cref="OptionsWithLogNoTrace"/>,
        /// <see cref="OptionsWithLogWithTrace"/> or <see cref="OptionsFullInfo"/>.
        /// </remarks>
        public int Options { get; }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        /// <remarks>
        /// If this field is <see langword="null"/> or <see cref="string.Empty"/>, all Application IDs with all Context
        /// IDs registered with the ECU are requested.
        /// </remarks>
        public string ApplicationId { get; }

        /// <summary>
        /// Gets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        /// <remarks>
        /// If this field is <see langword="null"/> or <see cref="string.Empty"/>, all Context IDs belonging to the
        /// given Application ID are requested.
        /// </remarks>
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
            if (string.IsNullOrEmpty(ApplicationId)) {
                if (string.IsNullOrEmpty(ComInterface)) {
                    return string.Format("[get_log_info options={0}] -all-", Options);
                }
                return string.Format("[get_log_info options={0}] -all- {1}", Options, ComInterface);
            }

            if (string.IsNullOrEmpty(ComInterface))
                return string.Format("[get_log_info options={0}] {1} ({2})",
                    Options, ApplicationId, ContextId);
            return string.Format("[get_log_info options={0}] {1} ({2}) {3}",
                Options, ApplicationId, ContextId, ComInterface);
        }
    }
}
