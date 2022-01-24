namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Diagnostics;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="BufferOverflowNotificationResponse"/>.
    /// </summary>
    public class BufferOverflowNotificationResponseDecoder : IControlArgDecoder
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
            if (buffer.Length < 9) {
                service = null;
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Control message 'BufferOverflowNotificationResponse' with insufficient buffer length of {0} (needed 9)",
                    buffer.Length);
                return -1;
            }

            int status = buffer[4];
            int counter = BitOperations.To32Shift(buffer[5..9], !msbf);

            service = new BufferOverflowNotificationResponse(status, counter);
            return 9;
        }
    }
}
