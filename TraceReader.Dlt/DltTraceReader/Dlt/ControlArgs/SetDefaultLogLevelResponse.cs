namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to modify the pass through range for log messages for all not explicit set Context IDs.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetDefaultLogLevelResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultLogLevelResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public SetDefaultLogLevelResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x11.</value>
        public override int ServiceId { get { return 0x11; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[set_default_log_level {0}]", ToString(Status));
        }
    }
}
