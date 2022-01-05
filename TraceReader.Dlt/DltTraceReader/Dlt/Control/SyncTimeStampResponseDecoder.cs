namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="SyncTimeStampResponse"/>.
    /// </summary>
    public class SyncTimeStampResponseDecoder : IControlArgDecoder
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
            int status = buffer[4];
            uint ns = unchecked((uint)BitOperations.To32Shift(buffer[5..9], !msbf));
            uint secLow = unchecked((uint)BitOperations.To32Shift(buffer[9..13], !msbf));
            uint secHigh = unchecked((ushort)BitOperations.To16Shift(buffer[13..15], !msbf));

            long sec = (secHigh << 32) | secLow;  // Number of seconds since 1/1/1970
            long nsTicks = ns / 100;         // TicksPerMillisecond = 10000. i.e. 100ns per tick.
                                             // Manual calculation to avoid int overflow.
            DateTimeOffset timeStamp = DateTimeOffset.FromUnixTimeSeconds(sec).AddTicks(nsTicks);
            service = new SyncTimeStampResponse(status, new DateTime(timeStamp.Ticks, DateTimeKind.Utc));
            return 15;
        }
    }
}
