namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Decoder for the payload with <see cref="MessageBufferOverflowResponse"/>.
    /// </summary>
    public class MessageBufferOverflowResponseDecoder : ControlArgDecoderBase
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
            if (buffer.Length < 5)
                return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                    "'MessageBufferOverflowResponse' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            int status = buffer[4];
            if (status == ControlResponse.StatusError ||
                status == ControlResponse.StatusNotSupported) {
                service = new ControlErrorNotSupported(serviceId, status, "message_buffer_overflow");
                return 5;
            }

            if (buffer.Length < 6)
                return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                    "'MessageBufferOverflowResponse' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            bool overflow = buffer[5] != 0;
            service = new MessageBufferOverflowResponse(status, overflow);
            return 6;
        }
    }
}
