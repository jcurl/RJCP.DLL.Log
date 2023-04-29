﻿namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes a `S_SINT*` argument types.
    /// </summary>
    public sealed class NonVerboseSignedIntArgDecoder : INonVerboseArgDecoder
    {
        private readonly int m_IntLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseSignedIntArgDecoder" /> class.
        /// </summary>
        /// <param name="length">The length of the integer.</param>
        public NonVerboseSignedIntArgDecoder(int length)
        {
            switch (length) {
            case 1:
            case 2:
            case 4:
            case 8:
                break;
            default:
                throw new InvalidOperationException("Unexpected data length");
            }
            m_IntLength = length;
        }

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
        /// <exception cref="System.InvalidOperationException">Unexpected data length</exception>
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (buffer.Length < pdu.PduLength)
                return DltArgError.Get($"Insufficient payload buffer {pdu.PduLength} for signed int argument", out arg);
            if (pdu.PduLength < m_IntLength)
                return DltArgError.Get($"S_SINT{m_IntLength * 8} invalid length in PDU", out arg);

            if (m_IntLength == 1) {
                arg = new SignedIntDltArg(unchecked((sbyte)buffer[0]));
            } else if (m_IntLength == 2) {
                arg = new SignedIntDltArg(BitOperations.To16Shift(buffer, !msbf));
            } else if (m_IntLength == 4) {
                arg = new SignedIntDltArg(BitOperations.To32Shift(buffer, !msbf));
            } else if (m_IntLength == 8) {
                arg = new SignedIntDltArg(BitOperations.To64Shift(buffer, !msbf));
            } else {
                throw new InvalidOperationException("Unexpected data length");
            }
            return pdu.PduLength;
        }
    }
}