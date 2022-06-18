namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.IO;

    public class NullStream : Stream
    {
        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return true; } }

        public override bool CanWrite { get { return true; } }

        private long m_Length;

        public override long Length { get { return m_Length; } }

        private long m_Position;

        public override long Position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                if (m_Position > m_Length) m_Length = m_Position;
            }
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (offset > buffer.Length - count) throw new ArgumentException("Out of bounds");

            long maxRead = m_Length - m_Position;
            if (count > maxRead) {
                m_Position = m_Length;
                return (int)maxRead;
            }
            m_Position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin) {
            case SeekOrigin.Begin:
                m_Position = offset;
                if (m_Length < m_Position) m_Length = m_Position;
                return m_Position;
            case SeekOrigin.Current:
                m_Position += offset;
                if (m_Length < m_Position) m_Length = m_Position;
                return m_Position;
            case SeekOrigin.End:
                if (offset > m_Length) {
                    m_Position = 0;
                } else {
                    m_Position = m_Length - offset;
                }
                return m_Position;
            default:
                throw new ArgumentException("Unknown origin");
            }
        }

        public override void SetLength(long value)
        {
            if (value < 0) value = 0;

            m_Length = value;
            if (m_Position > m_Length) m_Position = m_Length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (offset > buffer.Length - count) throw new ArgumentException("Out of bounds");

            m_Position += count;
            if (m_Position > m_Length) m_Length = m_Position;
        }
    }
}
