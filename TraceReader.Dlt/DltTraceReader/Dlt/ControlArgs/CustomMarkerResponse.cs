namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Decodes a DLT service for an inserted marker.
    /// </summary>
    /// <remarks>
    /// This is not specified in AutoSAR R20-11 or earlier, but is implemented by the Genivi DLT as of
    /// a3c77c3d9bd7523d8dc4f6401109d29f973b01ba.
    /// </remarks>
    public sealed class CustomMarkerResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMarkerResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public CustomMarkerResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId
        {
            get { return 0xF04; }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "MARKER";
        }
    }
}
