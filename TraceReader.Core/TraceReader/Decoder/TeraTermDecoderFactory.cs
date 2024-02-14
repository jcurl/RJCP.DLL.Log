namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Text;

    /// <summary>
    /// A factory for creating objects of type <see cref="TeraTermDecoder"/>.
    /// </summary>
    public class TeraTermDecoderFactory : ITraceDecoderFactory<LogTraceLine>
    {
        private static Encoding GetDefaultEncoding()
        {
            return Encoding.GetEncoding("UTF-8", new EncoderReplacementFallback("."), new DecoderReplacementFallback("."));
        }

        private Encoding m_Encoding;

        /// <summary>
        /// Gets or sets the encoding that should be used when decoding lines.
        /// </summary>
        /// <value>The encoding to use when decoding byte data to characters.</value>
        /// <exception cref="ArgumentNullException">This property is set to <see langword="null"/>.</exception>
        public Encoding Encoding
        {
            get
            {
                m_Encoding ??= GetDefaultEncoding();
                return m_Encoding;
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                m_Encoding = value;
            }
        }

        /// <summary>
        /// Creates a new instance of a <see cref="TeraTermDecoder"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="TeraTermDecoder"/> object.</returns>
        public ITraceDecoder<LogTraceLine> Create()
        {
            TeraTermDecoder decoder = new();
            if (m_Encoding is not null) decoder.Encoding = m_Encoding;
            return decoder;
        }
    }
}
