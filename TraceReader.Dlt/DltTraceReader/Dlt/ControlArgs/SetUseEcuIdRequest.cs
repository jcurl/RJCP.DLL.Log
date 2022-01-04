namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to enable/disable the transmission of the ECU in the header.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetUseEcuIdRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUseEcuIdRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// Set to <see langword="true"/> if the ECU ID is enabled, <see langword="false"/> otherwise.
        /// </param>
        public SetUseEcuIdRequest(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0D.</value>
        public override int ServiceId { get { return 0x0D; } }

        /// <summary>
        /// Gets the status for the ECU ID transmission.
        /// </summary>
        /// <value>The status for the ECU ID transmission.</value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[use_ecu_id] {0}", Enabled ? "on" : "off");
        }
    }
}
