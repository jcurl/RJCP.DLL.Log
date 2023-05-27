namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="GetMessageFilteringStatusResponse"/>.
    /// </summary>
    public sealed class GetMessageFilteringStatusResponseDecoder : IControlArgDecoder
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
                return Result.FromException<int>(new DltDecodeException($"'GetMessageFilteringStatusResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            if (status == ControlResponse.StatusError ||
                status == ControlResponse.StatusNotSupported) {
                service = new ControlErrorNotSupported(serviceId, status, "get_message_filtering");
                return 5;
            }

            if (buffer.Length < 6) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetMessageFilteringStatusResponse' with insufficient buffer length of {buffer.Length}"));
            }

            bool enabled = buffer[5] != 0;
            service = new GetMessageFilteringStatusResponse(status, enabled);
            return 6;
        }
    }
}
