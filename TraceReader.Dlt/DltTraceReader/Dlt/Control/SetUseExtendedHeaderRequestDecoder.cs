namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Decodes the contents of the buffer to return a <see cref="SetUseExtendedHeaderRequest"/>.
    /// </summary>
    public sealed class SetUseExtendedHeaderRequestDecoder : ControlArgDecoderBase
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
                return DecodeError(serviceId, DltType.CONTROL_REQUEST,
                    "'SetUseExtendedHeaderRequest' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            int enabled = buffer[4];
            service = new SetUseExtendedHeaderRequest(enabled != 0);
            return 5;
        }
    }
}
