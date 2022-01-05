namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="SwInjectionRequest"/>.
    /// </summary>
    /// <seealso cref="RJCP.Diagnostics.Log.Dlt.Control.IControlArgDecoder" />
    public class SwInjectionRequestDecoder : IControlArgDecoder
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
            int length = BitOperations.To32ShiftLittleEndian(buffer[4..8]);
            if (length != buffer.Length - 8) {
                service = null;
                return -1;
            }
            return 8 + Decode(serviceId, length, buffer[8..], out service);
        }

        /// <summary>
        /// Decodes the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="length">The length as provided in the original packet.</param>
        /// <param name="buffer">The buffer after skipping over the length field.</param>
        /// <param name="service">The control message that is created on exit of this function.</param>
        /// <returns>The number of bytes decoded.</returns>
        protected virtual int Decode(int serviceId, int length, ReadOnlySpan<byte> buffer, out IControlArg service)
        {
            service = new SwInjectionRequest(serviceId, buffer.ToArray());
            return buffer.Length;
        }
    }
}
