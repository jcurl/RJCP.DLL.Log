namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to the message the Dlt module sends this message when the dlt message buffer overflows.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release R20-11 specification.</remarks>
    public sealed class BufferOverflowNotificationRequest : ControlRequest
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x23.</value>
        public override int ServiceId { get { return 0x23; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "[buffer_overflow]";
        }
    }
}
