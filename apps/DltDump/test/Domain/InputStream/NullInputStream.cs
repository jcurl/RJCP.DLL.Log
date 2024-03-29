﻿namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;

    public class NullInputStream : IInputStream
    {
        public string Scheme { get { return "null"; } }

        public string Connection { get { return "null:"; } }

        public string InputFileName { get { return null; } }

        public bool IsLiveStream
        {
            get { return false; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.File; } }

        public bool RequiresConnection { get { return false; } }

        public Stream InputStream { get; private set; }

        public IPacket InputPacket { get { return null; } }

        public virtual void Open()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            InputStream ??= new MemoryStream(Array.Empty<byte>());
        }

        public virtual Task<bool> ConnectAsync()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            return Task.FromResult(true);
        }

        public void Close()
        {
            if (InputStream is not null) InputStream.Dispose();
            InputStream = null;
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
                    Close();
                    m_IsDisposed = true;
                }
            }
        }
    }
}
