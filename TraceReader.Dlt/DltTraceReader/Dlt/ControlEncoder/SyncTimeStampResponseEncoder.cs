namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="SyncTimeStampResponse"/>.
    /// </summary>
    public sealed class SyncTimeStampResponseEncoder : ControlArgResponseEncoder
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
            if (arg.Status != ControlResponse.StatusOk) return 0;
            if (buffer.Length < 10) return -1;

            SyncTimeStampResponse controlArg = (SyncTimeStampResponse)arg;
            long time = controlArg.TimeStamp.ToUniversalTime().Ticks - DateTimeOffset.FromUnixTimeSeconds(0).Ticks;

            long ns = (time % TimeSpan.TicksPerSecond) * (1000000000 / TimeSpan.TicksPerSecond);
            BitOperations.Copy32Shift(ns, buffer[0..4], !msbf);

            time /= TimeSpan.TicksPerSecond;
            BitOperations.Copy32Shift((time & 0xFFFFFFFF), buffer[4..8], !msbf);
            BitOperations.Copy16Shift(time >> 32, buffer[8..10], !msbf);
            return 10;
        }
    }
}
