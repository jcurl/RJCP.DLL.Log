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
                    cu = Iso8859_15(buffer[6..(6 + strLength)], m_CharResult);
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

        private static readonly char[] Iso8859_15Map = new char[256] {
            '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',   /* 00-0F */
            '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',   /* 10-1F */
            ' ', '!', '\"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', /* 20-2F */
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',   /* 30-3F */
            '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',   /* 40-4F */
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',  /* 50-5F */
            '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',   /* 60-6F */
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '.',   /* 70-7F */
            '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',   /* 80-8F */
            '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',   /* 90-9F */
            ' ', '¡', '¢', '£', '€', '¥', 'Š', '§', 'š', '©', 'ª', '«', '¬', '­', '®', '¯',    /* A0-AF; AD=Soft Hyphen */
            '°', '±', '²', '³', 'Ž', 'µ', '¶', '·', 'ž', '¹', 'º', '»', 'Œ', 'œ', 'Ÿ', '¿',   /* B0-BF */
            'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï',   /* C0-CF */
            'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß',   /* D0-DF */
            'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï',   /* E0-EF */
            'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', '÷', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ',   /* F0-FF */
        };

        private static int Iso8859_15(ReadOnlySpan<byte> bytes, char[] chars)
        {
            int cu = bytes.Length < chars.Length ? bytes.Length : chars.Length;
            for (int i = 0; i < cu; i++) {
                chars[i] = Iso8859_15Map[bytes[i]];
            }
            return cu;
        }
    }
}
