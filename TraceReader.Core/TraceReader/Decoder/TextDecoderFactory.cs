﻿namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Text;

    /// <summary>
    /// A factory for creating a <see cref="TextDecoder"/> object.
    /// </summary>
    public class TextDecoderFactory : ITraceDecoderFactory<TraceLine>
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
        /// Creates a new instance of a <see cref="TextDecoder"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="TextDecoder"/> object.</returns>
        public ITraceDecoder<TraceLine> Create()
        {
            TextDecoder decoder = new();
            if (m_Encoding is not null) decoder.Encoding = m_Encoding;
            return decoder;
        }
    }
}
