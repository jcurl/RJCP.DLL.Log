namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Text;
    using Decoder;

    /// <summary>
    /// A factory to return a <see cref="ITraceReader{T}"/> for reading text files.
    /// </summary>
    /// <remarks>
    /// Decoding used for the Text decoder is UTF8 by default. Change by setting the <see cref="Encoding"/>
    /// property.
    /// </remarks>
    public class TextTraceReaderFactory : TraceReaderFactory<TraceLine>
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
                if (m_Encoding == null) {
                    m_Encoding = GetDefaultEncoding();
                }
                return m_Encoding;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Encoding));
                m_Encoding = value;
            }
        }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="TextDecoder"/> to split log lines as text.</returns>
        protected override ITraceDecoder<TraceLine> GetDecoder()
        {
            TextDecoder decoder = new TextDecoder();
            if (m_Encoding != null) decoder.Encoding = m_Encoding;
            return decoder;
        }
    }
}
