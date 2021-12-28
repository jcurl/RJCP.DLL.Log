namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decode a verbose payload that contains a string.
    /// </summary>
    public class StringArgDecoder : IVerboseArgDecoder
    {
        private readonly Decoder m_Utf8Decoder = Encoding.UTF8.GetDecoder();
        private readonly Decoder m_ArgumentDecoder = Encoding.GetEncoding("ISO-8859-15").GetDecoder();
        private readonly char[] m_CharResult = new char[ushort.MaxValue];

        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian. The length of the string is expected to be coding with this endianness.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            if (TypeInfo.IsVariSet(buffer)) {
                arg = null;
                return -1;
            }

            StringEncodingType coding = (StringEncodingType)TypeInfo.GetCoding(buffer);
            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(buffer[4..6], !msbf));
            int strLength = payloadLength;
            if (strLength > 0 && buffer[DltConstants.TypeInfo.TypeInfoSize + 2 + strLength - 1] == '\0')
                strLength--;

            string data;
            if (strLength == 0) {
                data = string.Empty;
            } else {
                int cu;
                switch (coding) {
                case StringEncodingType.Ascii:
                    m_ArgumentDecoder.Convert(buffer[6..(6 + strLength)], m_CharResult.AsSpan(), true, out _, out cu, out _);
                    break;
                default:
                    m_Utf8Decoder.Convert(buffer[6..(6 + strLength)], m_CharResult.AsSpan(), true, out _, out cu, out _);
                    break;
                }
                data = new string(m_CharResult, 0, cu);
            }
            arg = new StringDltArg(data, coding);
            return DltConstants.TypeInfo.TypeInfoSize + 2 + payloadLength;
        }
    }
}
