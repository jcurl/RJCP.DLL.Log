namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Text;
    using Args;
    using RJCP.Core;
    using Text;

    /// <summary>
    /// Decodes `S_STRG_ASCII` or `S_STRG_UTF8` argument types.
    /// </summary>
    public sealed class NonVerboseStringArgDecoder : INonVerboseArgDecoder
    {
        private readonly StringEncodingType m_Encoding;
        private readonly Decoder m_Utf8Decoder = Encoding.UTF8.GetDecoder();
        private readonly char[] m_CharResult = new char[ushort.MaxValue];

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseStringArgDecoder"/> class.
        /// </summary>
        /// <param name="encoding">The encoding used in the buffer payload.</param>
        public NonVerboseStringArgDecoder(StringEncodingType encoding)
        {
            m_Encoding = encoding;
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
        public Result<int> Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (buffer.Length < 2) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException("Insufficient payload buffer for string argument length"));
            }

            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(buffer, !msbf));
            if (buffer.Length < 2 + payloadLength) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"Insufficient payload buffer for string argument length {payloadLength}"));
            }

            int strLength = payloadLength;
            if (strLength > 0 && buffer[2 + payloadLength - 1] == '\0')
                strLength--;

            string data;
            if (payloadLength == 0) {
                data = string.Empty;
            } else {
                int cu;
                switch (m_Encoding) {
                case StringEncodingType.Ascii:
                    cu = Iso8859_15.Convert(buffer[2..(2 + strLength)], m_CharResult);
                    break;
                default:
                    m_Utf8Decoder.Convert(buffer[2..(2 + strLength)], m_CharResult.AsSpan(), true, out _, out cu, out _);
                    break;
                }
                data = new string(m_CharResult, 0, cu);
            }
            arg = new StringDltArg(data, m_Encoding);
            return 2 + payloadLength;
        }
    }
}
