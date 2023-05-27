namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Indicates a control message that is decoded as having an error, or is unknown.
    /// </summary>
    public sealed class ControlErrorNotSupported : ControlResponse
    {
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlErrorNotSupported" /> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="status">The status of the control response message.</param>
        /// <param name="name">The name of the message, used for generating the string.</param>
        public ControlErrorNotSupported(int serviceId, int status, string name)
            : base(status)
        {
            ServiceId = serviceId;
            m_Name = name ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x23.</value>
        public override int ServiceId { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[{0} {1}]", m_Name, ToString(Status));
        }
    }
}
