namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to the actual trace status.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR PRS 1.4.0 specification.</remarks>
    public sealed class GetTraceStatusResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTraceStatusResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="enabled">Indicates if the actual trace status is enabled (on) or not (off).</param>
        public GetTraceStatusResponse(int status, bool enabled) : base(status)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x15.</value>
        public override int ServiceId { get { return 0x1F; } }

        /// <summary>
        /// Gets a value indicating the trace status occurred.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the trace status is enabled, <see langword="false"/> otherwise.
        /// </value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (Status == StatusOk)
                return string.Format("[get_trace_status {0}] {1}", ToString(Status), Enabled ? "on" : "off");

            return string.Format("[get_trace_status {0}]", ToString(Status));
        }
    }
}
