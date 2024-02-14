namespace RJCP.App.DltDump.Infrastructure.Uri
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Resources;

    /// <summary>
    /// Provide useful methods for parsing URI strings.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Parses a comma separated list of elements.
        /// </summary>
        /// <param name="connparams">The string containing the comma separated list of parameters.</param>
        /// <returns>A list of strings in the order of the comma separated list of parameters.</returns>
        public static IReadOnlyList<string> ParseCommaSeparatedList(string connparams)
        {
            List<string> list = new();

            if (connparams.Trim().Length == 0) return list;

            // Each parameter is separated by a comma
            char quotes = '\0';
            bool esc = false;
            bool quoted = false;
            StringBuilder token = new();

            for (int i = 0; i < connparams.Length; i++) {
                char c = connparams[i];
                if (esc) {
                    token.Append(c);
                    esc = false;
                } else {
                    if (c == '\\') {
                        // Escape the next character, regardless
                        esc = true;
                    } else if (c is '\'' or '\"') {
                        if (quotes == '\0') {
                            if (quoted)
                                throw new ArgumentException(AppResources.InfraUriError_QuotesMultiple, nameof(connparams));
                            if (!string.IsNullOrWhiteSpace(token.ToString()))
                                throw new ArgumentException(AppResources.InfraUriError_QuoteInMiddle, nameof(connparams));
                            // Start of quotes. End quote must match.
                            token.Clear();
                            quotes = c;
                            quoted = true;
                        } else if (c == quotes) {
                            // End of quote, including white space.
                            quotes = '\0';
                        } else {
                            throw new ArgumentException(AppResources.InfraUriError_QuotesMismatch, nameof(connparams));
                        }
                    } else if (c == ',' && quotes == '\0') {
                        if (!quoted) {
                            list.Add(token.ToString().Trim());
                        } else {
                            list.Add(token.ToString());
                        }
                        token.Clear();
                        quoted = false;
                    } else {
                        if (quotes == '\0' && quoted) {
                            if (c != ' ')
                                throw new ArgumentException(AppResources.InfraUriError_QuoteInMiddle, nameof(connparams));
                        } else {
                            token.Append(c);
                        }
                    }
                }
            }

            if (esc)
                throw new ArgumentException(AppResources.InfraUriError_EscapeCharacter, nameof(connparams));

            if (quotes != '\0')
                throw new ArgumentException(AppResources.InfraUriError_QuotesNotClosed, nameof(connparams));

            if (!quoted) {
                // If the last space was escaped, this is still trimmed. It must be quoted.
                list.Add(token.ToString().Trim());
            } else {
                list.Add(token.ToString());
            }

            return list;
        }

        /// <summary>
        /// Parses the query portion in the format '?key=value'.
        /// </summary>
        /// <param name="uri">The URI to parse the query section of.</param>
        /// <returns>An ordered list of key/value pairs after parsing.</returns>
        /// <exception cref="ArgumentException">The URI has an invalid character in the query.</exception>
        public static IReadOnlyList<KeyValuePair<string, string>> ParseQuery(this Uri uri)
        {
            string query = uri.Query;
            if (query.Length == 0) return Array.Empty<KeyValuePair<string, string>>();

            List<KeyValuePair<string, string>> kvps = new();
            int keyStart = 0;
            int valueStart = -1;

            for (int i = 0; i < query.Length; i++) {
                char c = query[i];
                if (c == '&') {
                    if (i == keyStart) {
                        keyStart++;
                        continue;
                    }

                    kvps.Add(GetPair(query, keyStart, valueStart, i));
                    keyStart = i + 1;
                } else if (c == '?') {
                    if (i == 0) keyStart = 1;
                } else if (c == '=') {
                    if (valueStart < keyStart) valueStart = i;
                }
            }

            if (keyStart < query.Length) {
                kvps.Add(GetPair(query, keyStart, valueStart, query.Length));
            }
            return kvps;
        }

        private static readonly KeyValuePair<string, string> EmptyKeyValue =
            new(string.Empty, string.Empty);

        private static KeyValuePair<string, string> GetPair(string query, int keyStart, int valuePos, int endPos)
        {
            if (keyStart >= query.Length) return EmptyKeyValue;

            string key;
            string value;
            if (valuePos < keyStart) {
                value = string.Empty;
                key = Uri.UnescapeDataString(query[keyStart..endPos]);
            } else {
                key = Uri.UnescapeDataString(query[keyStart..valuePos]);
                value = valuePos == endPos ?
                    string.Empty :
                    Uri.UnescapeDataString(query[(valuePos + 1)..endPos]);
            }

            if (key == String.Empty && value == string.Empty)
                return EmptyKeyValue;

            return new KeyValuePair<string, string>(key, value);
        }
    }
}
