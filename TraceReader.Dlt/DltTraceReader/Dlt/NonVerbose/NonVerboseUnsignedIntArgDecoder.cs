namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes a `S_UINT*` argument types.
    /// </summary>
    public sealed class NonVerboseUnsignedIntArgDecoder : INonVerboseArgDecoder
    {
        private readonly int m_IntLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseUnsignedIntArgDecoder" /> class.
        /// </summary>
        /// <param name="length">The length of the integer.</param>
        /// <exception cref="InvalidOperationException">Unexpected data length.</exception>
        public NonVerboseUnsignedIntArgDecoder(int length)
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
        /// <exception cref="InvalidOperationException">Unexpected data length.</exception>
        public Result<int> Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (buffer.Length < pdu.PduLength) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"Insufficient payload buffer {pdu.PduLength} for unsigned int argument"));
            }
            if (pdu.PduLength < m_IntLength) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"S_UINT{m_IntLength * 8} invalid length in PDU"));
            }

            if (m_IntLength == 1) {
                arg = new UnsignedIntDltArg(buffer[0], 1);
            } else if (m_IntLength == 2) {
                arg = new UnsignedIntDltArg(unchecked((ushort)BitOperations.To16Shift(buffer, !msbf)), 2);
            } else if (m_IntLength == 4) {
                arg = new UnsignedIntDltArg(unchecked((uint)BitOperations.To32Shift(buffer, !msbf)), 4);
            } else if (m_IntLength == 8) {
                arg = new UnsignedIntDltArg(unchecked((ulong)BitOperations.To64Shift(buffer, !msbf)), 8);
            } else {
                throw new InvalidOperationException("Unexpected data length");
            }
            return pdu.PduLength;
        }
    }
}
