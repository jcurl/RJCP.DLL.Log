namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Diagnostics;
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
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 8) {
                service = null;
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Control message 'SwInjectionRequest' id 0x{0:x} with insufficient buffer length of {1} (minimum 8)",
                    serviceId, buffer.Length);
                return -1;
            }

            int length = BitOperations.To32Shift(buffer[4..8], !msbf);
            if (length != buffer.Length - 8) {
                service = null;
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Control message 'SwInjectionRequest' id 0x{0:x} with insufficient buffer length of {1} (needed {2})",
                    serviceId, buffer.Length, length + 8);
                return -1;
            }
            return 8 + Decode(serviceId, length, buffer[8..], msbf, out service);
        }

        /// <summary>
        /// Decodes the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="length">The length as provided in the original packet.</param>
        /// <param name="buffer">The buffer after skipping over the length field.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message that is created on exit of this function.</param>
        /// <returns>The number of bytes decoded.</returns>
        protected virtual int Decode(int serviceId, int length, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            service = new SwInjectionRequest(serviceId, buffer.ToArray());
            return buffer.Length;
        }
    }
}
