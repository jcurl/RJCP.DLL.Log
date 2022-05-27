namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Diagnostics;

    public abstract partial class DltTraceDecoderBase
    {
        private sealed class PosMap
        {
            [DebuggerDisplay("Pos={Position}; Len={Blocksize}")]
            private struct PosBlock
            {
                // Current position for the start of the current block
                public long Position;

                // Current size of the current block
                public int Blocksize;
            }

            // A circular buffer for positions.
            private PosBlock[] m_PosMap;
            private int m_PosMapCount;
            private int m_PosMapHead;
            private int m_PosMapTail;

            // An optimization for fast modulo and buffer wrapping
            private int m_Modulo;
            private int m_ModuloPos;

            private long m_EndPosition;

            public PosMap() : this(32) { }

            public PosMap(int capacity)
            {
                // Ensure that the capacity is a multiple of 2, so we can use bit operators instead of division
                if (capacity < 2)
                    throw new ArgumentException("Capacity must be 2 or more");

                bool bitFound = false;
                for (int i = 0; i < 32; i++) {
                    if ((capacity & (1 << i)) != 0) {
                        if (bitFound)
                            throw new ArgumentException("Capacity must be a power of 2", nameof(capacity));
                        bitFound = true;
                    }
                    if (!bitFound) {
                        m_ModuloPos = (1 << i);
                        m_Modulo ^= m_ModuloPos;
                    }
                }

                m_PosMap = new PosBlock[capacity];
            }

            public long Position
            {
                get
                {
                    if (m_PosMapCount == 0) return m_EndPosition;
                    return m_PosMap[m_PosMapHead].Position;
                }
            }

            public int Length
            {
                get
                {
                    if (m_PosMapCount == 0) return 0;

                    int length = 0;
                    for (int i = 0; i < m_PosMapCount; i++) {
                        length += m_PosMap[(i + m_PosMapHead) & m_Modulo].Blocksize;
                    }

                    return length;
                }
            }

            public void Append(long position, int length)
            {
                if (m_PosMapCount == 0) {
                    m_PosMapCount = 1;
                    m_PosMapHead = 0;
                    m_PosMapTail = 0;
                } else if (position == m_EndPosition) {
                    m_EndPosition += length;
                    m_PosMap[m_PosMapTail].Blocksize += length;
                    return;
                } else {
                    if (m_PosMapCount == m_PosMap.Length) Realloc();

                    m_PosMapCount++;
                    m_PosMapTail = (m_PosMapTail + 1) & m_Modulo;
                }
                m_PosMap[m_PosMapTail].Blocksize = length;
                m_PosMap[m_PosMapTail].Position = position;
                m_EndPosition = position + length;
            }

            private void Realloc()
            {
                if ((m_Modulo & 0x80000000) != 0)
                    throw new InsufficientMemoryException("Cannot realloc, reached maximum size");

                PosBlock[] newMap = new PosBlock[m_PosMap.Length * 2];

                // Copy from the current map to the new map. We know it's full
                if (m_PosMapHead == 0) {
                    Array.Copy(m_PosMap, newMap, m_PosMapCount);
                } else {
                    Array.Copy(m_PosMap, m_PosMapHead, newMap, 0, m_PosMap.Length - m_PosMapHead);
                    Array.Copy(m_PosMap, 0, newMap, m_PosMap.Length - m_PosMapHead, m_PosMapHead);
                }

                // Update when we wrap in the new array
                m_ModuloPos <<= 1;
                m_Modulo ^= m_ModuloPos;

                m_PosMapHead = 0;
                m_PosMapTail = m_PosMapCount - 1;
                m_PosMap = newMap;
            }

            public void Consume(int length)
            {
                int remaining = length;
                while (remaining > 0) {
#if DEBUG
                    if (m_PosMapCount == 0) {
                        string message = string.Format("Consuming more data {0} than available {1}",
                            length, length - remaining);
                        throw new ArgumentOutOfRangeException(nameof(length), message);
                    }
#else
                    if (m_PosMapCount == 0) return;
#endif
                    if (m_PosMap[m_PosMapHead].Blocksize <= remaining) {
                        remaining -= m_PosMap[m_PosMapHead].Blocksize;
                        m_PosMapCount--;
                        m_PosMapHead = (m_PosMapHead + 1) & m_Modulo;
                    } else {
                        m_PosMap[m_PosMapHead].Blocksize -= remaining;
                        m_PosMap[m_PosMapHead].Position += remaining;
                        return;
                    }
                }
            }
        }
    }
}
