namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to set the configuration of DLT to factory defaults.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class ResetFactoryDefaultRequest : ControlRequest
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x06.</value>
        public override int ServiceId { get { return 0x06; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "[reset_to_factory_default]";
        }
    }
}
