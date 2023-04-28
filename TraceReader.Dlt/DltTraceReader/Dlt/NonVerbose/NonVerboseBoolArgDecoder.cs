﻿namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;

    /// <summary>
    /// Decodes a `S_BOOL` argument type.
    /// </summary>
    public sealed class NonVerboseBoolArgDecoder : INonVerboseArgDecoder
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
                return DltArgError.Get($"Insufficient payload buffer {pdu.PduLength} for bool argument", out arg);
            if (pdu.PduLength < 1)
                return DltArgError.Get("S_BOOL invalid length in PDU", out arg);

            bool boolArg = false;
            for (int i = 0; i < pdu.PduLength && !boolArg; i++) {
                boolArg = buffer[i] != 0x00;
            }
            arg = new BoolDltArg(boolArg);
            return pdu.PduLength;
        }
    }
}
