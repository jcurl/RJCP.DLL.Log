namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Diagnostics;
    using Dlt;

    public abstract partial class DltTraceDecoderBase
    {
        /// <summary>
        /// Handles caching of data between calls to <see cref="DltTraceDecoderBase.Decode(ReadOnlySpan{byte}, long)"/>.
        /// </summary>
        [DebuggerDisplay("Cached = {m_CacheLength}")]
        private sealed class LineCache
        {
            private const int MaxPacket = DltConstants.MaxStorageHeaderSize + DltConstants.MaxPacketSize;
            private const int CacheSize = MaxPacket * 2;
            private readonly byte[] m_Cache = new byte[CacheSize];

            private int m_CacheStart;
            private int m_CacheLength;

            public int Length { get { return m_CacheLength; } }

            public ReadOnlySpan<byte> GetCache() { return m_Cache.AsSpan(m_CacheStart, m_CacheLength); }

            public int Consume(int bytes)
            {
                if (bytes >= m_CacheLength) {
                    int consumed = m_CacheLength;
                    Reset();
                    return consumed;
                }
                m_CacheStart += bytes;
                m_CacheLength -= bytes;
                return bytes;
            }

            /// <summary>
            /// Appends the specified buffer to the cache line.
            /// </summary>
            /// <param name="buffer">The buffer containing the data to append.</param>
            /// <returns>The number of bytes discarded, either by the cache, or the appended buffer.</returns>
            /// <remarks>
            /// Appending data to the cached line should only be done when we're confident we have a DLT packet, but
            /// have not yet received all the data. Data is only discarded when appending if the cache size is less than
            /// the maximum size of the packet. This implementation has more cache data than a maximum packet size as an
            /// optimization to reduce the number of copy operations in specific cases when appending (and the data
            /// happens to be at the end of the cache already).
            /// <para>
            /// The only case data can then be lost (the return value of this function is then non-zero), can be if the
            /// packet data being recorded is greater than the cache size, which is greater than the maximum size of a
            /// DLT packet. As a packet is consumed with the <see cref="Consume(int)"/> function, either through invalid
            /// data, or finding a line.
            /// </para>
            /// <para>
            /// Once a valid line is found, the rest of the line should also be added to the cache, so that parsing can
            /// occur on this buffer, instead of the original buffer. Else if data doesn't need to be cached, use the
            /// original buffer direct to reduce the number of copy operations.
            /// </para>
            /// </remarks>
            public int Append(ReadOnlySpan<byte> buffer)
            {
                if (buffer.Length == 0) return 0;

                // If the amount of data we have exceeds our cache, we need to truncate.
                int skipped = 0;
                ReadOnlySpan<byte> append = buffer;
                if (buffer.Length + m_CacheLength > CacheSize) {
                    skipped = m_CacheLength + buffer.Length - CacheSize;
                    m_CacheLength -= skipped;
                    m_CacheStart += skipped;
                    if (m_CacheLength <= 0) {
                        append = buffer[(-m_CacheLength)..];
                        Reset();
                    }
                }

                if (append.Length <= CacheSize - m_CacheStart - m_CacheLength) {
                    append.CopyTo(m_Cache.AsSpan(m_CacheStart + m_CacheLength, append.Length));
                    m_CacheLength += append.Length;
                } else {
                    Array.Copy(m_Cache, m_CacheStart, m_Cache, 0, m_CacheLength);
                    m_CacheStart = 0;
                    append.CopyTo(m_Cache.AsSpan(m_CacheLength, append.Length));
                    m_CacheLength += append.Length;
                }

                return skipped;
            }

            public void Reset()
            {
                m_CacheStart = 0;
                m_CacheLength = 0;
            }
        }
    }
}
