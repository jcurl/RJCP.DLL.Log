namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to enable or disable trace messages for a given Context ID.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetTraceStatusResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTraceStatusResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public SetTraceStatusResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x02.</value>
        public override int ServiceId { get { return 0x02; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[set_trace_status {0}]", ToString(Status));
        }
    }
}
