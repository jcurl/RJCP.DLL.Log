namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Decodes a DLT service for a connection info event.
    /// </summary>
    /// <remarks>
    /// This is not specified in AutoSAR R20-11 or earlier, but is implemented by the Genivi DLT as of
    /// a3c77c3d9bd7523d8dc4f6401109d29f973b01ba.
    /// </remarks>
    public sealed class CustomConnectionInfoResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUnregisterContextResponse" /> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="state">The connection state.</param>
        public CustomConnectionInfoResponse(int status, int state)
            : this(status, state, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUnregisterContextResponse" /> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="state">The connection state.</param>
        /// <param name="comInterface">The communication interface.</param>
        public CustomConnectionInfoResponse(int status, int state, string comInterface)
            : base(status)
        {
            ConnectionState = state;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId
        {
            get { return 0xF02; }
        }

        /// <summary>
        /// Gets the state of the connection.
        /// </summary>
        /// <value>The state of the connection.</value>
        public int ConnectionState { get; }

        /// <summary>
        /// Gets the communication interface.
        /// </summary>
        /// <value>The communication interface.</value>
        public string ComInterface { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            string connState;
            switch (ConnectionState) {
            case 1:
                connState = "disconnected";
                break;
            case 2:
                connState = "connected";
                break;
            default:
                connState = "unknown";
                break;
            }

            if (string.IsNullOrEmpty(ComInterface))
                return string.Format("[connection_info {0}] {1}", ToString(Status), connState);

            return string.Format("[connection_info {0}] {1} {2}", ToString(Status), connState, ComInterface);
        }
    }
}
