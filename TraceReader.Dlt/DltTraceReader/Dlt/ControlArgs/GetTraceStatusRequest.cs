namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to return the actual trace status.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR PRS 1.4.0 specification.</remarks>
    public sealed class GetTraceStatusRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTraceStatusRequest"/> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="contextId">The context identifier.</param>
        public GetTraceStatusRequest(string appId, string contextId)
        {
            ApplicationId = appId ?? string.Empty;
            ContextId = contextId ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x1F.</value>
        public override int ServiceId { get { return 0x1F; } }

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
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ContextId))
                return "[get_trace_status]";

            return string.Format("[get_trace_status] {0} ({1})", ApplicationId, ContextId);
        }
    }
}
