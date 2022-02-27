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
            private bool m_CacheLocked;

            /// <summary>
            /// Gets the amount of data cached in this cache.
            /// </summary>
            /// <value>The length of the cache.</value>
            public int CacheLength { get { return m_CacheLength; } }

            /// <summary>
            /// Gets a value indicating whether this instance has cached data.
            /// </summary>
            /// <value>
            /// Is <see langword="true"/> if this there is cached data; otherwise, <see langword="false"/>.
            /// </value>
            public bool IsCached
            {
                get { return m_CacheLength > 0; }
            }

            /// <summary>
            /// Gets or sets the virtual offset.
            /// </summary>
            /// <value>The virtual offset, in bytes.</value>
            /// <remarks>
            /// The virtual offset can be set by the decoder, to calculate the offset of bytes where cached data is
            /// present, in comparison to the actual input data. Usually it will initialize the
            /// <see cref="CacheWriteOffset"/> property to be the negative of the <see cref="CacheLength"/>, indicating
            /// that there are <see cref="CacheLength"/> bytes available before the buffer start.
            /// <para>
            /// As data is <see cref="Consume(int)"/> d, the <see cref="CacheWriteOffset"/> is incremented. The decoder
            /// can use this to determine if enough data has been consumed, that it can clear this cache and just use
            /// the original buffer at the real offset given by this <see cref="CacheWriteOffset"/>.
            /// </para>
            /// </remarks>
            public int CacheWriteOffset { get; private set; }

            /// <summary>
            /// Gets the cache buffer.
            /// </summary>
            /// <returns>The cache buffer.</returns>
            public ReadOnlySpan<byte> GetCache() { return m_Cache.AsSpan(m_CacheStart, m_CacheLength); }

            /// <summary>
            /// Indicate new buffer data.
            /// </summary>
            /// <remarks>
            /// Indicates a new data buffer. This is used to maintain the current position in the stream, which is
            /// updated as data is consumed.
            /// </remarks>
            public void Write()
            {
                if (m_CacheLocked) throw new InvalidOperationException("Line cache is locked");
                CacheWriteOffset = -m_CacheLength;
            }

            /// <summary>
            /// Reduces the number of bytes in the cache.
            /// </summary>
            /// <param name="bytes">The number of bytes to remove from the beginning of the cache.</param>
            /// <returns>The actual number of bytes consumed from the cache.</returns>
            public int Consume(int bytes)
            {
                if (m_CacheLocked) throw new InvalidOperationException("Line cache is locked");
                CacheWriteOffset += bytes;

                if (bytes >= m_CacheLength) {
                    int consumed = m_CacheLength;
                    m_CacheStart = 0;
                    m_CacheLength = 0;
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
            public void Append(ReadOnlySpan<byte> buffer)
            {
                if (m_CacheLocked) throw new InvalidOperationException("Line cache is locked");
                if (buffer.Length == 0) return;

                if (buffer.Length <= CacheSize - m_CacheStart - m_CacheLength) {
                    buffer.CopyTo(m_Cache.AsSpan(m_CacheStart + m_CacheLength, buffer.Length));
                    m_CacheLength += buffer.Length;
                } else if (buffer.Length <= CacheSize - m_CacheLength) {
                    // Shift the cache to the start, then append.
                    Array.Copy(m_Cache, m_CacheStart, m_Cache, 0, m_CacheLength);
                    m_CacheStart = 0;
                    buffer.CopyTo(m_Cache.AsSpan(m_CacheLength, buffer.Length));
                    m_CacheLength += buffer.Length;
                } else {
                    throw new InsufficientMemoryException("Buffer appended would exceed available cache size");
                }
            }

            public ReadOnlySpan<byte> SetFlush()
            {
                ReadOnlySpan<byte> buffer = m_Cache.AsSpan(m_CacheStart, m_CacheLength);
                m_CacheStart = 0;
                m_CacheLength = 0;
                m_CacheLocked = true;
                return buffer;
            }

            /// <summary>
            /// Resets the state of the cache.
            /// </summary>
            /// <remarks>The cache offset <see cref="CacheWriteOffset"/> is not affected.</remarks>
            public void Clear()
            {
                m_CacheStart = 0;
                m_CacheLength = 0;
            }
        }
    }
}
