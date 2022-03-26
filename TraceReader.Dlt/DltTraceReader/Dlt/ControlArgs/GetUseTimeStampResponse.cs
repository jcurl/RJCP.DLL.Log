﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to get the DltHeaderUseTimestamp.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class GetUseTimeStampResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUseTimeStampResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="enabled">Indicates if the ECU ID is enabled (on) or not (off).</param>
        public GetUseTimeStampResponse(int status, bool enabled) : base(status)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x1D.</value>
        public override int ServiceId { get { return 0x1D; } }

        /// <summary>
        /// Gets a value indicating whether a message buffer overflow occurred.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a messages buffer overflow occurred, <see langword="false"/> otherwise.
        /// </value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Status == StatusOk ?
                string.Format("[get_use_timestamp {0}] {1}", ToString(Status), Enabled ? "on" : "off") :
                string.Format("[get_use_timestamp {0}]", ToString(Status));
        }
    }
}
