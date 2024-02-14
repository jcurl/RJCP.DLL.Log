namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="SyncTimeStampResponse"/>.
    /// </summary>
    public sealed class SyncTimeStampResponseDecoder : IControlArgDecoder
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
                return Result.FromException<int>(new DltDecodeException($"'SyncTimeStampResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            if (status is ControlResponse.StatusError or
                ControlResponse.StatusNotSupported) {
                service = new ControlErrorNotSupported(serviceId, status, "sync_timestamp");
                return 5;
            }

            if (buffer.Length < 15) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'SyncTimeStampResponse' with insufficient buffer length of {buffer.Length}"));
            }

            uint ns = unchecked((uint)BitOperations.To32Shift(buffer[5..9], !msbf));
            uint secLow = unchecked((uint)BitOperations.To32Shift(buffer[9..13], !msbf));
            uint secHigh = unchecked((ushort)BitOperations.To16Shift(buffer[13..15], !msbf));

            long sec = ((long)secHigh << 32) | secLow;  // Number of seconds since 1/1/1970
            long nsTicks = ns / 100;         // TicksPerMillisecond = 10000. i.e. 100ns per tick.
                                             // Manual calculation to avoid int overflow.
            DateTimeOffset timeStamp = DateTimeOffset.FromUnixTimeSeconds(sec).AddTicks(nsTicks);
            service = new SyncTimeStampResponse(status, new DateTime(timeStamp.Ticks, DateTimeKind.Utc));
            return 15;
        }
    }
}
