namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="ResetFactoryDefaultResponse"/>.
    /// </summary>
    public sealed class ResetFactoryDefaultResponseDecoder : IControlArgDecoder
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
        /// <returns>The number of bytes decoded.</returns>
        public Result<int> Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 5) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'ResetFactoryDefaultResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            if (status is ControlResponse.StatusError or
                ControlResponse.StatusNotSupported) {
                service = new ControlErrorNotSupported(serviceId, status, "reset_to_factory_default");
                return 5;
            }

            service = new ResetFactoryDefaultResponse(status);
            return 5;
        }
    }
}
