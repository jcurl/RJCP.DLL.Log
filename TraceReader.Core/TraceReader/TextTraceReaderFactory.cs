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
        private readonly TextDecoderFactory m_DecoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTraceReaderFactory"/> class.
        /// </summary>
        public TextTraceReaderFactory() : this(new TextDecoderFactory()) { }

        private TextTraceReaderFactory(TextDecoderFactory decoderFactory) : base(decoderFactory)
        {
            m_DecoderFactory = decoderFactory;
        }

        /// <summary>
        /// Gets or sets the encoding that should be used when decoding lines.
        /// </summary>
        /// <value>The encoding to use when decoding byte data to characters.</value>
        /// <exception cref="ArgumentNullException">This property is set to <see langword="null"/>.</exception>
        public Encoding Encoding
        {
            get { return m_DecoderFactory.Encoding; }
            set { m_DecoderFactory.Encoding = value; }
        }
    }
}
