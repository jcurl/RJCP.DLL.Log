namespace RJCP.Diagnostics.Log.Text
{
    using System;

    internal static class Iso8859_1
    {
        private static readonly char[] Iso8859_1Map = new char[256] {
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
            ' ', '¡', '¢', '£', '¤', '¥', '¦', '§', '¨', '©', 'ª', '«', '¬', '­', '®', '¯',    /* A0-AF; AD=Soft Hyphen */
            '°', '±', '²', '³', '´', 'µ', '¶', '·', '¸', '¹', 'º', '»', '¼', '½', '¾', '¿',   /* B0-BF */
            'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï',   /* C0-CF */
            'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß',   /* D0-DF */
            'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï',   /* E0-EF */
            'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', '÷', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ',   /* F0-FF */
        };

        /// <summary>
        /// Converts the specified bytes into characters.
        /// </summary>
        /// <param name="bytes">The buffer to convert from.</param>
        /// <param name="chars">The character buffer to put the result to.</param>
        /// <returns>The number of bytes converted.</returns>
        public static int Convert(ReadOnlySpan<byte> bytes, char[] chars)
        {
            int cu = bytes.Length < chars.Length ? bytes.Length : chars.Length;
            for (int i = 0; i < cu; i++) {
                chars[i] = Iso8859_1Map[bytes[i]];
            }
            return cu;
        }

        /// <summary>
        /// Converts the specified value into the bytes.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        /// <param name="bytes">The bytes to convert to.</param>
        /// <returns>The number of bytes written.</returns>
        public static int Convert(string value, Span<byte> bytes)
        {
            if (value is null || bytes.Length == 0) return 0;

            int bu = 0;
            foreach (char c in value) {
                if (c == 0) return bu;
                if (c >= 1 && c <= 255) {
                    // ISO-8859-1 and the first 256 bytes of Unicode match exactly.
                    bytes[bu] = unchecked((byte)c);
                    bu++;
                }
                if (bu == bytes.Length) return bu;
            }
            return bu;
        }
    }
}
