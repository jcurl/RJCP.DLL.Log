namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to switch on/off the continuous cyclic sending of timing packets.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetTimingPacketsRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTimingPacketsRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// Set to <see langword="true"/> if filtering is enabled, <see langword="false"/> otherwise.
        /// </param>
        public SetTimingPacketsRequest(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x0B.</value>
        public override int ServiceId { get { return 0x0B; } }

        /// <summary>
        /// Gets if filtering mode is enabled or not.
        /// </summary>
        /// <value>If filtering mode is enabled or not.</value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[set_timing_packets] {0}", Enabled ? "on" : "off");
        }
    }
}
