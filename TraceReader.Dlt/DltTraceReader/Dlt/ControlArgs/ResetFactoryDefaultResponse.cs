namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to set the configuration of DLT to factory defaults.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class ResetFactoryDefaultResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetFactoryDefaultResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public ResetFactoryDefaultResponse(int status) : base(status) { }

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
            return string.Format("[reset_to_factory_default {0}]", ToString(Status));
        }
    }
}
