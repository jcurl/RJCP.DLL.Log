namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using RJCP.Core;

    /// <summary>
    /// Encodes DLT Trace Lines with a storage header prepended.
    /// </summary>
    /// <remarks>
    /// This class creates trace lines encoded to a buffer that contains a standard header, and optionally, an extended
    /// header. The output format is DLT v1 with a storage header.
    /// </remarks>
    public class DltFileTraceEncoder : DltTraceEncoder
    {
        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>
        /// The number of bytes written to the buffer. If the data couldn't be encoded and there is an error, -1 is
        /// returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="null"/>.</exception>
        /// <remarks>This encoder takes a trace line or a control line and always writes out a verbose line.</remarks>
        public override Result<int> Encode(Span<byte> buffer, DltTraceLineBase line)
        {
            Result<int> result = WriteStorageHeader(buffer, line);
            if (!result.TryGet(out int storageLength)) return result;

            result = base.Encode(buffer[storageLength..], line);
            if (!result.TryGet(out int length)) return result;
            return storageLength + length;
        }

        /// <summary>
        /// Writes the storage header for DLTv1.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="line">The line containing the ECU ID and the time stamp.</param>
        /// <returns>The amount of data written.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Applications can use this to write a DLTv1 header, if they already have a DLTv1 packet with the payload
        /// already encoded.
        /// </remarks>
        public static Result<int> WriteStorageHeader(Span<byte> buffer, DltTraceLineBase line)
        {
            ArgumentNullException.ThrowIfNull(line);
            if (buffer.Length < 16)
                return Result.FromException<int>(new DltEncodeException("Insufficient buffer encoding line"));

            buffer[0] = 0x44;
            buffer[1] = 0x4C;
            buffer[2] = 0x54;
            buffer[3] = 0x01;

            if (line.Features.TimeStamp) {
                long seconds = new DateTimeOffset(line.TimeStamp).ToUnixTimeSeconds();
                int fractionalSecondTicks = (int)(line.TimeStamp.Ticks % TimeSpan.TicksPerSecond);
                int microseconds = fractionalSecondTicks / ((int)TimeSpan.TicksPerMillisecond / 1000);
                BitOperations.Copy32ShiftLittleEndian(seconds, buffer[4..8]);
                BitOperations.Copy32ShiftLittleEndian(microseconds, buffer[8..12]);
            } else {
                BitOperations.Copy64ShiftLittleEndian(0, buffer[4..12]);
            }
            WriteId(buffer[12..16], line.EcuId);
            return 16;
        }
    }
}
