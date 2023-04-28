namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes a `S_FLOA32` argument type.
    /// </summary>
    public sealed class NonVerboseFloat32ArgDecoder : INonVerboseArgDecoder
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
            if (buffer.Length < pdu.PduLength)
                return DltArgError.Get($"Insufficient payload buffer {pdu.PduLength} for float32 argument", out arg);
            if (pdu.PduLength < 4)
                return DltArgError.Get("S_FLOA32 invalid length in PDU", out arg);

            float data = BitOperations.To32FloatShift(buffer, !msbf);
            arg = new Float32DltArg(data);
            return pdu.PduLength;
        }
    }
}
