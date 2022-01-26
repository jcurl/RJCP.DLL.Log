namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="SwInjectionRequest"/>.
    /// </summary>
    public class SwInjectionRequestDecoder : ControlArgDecoderBase
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
        public override int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 8)
                return DecodeError(serviceId, DltType.CONTROL_REQUEST,
                    "'SwInjectionRequest' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            int length = BitOperations.To32Shift(buffer[4..8], !msbf);
            if (buffer.Length != 8 + length)
                return DecodeError(serviceId, DltType.CONTROL_REQUEST,
                    "'SwInjectionRequest' with incorrect buffer length of {0}", buffer.Length,
                    out service);
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
