﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Response to store the actual configuration of Dlt to NVRAM.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class StoreConfigurationResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreConfigurationResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        public StoreConfigurationResponse(int status) : base(status) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x05.</value>
        public override int ServiceId { get { return 0x05; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[store_config {0}]", ToString(Status));
        }
    }
}
