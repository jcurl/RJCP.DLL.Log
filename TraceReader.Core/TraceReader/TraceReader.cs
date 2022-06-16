namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Decoder;

    /// <summary>
    /// A common implementation of <see cref="ITraceReader{T}"/> that reads from a stream and uses a
    /// <see cref="ITraceDecoder{T}"/> to decode log lines.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    public class TraceReader<T> : ITraceReader<T> where T : class, ITraceLine
    {
        private readonly Stream m_Stream;
        private readonly ITraceDecoder<T> m_Decoder;
        private readonly byte[] m_Buffer = new byte[65536];
        private readonly Memory<byte> m_BufferMem;

        private long m_Position;
        private bool m_StreamEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceReader{T}"/> class.
        /// </summary>
        /// <param name="stream">The readable stream to decode.</param>
        /// <param name="decoder">The decoder.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="decoder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        public TraceReader(Stream stream, ITraceDecoder<T> decoder)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (decoder == null) throw new ArgumentNullException(nameof(decoder));

            if (!stream.CanRead) throw new ArgumentException("Stream is not readable", nameof(stream));

            m_Stream = stream;
            m_Decoder = decoder;
            m_BufferMem = m_Buffer.AsMemory();
        }

        private IEnumerator<T> m_LineEnumerator;

        /// <summary>
        /// Gets the next line from the stream asynchronously.
        /// </summary>
        /// <returns>
        /// The next line from the stream. If the result is <see langword="null"/>, then the stream end has been
        /// reached.
        /// </returns>
        public async Task<T> GetLineAsync()
        {
            do {
                if (m_LineEnumerator != null) {
                    if (m_LineEnumerator.MoveNext()) {
                        return m_LineEnumerator.Current;
                    }
                    m_LineEnumerator = null;
                }

                // We have no more data to parse.
                if (m_StreamEnd) return null;

                IEnumerable<T> lines;
                int read = await m_Stream.ReadAsync(m_BufferMem, CancellationToken.None);
                if (read <= 0) {
                    m_StreamEnd = true;
                    lines = m_Decoder.Flush();
                } else {
                    lines = m_Decoder.Decode(m_Buffer.AsSpan(0, read), m_Position);
                    m_Position += read;
                }

                if (lines == null) return null;
                m_LineEnumerator = lines.GetEnumerator();
            } while (true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and/or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Set to <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            m_Decoder.Dispose();
        }
    }
}
