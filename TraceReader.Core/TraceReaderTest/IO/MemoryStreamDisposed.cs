namespace RJCP.Diagnostics.Log.IO
{
    using System.IO;

    public class MemoryStreamDisposed : Stream
    {
        private readonly MemoryStream m_Stream = new MemoryStream();

        public override bool CanRead { get { return m_Stream.CanRead; } }

        public override bool CanSeek { get { return m_Stream.CanSeek; } }

        public override bool CanWrite { get { return m_Stream.CanWrite; } }

        public override long Length { get { return m_Stream.Length; } }

        public override long Position
        {
            get { return m_Stream.Position; }
            set { m_Stream.Position = value; }
        }

        public override void Flush()
        {
            m_Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_Stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_Stream.Write(buffer, offset, count);
        }

        public bool IsDisposed { get; set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
