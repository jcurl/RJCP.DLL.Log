namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Diagnostics;
    using ControlArgs;

    /// <summary>
    /// Decoder for the payload with <see cref="GetUseTimeStampResponse"/>.
    /// </summary>
    public class GetUseTimeStampResponseDecoder : IControlArgDecoder
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
            if (buffer.Length < 6) {
                service = null;
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Control message 'GetUseTimeStampResponse' with insufficient buffer length of {0} (needed 6)",
                    buffer.Length);
                return -1;
            }

            int status = buffer[4];
            bool enabled = buffer[5] != 0;
            service = new GetUseTimeStampResponse(status, enabled);
            return 6;
        }
    }
}
