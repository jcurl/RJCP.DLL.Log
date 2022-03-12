namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;

    public class NullInputStream : IInputStream
    {
        public string Scheme { get { return "null"; } }

        public string Connection { get { return "null:"; } }

        public bool IsLiveStream
        {
            get { return false; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.File; } }

        public bool RequiresConnection { get { return false; } }

        public Stream InputStream { get; private set; }

        public virtual void Open()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            if (InputStream == null)
                InputStream = new MemoryStream(Array.Empty<byte>());
        }

        public virtual Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            return Task.FromResult(true);
        }

        private bool m_IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                if (!m_IsDisposed) {
                    if (InputStream != null) InputStream.Dispose();
                    m_IsDisposed = true;
                }
            }
        }
    }
}
