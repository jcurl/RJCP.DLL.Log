namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to switch on/off the message filtering by the DLT module.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetMessageFilteringResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetMessageFilteringResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public SetMessageFilteringResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0A.</value>
        public override int ServiceId { get { return 0x0A; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[set_message_filtering {0}]", ToString(Status));
        }
    }
}
