namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to get the status of DltHeaderUseSessionID.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetUseSessionIdResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUseSessionIdResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="enabled">Indicates if the ECU ID is enabled (on) or not (off).</param>
        public GetUseSessionIdResponse(int status, bool enabled) : base(status)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x1C.</value>
        public override int ServiceId { get { return 0x1C; } }

        /// <summary>
        /// Gets a value indicating whether a message buffer overflow occurred.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a messages buffer overflow occurred, <see langword="false"/> otherwise.
        /// </value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (Status == StatusOk)
                return string.Format("[get_use_session_id {0}] {1}", ToString(Status), Enabled ? "on" : "off");

            return string.Format("[get_use_session_id {0}]", ToString(Status));
        }
    }
}
