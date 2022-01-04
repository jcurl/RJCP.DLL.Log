namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to the actual default trace status.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetDefaultTraceStatusResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDefaultTraceStatusResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="enabled">Indicates if the actual default trace status is enabled (on) or not (off).</param>
        public GetDefaultTraceStatusResponse(int status, bool enabled) : base(status)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x15.</value>
        public override int ServiceId { get { return 0x15; } }

        /// <summary>
        /// Gets a value indicating default trace status is enabled or not.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if default trace status is enabled, <see langword="false"/> otherwise.
        /// </value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (Status == StatusOk)
                return string.Format("[get_default_trace_status {0}] {1}", ToString(Status), Enabled ? "on" : "off");

            return string.Format("[get_default_trace_status {0}]", ToString(Status));
        }
    }
}
