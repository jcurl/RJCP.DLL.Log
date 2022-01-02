namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to enable or disable trace messages for all not explicit set Context IDs.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetDefaultTraceStatusRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultTraceStatusRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// If <see langword="true"/> then trace status is on, otherwise if <see langword="false"/> then off.
        /// </param>
        public SetDefaultTraceStatusRequest(bool enabled) : this(enabled, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultTraceStatusRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// If <see langword="true"/> then trace status is on, otherwise if <see langword="false"/> then off.
        /// </param>
        /// <param name="comInterface">The COM interface.</param>
        public SetDefaultTraceStatusRequest(bool enabled, string comInterface)
        {
            Enabled = enabled;
            ComInterface = comInterface ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x09.</value>
        public override int ServiceId { get { return 0x12; } }

        /// <summary>
        /// Gets the enabled state for the the default trace status.
        /// </summary>
        /// <value>The enabled state for the default trace status.</value>
        public bool Enabled { get; }

        /// <summary>
        /// Gets the communication interface.
        /// </summary>
        /// <value>The communication interface.</value>
        public string ComInterface { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ComInterface))
                return string.Format("[set_default_trace_status] {0}", Enabled ? "on" : "off");

            return string.Format("[set_default_trace_status] {0} {1}", Enabled ? "on" : "off", ComInterface);
        }
    }
}
