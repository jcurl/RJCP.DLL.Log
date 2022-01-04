namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to enable/disable the transmission of the Time Stamp in the header.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetUseTimeStampResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUseTimeStampResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public SetUseTimeStampResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0F.</value>
        public override int ServiceId { get { return 0x0F; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[use_timestamp {0}]", ToString(Status));
        }
    }
}
