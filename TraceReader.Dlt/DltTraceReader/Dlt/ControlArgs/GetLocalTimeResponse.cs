namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to get a single packet with timestamp (TMSP).
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetLocalTimeResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetLocalTimeResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public GetLocalTimeResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0C.</value>
        public override int ServiceId { get { return 0x0C; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[get_local_time {0}]", ToString(Status));
        }
    }
}
