﻿namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Diagnostics;
    using ControlArgs;

    /// <summary>
    /// Decodes the contents of the buffer to return a <see cref="SetMessageFilteringRequest"/>.
    /// </summary>
    public class SetMessageFilteringRequestDecoder : IControlArgDecoder
    {
        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 5) {
                service = null;
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Control message 'SetMessageFilteringRequest' with insufficient buffer length of {0} (needed 5)",
                    buffer.Length);
                return -1;
            }

            int enabled = buffer[4];
            service = new SetMessageFilteringRequest(enabled != 0);
            return 5;
        }
    }
}
