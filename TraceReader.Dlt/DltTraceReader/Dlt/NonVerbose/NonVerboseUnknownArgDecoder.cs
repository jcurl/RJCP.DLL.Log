namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decode unknown PDU types as a fixed size.
    /// </summary>
    public sealed class NonVerboseUnknownArgDecoder : INonVerboseArgDecoder
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
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            byte headerLength = 0;
            ushort payloadLength;

            if (pdu.PduLength > 0 && buffer.Length < pdu.PduLength)
                return DltArgError.Get($"Insufficient payload buffer {pdu.PduLength} for unknown argument", out arg);
            if (pdu.PduLength == 0) {
                if (buffer.Length < 2)
                    return DltArgError.Get("Insufficient payload buffer for unknown argument length", out arg);
                payloadLength = unchecked((ushort)BitOperations.To16Shift(buffer, !msbf));
                if (buffer.Length < 2 + payloadLength)
                    return DltArgError.Get($"Insufficient payload buffer {buffer.Length} for unknown argument length {payloadLength}", out arg);
                headerLength = 2;
            } else {
                if (pdu.PduLength > ushort.MaxValue)
                    return DltArgError.Get($"PDU Payload length exceeds 16-bit for unknown argument length {pdu.PduLength}", out arg);
                payloadLength = unchecked((ushort)pdu.PduLength);
            }

            arg = new UnknownNonVerboseDltArg(buffer[headerLength..(payloadLength + headerLength)]);
            return payloadLength + headerLength;
        }
    }
}
