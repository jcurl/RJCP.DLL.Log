namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Response for request information about registered Application IDs, there Context IDs and the corresponding log
    /// level.
    /// </summary>
    public sealed class GetLogInfoResponse : ControlResponse
    {
        /// <summary>
        /// No log and no trace code.
        /// </summary>
        public const int StatusNoLogNoTrace = 3;

        /// <summary>
        /// With log and no trace code.
        /// </summary>
        public const int StatusWithLogNoTrace = 4;

        /// <summary>
        /// No log and with trace code.
        /// </summary>
        public const int StatusNoLogWithTrace = 5;

        /// <summary>
        /// With log and with trace code.
        /// </summary>
        public const int StatusWithLogWithTrace = 6;

        /// <summary>
        /// Full information code.
        /// </summary>
        public const int StatusFullInfo = 7;

        /// <summary>
        /// No match code.
        /// </summary>
        public const int StatusNoMatch = 8;

        /// <summary>
        /// The response would overflow the size of a single DLT packet.
        /// </summary>
        public const int StatusOverflow = 9;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLogInfoResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public GetLogInfoResponse(int status) : this(status, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLogInfoResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="comInterface">The communication interface name.</param>
        /// <remarks>
        /// Following the Specification of Diagnostic Log and Trace AUTOSAR Release 4.2.2 the communication interface
        /// field can be filled with zeros if it's not used. Thus, it can be set to <see langword="null"/>.
        /// </remarks>
        public GetLogInfoResponse(int status, string comInterface) : base(status)
        {
            AppIds = new List<AppId>();
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x03.</value>
        public override int ServiceId { get { return 0x03; } }

        /// <summary>
        /// Gets the application ids.
        /// </summary>
        /// <value>The application ids.</value>
        public IList<AppId> AppIds { get; }

        /// <summary>
        /// Gets or sets the communication interface name.
        /// </summary>
        /// <value>The communication interface name.</value>
        /// <remarks>
        /// Following the DLT Specification of Diagnostic Log and Trace AUTOSAR Release 4.2.2 this is not expected to
        /// exceed 4 characters in length.
        /// </remarks>
        public string ComInterface { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>The <see cref="ComInterface"/> value shall not be included in the result.</remarks>
        public override string ToString()
        {
            if (AppIds.Count == 0) return string.Format("[get_log_info {0}]", ToString(Status));

            StringBuilder result = new StringBuilder();
            result.AppendFormat("[get_log_info {0}]", ToString(Status));
            for (int index = 0; index < AppIds.Count; index++) {
                result.Append(' ');
                AppIds[index].StringAppend(result);
                result.Append(';');
            }

            if (!string.IsNullOrEmpty(ComInterface)) {
                result.Append(' ').Append(ComInterface);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        protected override string ToString(int status)
        {
            switch (status) {
            case 0x00:
                // If the status code value is 0, a string such as [get_log_info ok] shall be returned, even though the
                // Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification does not contain this specific code for
                // the DLT message type. This is being done to maintain similarity to the Genivi implementation,
                // specifically with what the DLT Viewer displays under the Payload column.
                return "ok";
            case StatusNoMatch:
                return "no_matching_context_id";
            case StatusOverflow:
                return "overflow";
            case StatusNoLogNoTrace:
            case StatusWithLogNoTrace:
            case StatusNoLogWithTrace:
            case StatusWithLogWithTrace:
            case StatusFullInfo:
                return status.ToString();
            default:
                return base.ToString(status);
            }
        }
    }
}
