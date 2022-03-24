namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;

    /// <summary>
    /// Non-verbose argument used for decoding payloads with no Fibex file.
    /// </summary>
    public class NonVerboseDltArg : DltArgBase<byte[]>
    {
        private string m_NonVerboseArg;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseDltArg"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="data">The data.</param>
        public NonVerboseDltArg(int messageId, byte[] data)
            : base(data ?? Array.Empty<byte>())
        {
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (m_NonVerboseArg == null) {
                StringBuilder builder = new StringBuilder(4 * Data.Length + 20);
                builder.Append('[').Append(unchecked((uint)MessageId)).Append("] ");
                if (Data.Length > 0) {
                    int offset = builder.Length;
                    builder.Length += Data.Length;
                    for (int i = 0; i < Data.Length; i++) {
                        if (Data[i] >= 32 && Data[i] <= 126) {
                            builder[offset + i] = (char)Data[i];
                        } else {
                            builder[offset + i] = '-';
                        }
                    }
                    builder.Append('|');
                    HexConvert.ConvertToHex(builder, Data.AsSpan());
                }
                m_NonVerboseArg = builder.ToString();
            }
            return m_NonVerboseArg;
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="T:System.Text.StringBuilder" />.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(ToString());
        }
    }
}
