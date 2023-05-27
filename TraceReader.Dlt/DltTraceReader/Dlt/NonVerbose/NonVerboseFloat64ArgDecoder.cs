namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes a `S_FLOA64` argument type.
    /// </summary>
    public sealed class NonVerboseFloat64ArgDecoder : INonVerboseArgDecoder
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
            if (buffer.Length < pdu.PduLength) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"Insufficient payload buffer {pdu.PduLength} for float64 argument"));
            }
            if (pdu.PduLength < 8) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException("S_FLOA64 invalid length in PDU"));
            }

            double data = BitOperations.To64FloatShift(buffer, !msbf);
            arg = new Float64DltArg(data);
            return pdu.PduLength;
        }
    }
}
