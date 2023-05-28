namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Dlt;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// A common implementation of <see cref="ITraceWriter{T}"/> that writes to a stream and uses a
    /// <see cref="ITraceEncoder{TLine}"/> to encode log lines.
    /// </summary>
    public class DltTraceWriter : ITraceWriter<DltTraceLineBase>
    {
        private readonly Stream m_Stream;
        private readonly bool m_OwnsStream;
        private readonly ITraceEncoder<DltTraceLineBase> m_Encoder;
        private readonly byte[] m_Buffer = new byte[65535 + 16];

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public DltTraceWriter(Stream stream)
            : this(stream, new DltTraceEncoder(), false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoder">The encoder that creates the packets which can then be written to the stream.</param>
        /// <remarks>
        /// This class will dispose the <paramref name="encoder"/> (but not the <paramref name="stream"/>).
        /// </remarks>
        public DltTraceWriter(Stream stream, ITraceEncoder<DltTraceLineBase> encoder)
            : this(stream, encoder, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoder">The encoder that creates the packets which can then be written to the stream.</param>
        /// <param name="ownsStream">Set to <see langword="true"/> if this object should own the stream.</param>
        /// <remarks>
        /// This class will dispose the <paramref name="encoder"/> (but not the <paramref name="stream"/>). When owning
        /// the stream, this object will dispose of the stream. This is useful for factories that would create its own
        /// stream temporarily, and then need this class to close it.
        /// </remarks>
        public DltTraceWriter(Stream stream, ITraceEncoder<DltTraceLineBase> encoder, bool ownsStream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (encoder is null) throw new ArgumentNullException(nameof(encoder));

            if (!stream.CanWrite) throw new ArgumentException("Stream is not writable", nameof(stream));

            m_Stream = stream;
            m_Encoder = encoder;
            m_OwnsStream = ownsStream;
        }

        /// <summary>
        /// Writes the line asynchronously to the output.
        /// </summary>
        /// <param name="line">The line to write to the output.</param>
        /// <returns>A Task that can be waited upon.</returns>
        public async Task<bool> WriteLineAsync(DltTraceLineBase line)
        {
            Result<int> result = m_Encoder.Encode(m_Buffer, line);
            if (!result.TryGet(out int length)) return false;

            await m_Stream.WriteAsync(m_Buffer.AsMemory(0, length), CancellationToken.None);
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
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
        /// <remarks>
        /// This method also disposes the encoder provided.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            m_Encoder.Dispose();
            if (m_OwnsStream) m_Stream.Dispose();
        }
    }
}
