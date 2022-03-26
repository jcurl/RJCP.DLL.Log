namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to tell that a Message Buffer Overflow happens. This can be requested by a client. Additionally this
    /// message is actively send by DLT at the point when a Message Buffer Overflow happens.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class MessageBufferOverflowResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBufferOverflowResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="overflow">Indicates if an overflow has occurred or not.</param>
        public MessageBufferOverflowResponse(int status, bool overflow) : base(status)
        {
            Overflow = overflow;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x14.</value>
        public override int ServiceId { get { return 0x14; } }

        /// <summary>
        /// Gets a value indicating whether a message buffer overflow occurred.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a messages buffer overflow occurred, <see langword="false"/> otherwise.
        /// </value>
        public bool Overflow { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Status == StatusOk ?
                string.Format("[message_buffer_overflow {0}] {1}", ToString(Status), Overflow ? "true" : "false") :
                string.Format("[message_buffer_overflow {0}]", ToString(Status));
        }
    }
}
