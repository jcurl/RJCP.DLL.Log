namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Decoder for the payload with <see cref="BufferOverflowNotificationRequest"/>.
    /// </summary>
    public class BufferOverflowNotificationRequestDecoder : IControlArgDecoder
    {
        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, out IControlArg service)
        {
            service = new BufferOverflowNotificationRequest();
            return 4;
        }
    }
}
