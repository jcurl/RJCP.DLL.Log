namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// This class represents a decoder error, with the message in the argument string.
    /// </summary>
    public sealed class ControlDecodeError : IControlArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecodeError" /> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The control message type.</param>
        public ControlDecodeError(int serviceId, DltType defaultType)
        {
            ServiceId = serviceId;
            DefaultType = defaultType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecodeError" /> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The control message type.</param>
        /// <param name="message">The message describing the error.</param>
        public ControlDecodeError(int serviceId, DltType defaultType, string message)
        {
            ServiceId = serviceId;
            DefaultType = defaultType;
            SetMessage(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecodeError"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The control message type.</param>
        /// <param name="format">The format message describing the error.</param>
        /// <param name="args">The arguments for the format.</param>
        public ControlDecodeError(int serviceId, DltType defaultType, string format, params object[] args)
        {
            ServiceId = serviceId;
            DefaultType = defaultType;
            SetMessage(format, args);
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x23.</value>
        public int ServiceId { get; }

        /// <summary>
        /// Gets the default type.
        /// </summary>
        /// <value>The default type/subtype of the DLT message that is expected to hold this payload.</value>
        public DltType DefaultType { get; }

        /// <summary>
        /// Gets or sets the message associated with this decoding error.
        /// </summary>
        /// <value>The message associated with this decoding error.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Sets the message.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public void SetMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Sets the message.
        /// </summary>
        /// <param name="format">The format message describing the error.</param>
        /// <param name="args">The arguments for the format.</param>
        public void SetMessage(string format, params object[] args)
        {
            Message = string.Format(format, args);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Message;
        }
    }
}
