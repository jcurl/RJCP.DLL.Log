namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;
    using Text;

    /// <summary>
    /// Decodes the contents of the buffer to return a <see cref="GetSoftwareVersionResponse"/>.
    /// </summary>
    public sealed class GetSoftwareVersionResponseDecoder : IControlArgDecoder
    {
        private readonly char[] m_CharResult = new char[ushort.MaxValue];
        private const int VersionHeaderLength = 5;

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
                return Result.FromException<int>(new DltDecodeException($"'GetSoftwareVersionResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            if (status == ControlResponse.StatusError ||
                status == ControlResponse.StatusNotSupported) {
                service = new ControlErrorNotSupported(serviceId, status, "get_software_version");
                return 5;
            }

            const int VersionStringOffset = 4 + VersionHeaderLength;

            if (buffer.Length < VersionStringOffset) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetSoftwareVersionResponse' with insufficient buffer length of {buffer.Length}"));
            }

            uint payloadLength = unchecked((uint)BitOperations.To32Shift(buffer[5..9], !msbf));
            if (payloadLength > ushort.MaxValue) {
                service = null;
                return -1;
            }

            if (buffer.Length < VersionStringOffset + payloadLength) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetSoftwareVersionResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int versionLength = (int)payloadLength;
            if (buffer[VersionStringOffset + versionLength - 1] == '\0')
                versionLength--;

            int cu = Iso8859_1.Convert(buffer[VersionStringOffset..(VersionStringOffset + versionLength)], m_CharResult);
            string swVersion = new string(m_CharResult, 0, cu);

            service = new GetSoftwareVersionResponse(status, swVersion);
            return VersionStringOffset + (int)payloadLength;
        }
    }
}
