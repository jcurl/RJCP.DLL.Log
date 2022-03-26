namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response when the Dlt module sends this message when the dlt message buffer overflows.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release R20-11 specification.</remarks>
    public sealed class BufferOverflowNotificationResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferOverflowNotificationResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="overflowCounter">The number of lost DLT messages since the last overflow message.</param>
        public BufferOverflowNotificationResponse(int status, int overflowCounter) : base(status)
        {
            Counter = overflowCounter;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x23.</value>
        public override int ServiceId { get { return 0x23; } }

        /// <summary>
        /// The number of lost DLT messages since the last overflow message.
        /// </summary>
        /// <value>The number of lost DLT messages since the last overflow message.</value>
        public int Counter { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Status == StatusOk ?
                string.Format("[buffer_overflow {0}] {1}", ToString(Status), unchecked((uint)Counter)) :
                string.Format("[buffer_overflow {0}]", ToString(Status));
        }
    }
}
