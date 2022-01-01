namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;
    using Text;

    /// <summary>
    /// Decodes the contents of the buffer to return a <see cref="GetSoftwareVersionResponse"/>.
    /// </summary>
    public class GetSoftwareVersionResponseDecoder : IControlArgDecoder
    {
        private readonly char[] m_CharResult = new char[ushort.MaxValue];
        private const int VersionHeaderLength = 5;

        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, out IControlArg service)
        {
            uint payloadLength = unchecked((uint)BitOperations.To32ShiftLittleEndian(buffer[5..9]));
            if (payloadLength > ushort.MaxValue) {
                service = null;
                return -1;
            }

            int versionLength = (int)payloadLength;
            if (buffer[4 + VersionHeaderLength + versionLength - 1] == '\0')
                versionLength--;

            int status = buffer[4];
            int cu = Iso8859_1.Convert(buffer[9..(9 + versionLength)], m_CharResult);
            string swVersion = new string(m_CharResult, 0, cu);

            service = new GetSoftwareVersionResponse(status, swVersion);
            return 4 + VersionHeaderLength + (int)payloadLength;
        }
    }
}
