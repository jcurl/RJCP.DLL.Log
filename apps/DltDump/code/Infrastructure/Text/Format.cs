namespace RJCP.App.DltDump.Infrastructure.Text
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Resources;

    /// <summary>
    /// Formats strings for console output.
    /// </summary>
    public static class Format
    {
        private enum TokenType
        {
            None,
            Word,
            Space,
            Newline
        }

        private struct Token
        {
            public TokenType Type;
            public string Word;
        }

        /// <summary>
        /// Wraps the message to the specified width.
        /// </summary>
        /// <param name="width">The width to wrap at.</param>
        /// <param name="message">The message that should be wrapped.</param>
        /// <returns>An array of strings having wrapped the original <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> should be at least 10 characters including indent.
        /// </exception>
        /// <exception cref="InvalidOperationException">Error parsing string.</exception>
        /// <remarks>
        /// Takes the input string <paramref name="message"/> and wraps it so it doesn't exceed
        /// <paramref name="width"/>. The message is left indented depending on the number of spaces at the beginning of
        /// the message.
        /// <para>
        /// This method helps to format resource strings to print to the console. It is not a general formatter for
        /// large blocks of text.
        /// </para>
        /// </remarks>
        public static string[] Wrap(int width, string message)
        {
            return Wrap(width, 0, 0, message);
        }

        /// <summary>
        /// Wraps the message to the specified width.
        /// </summary>
        /// <param name="width">The width to wrap at.</param>
        /// <param name="indent">The indent for all lines.</param>
        /// <param name="message">The message that should be wrapped.</param>
        /// <returns>System.String[].</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> should be at least 10 characters including indent.
        /// <para>- or -</para>
        /// <paramref name="indent"/> must be zero or more characters.
        /// </exception>
        /// <exception cref="InvalidOperationException">Error parsing string.</exception>
        /// <remarks>
        /// Takes the input string <paramref name="message"/> and wraps it so it doesn't exceed
        /// <paramref name="width"/>. The message is left indented depending on the number of spaces at the beginning of
        /// the message.
        /// <para>
        /// This method helps to format resource strings to print to the console. It is not a general formatter for
        /// large blocks of text.
        /// </para>
        /// </remarks>
        public static string[] Wrap(int width, int indent, string message)
        {
            return Wrap(width, indent, indent, message);
        }

        /// <summary>
        /// Wraps the message to the specified width.
        /// </summary>
        /// <param name="width">The width to wrap at.</param>
        /// <param name="indent">The indent for the first line.</param>
        /// <param name="hangingIndent">The hanging indent for the second line and later.</param>
        /// <param name="message">The message that should be wrapped.</param>
        /// <returns>System.String[].</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> should be at least 10 characters including indent.
        /// <para>- or -</para>
        /// <paramref name="indent"/> must be zero or more characters.
        /// <para>- or -</para>
        /// <paramref name="hangingIndent"/> must be zero or more.
        /// </exception>
        /// <exception cref="InvalidOperationException">Error parsing string.</exception>
        /// <remarks>
        /// Takes the input string <paramref name="message"/> and wraps it so it doesn't exceed
        /// <paramref name="width"/>. The message is left indented depending on the number of spaces at the beginning of
        /// the message.
        /// <para>
        /// This method helps to format resource strings to print to the console. It is not a general formatter for
        /// large blocks of text.
        /// </para>
        /// </remarks>
        public static string[] Wrap(int width, int indent, int hangingIndent, string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (width < indent + 10)
                throw new ArgumentOutOfRangeException(nameof(width), AppResources.InfraFormatWidthException);
            if (indent < 0)
                throw new ArgumentOutOfRangeException(nameof(indent), AppResources.InfraFormatIndentException);
            if (hangingIndent < 0)
                throw new ArgumentOutOfRangeException(nameof(hangingIndent), AppResources.InfraFormatHangingIndentException);

            StringBuilder line = new StringBuilder(width);
            List<string> output = new List<string>();

            int lineIndent = indent;
            bool newLine = true;
            int offset = 0;
            while (offset < message.Length) {
                Token token = GetWord(ref offset, message);
                switch (token.Type) {
                case TokenType.None:
                    break;
                case TokenType.Space:
                    // If the first token is a space, then we take this as a hanging indent.
                    lineIndent = hangingIndent;
                    break;
                case TokenType.Newline:
                    if (newLine) {
                        output.Add(string.Empty);
                    } else {
                        output.Add(line.ToString());
                        line.Clear();
                        newLine = true;
                    }
                    lineIndent = indent;
                    break;
                case TokenType.Word:
                    if (newLine) {
                        line.Append(' ', lineIndent).Append(token.Word);
                        lineIndent = hangingIndent;
                    } else {
                        if (line.Length + token.Word.Length + 1 > width) {
                            output.Add(line.ToString());
                            line.Clear();
                            line.Append(' ', lineIndent).Append(token.Word);
                        } else {
                            line.Append(' ').Append(token.Word);
                        }
                    }
                    newLine = false;
                    break;
                default:
                    throw new InvalidOperationException(AppResources.InfraFormatMessageParseException);
                }
            }

            if (line.Length > 0) output.Add(line.ToString());
            return output.ToArray();
        }

        /// <summary>
        /// Scans the string and counts the number of initial spaces for the line.
        /// </summary>
        /// <param name="message">The message that should be wrapped.</param>
        /// <returns>The number of bytes for the first indent.</returns>
        public static int GetIndent(string message)
        {
            for (int i = 0; i < message.Length; i++) {
                if (message[i] != ' ') return i;
            }
            return 0;
        }

        private static Token GetWord(ref int offset, string message)
        {
            Token token = new Token() {
                Type = TokenType.None,
                Word = null
            };

            for (int i = offset; i < message.Length; i++) {
                char c = message[i];

                switch (token.Type) {
                case TokenType.None:
                    if (c == '\n') {
                        offset = i + 1;
                        token.Type = TokenType.Newline;
                        return token;
                    } else if (c == '\r') {
                        token.Type = TokenType.Newline;
                    } else if (char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSeparator(c)) {
                        token.Type = TokenType.Space;
                    } else {
                        token.Type = TokenType.Word;
                    }
                    break;
                case TokenType.Newline:
                    if (c == '\n') {
                        offset = i + 1;
                        return token;
                    }
                    offset = i;
                    return token;
                case TokenType.Space:
                    if (!char.IsWhiteSpace(c) && !char.IsControl(c) && !char.IsSeparator(c)) {
                        offset = i;
                        return token;
                    }
                    break;
                case TokenType.Word:
                    if (char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSeparator(c) || c == '\r' || c == '\n') {
                        token.Word = message[offset..i];
                        offset = i;
                        return token;
                    }
                    break;
                default:
                    throw new InvalidOperationException(AppResources.InfraFormatMessageParseException);
                }
            }

            switch (token.Type) {
            case TokenType.None:
            case TokenType.Newline:
            case TokenType.Space:
                offset = message.Length;
                return token;
            case TokenType.Word:
                token.Word = message[offset..];
                offset = message.Length;
                return token;
            default:
                throw new InvalidOperationException(AppResources.InfraFormatMessageParseException);
            }
        }
    }
}
