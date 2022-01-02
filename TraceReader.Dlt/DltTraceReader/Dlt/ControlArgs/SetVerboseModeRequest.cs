﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Request to switch on/off the Verbose Mode.
    /// </summary>
    /// <remarks>Based on the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.</remarks>
    public sealed class SetVerboseModeRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetVerboseModeRequest"/> class.
        /// </summary>
        /// <param name="enabled">
        /// Set to <see langword="true"/> if verbose mode is enabled, <see langword="false"/> otherwise.
        /// </param>
        public SetVerboseModeRequest(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message, which is 0x09.</value>
        public override int ServiceId { get { return 0x09; } }

        /// <summary>
        /// Gets the status for the Verbose mode.
        /// </summary>
        /// <value>The status for the verbose mode.</value>
        public bool Enabled { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[set_verbose_mode] {0}", Enabled ? "on" : "off");
        }
    }
}
