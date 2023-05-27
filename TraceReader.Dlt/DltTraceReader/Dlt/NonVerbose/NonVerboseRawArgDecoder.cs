namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes a `S_RAW` and `S_RAWD` argument type.
    /// </summary>
    public sealed class NonVerboseRawArgDecoder : INonVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT non verbose argument.
        /// </summary>
        /// <param name="buffer">The buffer where the encoded DLT non verbose argument can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="pdu">The Packet Data Unit instance representing the argument structure.</param>
        /// <param name="arg">On output, the decoded argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public Result<int> Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (buffer.Length < 2) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException("Insufficient payload buffer for raw argument length"));
            }

            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(buffer, !msbf));
            byte[] data;
            if (payloadLength == 0) {
                data = Array.Empty<byte>();
            } else {
                if (buffer.Length < 2 + payloadLength) {
                    arg = null;
                    return Result.FromException<int>(new DltDecodeException($"Insufficient payload buffer for raw argument length {payloadLength}"));
                }

                data = buffer[2..(2 + payloadLength)].ToArray();
            }
            arg = new RawDltArg(data);
            return 2 + payloadLength;
        }
    }
}
