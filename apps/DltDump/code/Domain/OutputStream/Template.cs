namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Resources;

    /// <summary>
    /// Manages an input file name template.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// The token type when parsing the template string.
        /// </summary>
        private enum TokenType
        {
            /// <summary>
            /// Default value, no token.
            /// </summary>
            None,

            /// <summary>
            /// A verbatim string.
            /// </summary>
            String,

            /// <summary>
            /// A variable that should be substituted.
            /// </summary>
            Variable
        }

        /// <summary>
        /// Token class containing the token type and the value for the token.
        /// </summary>
        private sealed class Token
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Token"/> class.
            /// </summary>
            /// <param name="tokenType">Type of the token.</param>
            /// <param name="value">The value for the given token.</param>
            public Token(TokenType tokenType, string value)
            {
                TokenType = tokenType;
                Value = value;
            }

            /// <summary>
            /// Gets the type of the token.
            /// </summary>
            /// <value>The type of the token.</value>
            public TokenType TokenType { get; }

            /// <summary>
            /// Gets the value of the token.
            /// </summary>
            /// <value>The value of the token.</value>
            public string Value { get; }
        }

        private readonly List<Token> m_TokenList = new List<Token>();
        private readonly HashSet<string> m_Found = new HashSet<string>();
        private string m_Template;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class for a template string.
        /// </summary>
        /// <param name="template">The template string that should be parsed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
        public Template(string template)
        {
            if (template is null) throw new ArgumentNullException(nameof(template));
            Parse(template);
            AllowConcatenation = !m_Found.Contains("FILE");
            SupportsSplit = m_Found.Contains("CTR") || m_Found.Contains("CDATE") ||
                m_Found.Contains("CTIME") || m_Found.Contains("CDATETIME");
        }

        /// <summary>
        /// Gets the variables that can override environment variables.
        /// </summary>
        /// <value>The variables that can override.</value>
        public IDictionary<string, string> Variables { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value indicating whether output concatenation of input files are allowed.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if file concatenation is allowed; otherwise, <see langword="false"/>.
        /// </value>
        public bool AllowConcatenation { get; }

        /// <summary>
        /// Gets a value indicating that the template supports splitting files.
        /// </summary>
        /// <value>Is <see langword="true"/> if split is supported; otherwise, <see langword="false"/>.</value>
        public bool SupportsSplit { get; }

        private void Parse(string template)
        {
            int start = -1;
            int end = -1;

            for (int i = 0; i < template.Length; i++) {
                if (template[i] != '%') continue;

                if (start < 0) {
                    // Start of an environment variable.
                    if (i > 0) {
                        // Copy the text up until the environment variable
                        string value;
                        if (end < 0) {
                            value = template[0..i];
                        } else {
                            value = template[end..i];
                        }
                        m_TokenList.Add(new Token(TokenType.String, value));
                    }
                    start = i;
                } else {
                    // End of an environment variable
                    int length = i - (start + 1);
                    if (length > 0) {
                        string variable = template[(start + 1)..i];
                        m_Found.Add(variable);
                        m_TokenList.Add(new Token(TokenType.Variable, variable));
                    } else {
                        // Two % characters one after the other
                        m_TokenList.Add(new Token(TokenType.String, "%"));
                    }

                    end = i + 1;
                    start = -1;
                }
            }

            if (start == -1 && end == -1) {
                m_Template = template;
            } else if (start != -1) {
                m_TokenList.Add(new Token(TokenType.String, template[start..]));
            } else if (end != -1) {
                m_TokenList.Add(new Token(TokenType.String, template[end..]));
            }
        }

        /// <summary>
        /// Determines whether the template uses a specified variable.
        /// </summary>
        /// <param name="variable">The variable to check for usage.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the specified variable is in use; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="variable"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="variable"/> is empty.</exception>
        public bool ContainsVariable(string variable)
        {
            if (variable is null) throw new ArgumentNullException(nameof(variable));
            if (string.IsNullOrEmpty(variable))
                throw new ArgumentException(AppResources.DomainTemplateEmptyVar, nameof(variable));

            return m_Found.Contains(variable);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that has all variables substituted.
        /// </summary>
        /// <returns>A <see cref="string"/> that has all variables substituted.</returns>
        public override string ToString()
        {
            if (m_Template is object) return m_Template;

            StringBuilder sb = new StringBuilder();
            foreach (Token token in m_TokenList) {
                switch (token.TokenType) {
                case TokenType.String:
                    sb.Append(token.Value);
                    break;
                case TokenType.Variable:
                    string envVar = Environment.GetEnvironmentVariable(token.Value);
                    if (Variables.TryGetValue(token.Value, out string var)) {
                        sb.Append(var);
                    } else if (envVar is object) {
                        sb.Append(envVar);
                    }
                    break;
                }
            }

            return sb.ToString();
        }
    }
}
