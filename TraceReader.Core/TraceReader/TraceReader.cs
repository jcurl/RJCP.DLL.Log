﻿namespace RJCP.Diagnostics.Log
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
        private readonly bool m_OwnsStream;
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
        /// <remarks>
        /// This class will dispose the <paramref name="decoder"/>, but it does now own the <paramref name="stream"/>,
        /// so this will not be disposed of.
        /// </remarks>
        public TraceReader(Stream stream, ITraceDecoder<T> decoder)
            : this(stream, decoder, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceReader{T}"/> class.
        /// </summary>
        /// <param name="stream">The readable stream to decode.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="ownsStream">Set to <see langword="true"/> if this object should own the stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="decoder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        /// <remarks>
        /// This class will dispose the <paramref name="decoder"/>. When owning the stream, this object will dispose of
        /// the <paramref name="stream"/>. This is useful for factories that would create its own stream temporarily,
        /// and then need this class to close it on disposal.
        /// </remarks>
        public TraceReader(Stream stream, ITraceDecoder<T> decoder, bool ownsStream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(decoder);
            if (!stream.CanRead) throw new ArgumentException("Stream is not readable", nameof(stream));

            m_Stream = stream;
            m_Decoder = decoder;
            m_BufferMem = m_Buffer.AsMemory();
            m_OwnsStream = ownsStream;
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
                if (m_LineEnumerator is not null) {
                    if (m_LineEnumerator.MoveNext()) {
                        return m_LineEnumerator.Current;
                    }
                    m_LineEnumerator = null;
                }

                // We have no more data to parse.
                if (m_StreamEnd) return null;

                IEnumerable<T> lines;
                int read = await m_Stream.ReadAsync(m_BufferMem, CancellationToken.None).ConfigureAwait(false);
                if (read <= 0) {
                    m_StreamEnd = true;
                    lines = m_Decoder.Flush();
                } else {
                    lines = m_Decoder.Decode(m_Buffer.AsSpan(0, read), m_Position);
                    m_Position += read;
                }

                if (lines is null) return null;
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
            if (m_OwnsStream) m_Stream.Dispose();
        }
    }
}
