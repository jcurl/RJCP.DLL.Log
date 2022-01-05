namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;

    /// <summary>
    /// Response to send this message to report the synchronized absolute time.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release R20-11 specification.</remarks>
    public class SyncTimeStampResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncTimeStampResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="timeStamp">The time stamp of the device.</param>
        public SyncTimeStampResponse(int status, DateTime timeStamp) : base(status)
        {
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x24.</value>
        public override int ServiceId { get { return 0x24; } }

        /// <summary>
        /// Gets the absolute time stamp of the device.
        /// </summary>
        /// <value>The absolute time stamp of the device.</value>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[sync_timestamp {0}] {1}", ToString(Status), TimeStamp.ToString(@"yyyy-MM-dd HH\:mm\:ss\.fffffK"));
        }
    }
}
