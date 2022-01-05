namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to send this message to report the synchronized absolute time.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release R20-11 specification.</remarks>
    public sealed class SyncTimeStampRequest : ControlRequest
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x24.</value>
        public override int ServiceId { get { return 0x24; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "[sync_timestamp]";
        }
    }
}
