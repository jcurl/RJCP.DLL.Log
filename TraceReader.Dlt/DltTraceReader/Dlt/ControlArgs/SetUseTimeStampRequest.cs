namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to enable/disable the transmission of the Time Stamp in the header.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetUseTimeStampRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUseTimeStampRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// Set to <see langword="true"/> if the Time Stamp is enabled, <see langword="false"/> otherwise.
        /// </param>
        public SetUseTimeStampRequest(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0F.</value>
        public override int ServiceId { get { return 0x0F; } }

        /// <summary>
        /// Gets the status for the time stamp transmission.
        /// </summary>
        /// <value>The status for the time stamp transmission.</value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[use_timestamp] {0}", Enabled ? "on" : "off");
        }
    }
}
