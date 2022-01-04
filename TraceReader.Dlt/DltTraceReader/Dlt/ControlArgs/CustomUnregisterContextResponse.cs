namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Decodes a DLT service for an unregistered context.
    /// </summary>
    /// <remarks>
    /// This is not specified in AutoSAR R20-11 or earlier, but is implemented by the Genivi DLT as of
    /// a3c77c3d9bd7523d8dc4f6401109d29f973b01ba.
    /// </remarks>
    public sealed class CustomUnregisterContextResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUnregisterContextResponse" /> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <param name="comInterface">The communication interface.</param>
        public CustomUnregisterContextResponse(int status, string appId, string contextId, string comInterface)
            : base(status)
        {
            ApplicationId = appId ?? string.Empty;
            ContextId = contextId ?? string.Empty;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId
        {
            get { return 0xF01; }
        }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string ApplicationId { get; }

        /// <summary>
        /// Gets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        public string ContextId { get; }

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
            if (string.IsNullOrEmpty(ComInterface)) {
                if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                    return string.Format("[unregister_context {0}]", ToString(Status));

                return string.Format("[unregister_context {0}] {1} ({2})",
                    ToString(Status), ApplicationId, ContextId);
            }

            if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                return string.Format("[unregister_context {0}] {1}", ToString(Status), ComInterface);

            return string.Format("[unregister_context {0}] {1} ({2}) {3}",
                ToString(Status), ApplicationId, ContextId, ComInterface);
        }
    }
}
