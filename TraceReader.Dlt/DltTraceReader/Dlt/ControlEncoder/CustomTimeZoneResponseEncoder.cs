namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="CustomTimeZoneResponse"/>.
    /// </summary>
    public sealed class CustomTimeZoneResponseEncoder : ControlArgResponseEncoder
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier and the status.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected override int EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            if (buffer.Length < 5) return -1;

            CustomTimeZoneResponse controlArg = (CustomTimeZoneResponse)arg;
            int seconds = unchecked((int)(controlArg.TimeZone.Ticks / TimeSpan.TicksPerSecond));
            BitOperations.Copy32Shift(seconds, buffer[0..4], !msbf);
            buffer[4] = controlArg.IsDst ? (byte)1 : (byte)0;
            return 5;
        }
    }
}
