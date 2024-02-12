namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A general class to parse text files and split them up by line.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    /// <remarks>
    /// The position of each line in the stream is estimated, with the assumption that each character is one byte. This
    /// approximation is necessary as it is not possible to calculate how much space each individual character consumes.
    /// </remarks>
    public abstract class TextDecoderBase<T> : ITraceDecoder<T> where T : class, ITraceLine
    {
        // Defines the size of the internal character buffer when converting from input bytes to characters. A line
        // cannot be longer than this. If it is longer, then it will be truncated to this line length.
        private const int BufferLength = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDecoderBase{T}"/> class with UTF8 decoding.
        /// </summary>
        protected TextDecoderBase() : this(GetDefaultEncoding()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDecoderBase{T}"/> class with specified decoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        protected TextDecoderBase(Encoding encoding)
        {
            Encoding = encoding;
        }

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
            get { return m_Encoding; }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                Decoder = value.GetDecoder();
                m_Encoding = value;
            }
        }

        private Decoder Decoder { get; set; }

        private readonly List<T> m_Lines = new List<T>();

        private readonly char[] m_LineBuffer = new char[BufferLength];
        private int m_Offset;       // Offset into m_LineBuffer for data already received and cached.
        private bool m_SkipCrLf;    // If m_LineBuffer[0] is 0x0A, should be ignored
        private long m_Position;    // Position in the stream for m_LineBuffer[0]

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        public IEnumerable<T> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            if (buffer.Length == 0) return Array.Empty<T>();

            ReadOnlySpan<byte> inputBuffer = buffer;
            Span<char> outputBuffer = m_LineBuffer;

            m_Lines.Clear();
            do {
                int bu, cu;
                bool complete;
                try {
                    Decoder.Convert(inputBuffer, outputBuffer[m_Offset..], false, out bu, out cu, out complete);
                } catch (ArgumentException ex) {
                    if (ex.ParamName is null || !ex.ParamName.Equals("chars")) throw;

                    // Handle the case the user tries to convert a UTF8 sequence (4-bytes) to two UTF16 characters when
                    // there is only a single UTF16 char available at the end. We'll get an exception. So we need to
                    // convert what we have to a new line, clear the buffer, and try again now that there is more space.
                    //
                    // It is expected that the Framework throws this exception if there is no other data prior to parse,
                    // so that the previous call passes and we would parse that first.
                    m_Lines.Add(GetLine(m_LineBuffer.AsSpan(0, m_Offset), m_Position));
                    m_Offset = 0;

                    Decoder.Convert(inputBuffer, outputBuffer, false, out bu, out cu, out complete);

                    // The position in this case may be incorrect. If the call to Decode is within the 4-byte UTF8 for
                    // the UTF8 decoder, the position may be after, but not before the end of the 4-byte sequence. The
                    // error occurs because the Decoder.Convert caches bytes between calls (i.e. the first byte of the
                    // sequence is cached, and a second call gives the 2nd, 3rd, 4th byte), we take the position of the
                    // second byte, even though what was decoded is a byte earlier. There is no workaround for this
                    // unless there's a new implementation of Decoder.Convert that never caches the bytes received.
                    m_Position = position;
                }

                int pos = 0;        // Offset into m_LineBuffer where the current line starts (after last 0x0D or 0x0A).
                int posCrLf = m_SkipCrLf ? 0 : -1;       // Offset into m_LineBuffer immediately after the last 0x0D.

                int scanStart = m_Offset;
                int scanEnd = m_Offset + cu;

                // Look for a new line. It can end with 0x0A, 0x0D 0x0A, 0x0D
                for (int i = scanStart; i < scanEnd; i++) {
                    if (m_LineBuffer[i] == 0x0A) {
                        if (i == posCrLf) {
                            // This is a CR LF, so we skip it.
                            pos++;
                        } else {
                            m_Lines.Add(GetLine(m_LineBuffer.AsSpan(pos, i - pos), position + (pos - scanStart)));
                            pos = i + 1;
                        }
                    } else if (m_LineBuffer[i] == 0x0D) {
                        m_Lines.Add(GetLine(m_LineBuffer.AsSpan(pos, i - pos), position + (pos - scanStart)));
                        pos = i + 1;
                        posCrLf = pos;
                    }
                }

                if (pos == 0) {
                    // We're scanning from the beginning of the buffer (pos == 0), and didn't find a new line.
                    if (scanEnd == m_LineBuffer.Length) {
                        // The buffer is full, so we'll never find any further new line and must clear it to accept the
                        // next block. Convert the current buffer to a string.
                        m_Lines.Add(GetLine(m_LineBuffer.AsSpan(0, scanEnd), m_Position));
                        m_Offset = 0;
                        position += bu;
                        m_Position = position - scanStart;
                    } else {
                        // The buffer isn't full, so go back and scan some more (potentially exiting the loop and having
                        // to wait for a new call to Decode()).
                        if (m_Offset == 0) m_Position = position;
                        m_Offset = scanEnd;
                        position += bu;
                    }
                    m_SkipCrLf = false;
                } else if (pos < scanEnd) {
                    // There was no ending CR or LF. That means, we need to copy this to the beginning of the buffer.
                    // Even though we prefer working with our data in-place, we expect that the line itself is quite
                    // shorter than the actual buffer we allocated, so there should normally be not much to copy.
                    Array.Copy(m_LineBuffer, pos, m_LineBuffer, 0, scanEnd - pos);
                    m_Offset = scanEnd - pos;
                    m_SkipCrLf = (posCrLf == pos);
                    m_Position = position + pos - scanStart;
                    position += pos - scanStart;
                } else {
                    m_Offset = 0;
                    m_SkipCrLf = (posCrLf == pos);
                    position += pos - scanStart;
                }

                if (complete) return m_Lines;
                inputBuffer = inputBuffer[bu..];
            } while (true);
        }

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        public IEnumerable<T> Flush()
        {
            m_Lines.Clear();
            if (m_Offset == 0) return Array.Empty<T>();

            T line = GetLine(m_LineBuffer.AsSpan(0, m_Offset), m_Position);
            m_Offset = 0;
            m_SkipCrLf = false;
            return new[] { line };
        }

        /// <summary>
        /// Gets the line that was decoded.
        /// </summary>
        /// <param name="line">The line as a memory slice.</param>
        /// <param name="position">The position in the stream where the memory slice starts.</param>
        /// <returns>A new <see cref="ITraceLine"/> derived object.</returns>
        /// <remarks>
        /// Use the <paramref name="line"/> as a span to convert to a string, or to further process. The
        /// <see cref="Span{T}"/> reduces the number of string copy operations when scanning the string, which can
        /// result in faster decoding.
        /// </remarks>
        protected abstract T GetLine(Span<char> line, long position);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Is <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            /* Nothing to dispose */
        }
    }
}
