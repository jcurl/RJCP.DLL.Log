﻿namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="CustomTimeZoneResponse"/>.
    /// </summary>
    public sealed class CustomTimeZoneResponseDecoder : IControlArgDecoder
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
            if (buffer.Length < 10) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'CustomTimeZoneResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            int timeZone = BitOperations.To32Shift(buffer[5..9], !msbf);
            int isDst = buffer[9];
            service = new CustomTimeZoneResponse(status, timeZone, isDst != 0);
            return 10;
        }
    }
}
