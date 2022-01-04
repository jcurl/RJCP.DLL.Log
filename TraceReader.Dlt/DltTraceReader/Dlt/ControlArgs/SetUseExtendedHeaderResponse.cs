namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to enable/disable the transmission of the extended header.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetUseExtendedHeaderResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUseExtendedHeaderResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public SetUseExtendedHeaderResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x10.</value>
        public override int ServiceId { get { return 0x10; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[use_extended_header {0}]", ToString(Status));
        }
    }
}
