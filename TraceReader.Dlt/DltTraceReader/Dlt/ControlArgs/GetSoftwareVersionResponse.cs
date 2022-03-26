namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response for the software version.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetSoftwareVersionResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSoftwareVersionResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="swVersion">The software version in the control message.</param>
        public GetSoftwareVersionResponse(int status, string swVersion) : base(status)
        {
            SwVersion = swVersion ?? string.Empty;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x13.</value>
        public override int ServiceId { get { return 0x13; } }

        /// <summary>
        /// Gets the software version.
        /// </summary>
        /// <value>The software version.</value>
        public string SwVersion { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Status == StatusOk ?
                string.Format("[get_software_version {0}] {1}", ToString(Status), SwVersion) :
                string.Format("[get_software_version {0}]", ToString(Status));
        }
    }
}
