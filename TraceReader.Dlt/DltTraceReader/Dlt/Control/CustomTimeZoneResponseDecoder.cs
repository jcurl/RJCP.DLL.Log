namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="CustomTimeZoneResponse"/>.
    /// </summary>
    public class CustomTimeZoneResponseDecoder : IControlArgDecoder
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
            int status = buffer[4];

            // TODO: Depends on the endianness.
            int timeZone = BitOperations.To32ShiftLittleEndian(buffer[5..9]);
            int isDst = buffer[9];
            service = new CustomTimeZoneResponse(status, timeZone, isDst != 0);
            return 10;
        }
    }
}
