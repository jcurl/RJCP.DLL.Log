namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;

    public sealed class TestNetworkStream : IInputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestNetworkStream"/> class.
        /// </summary>
        public TestNetworkStream() { }

        public event EventHandler<ConnectSuccessEventArgs> ConnectEvent;

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler != null) handler(sender, args);
        }

        public string Scheme { get { return "net"; } }

        public string Connection { get { return "net://127.0.0.1"; } }

        public bool IsLiveStream
        {
            get { return true; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        public bool RequiresConnection { get { return true; } }

        public Stream InputStream { get; private set; }

        public void Open()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            if (InputStream == null)
                InputStream = new MemoryStream(Array.Empty<byte>());
        }

        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            ConnectSuccessEventArgs args = new ConnectSuccessEventArgs();
            OnConnectEvent(this, args);
            return Task.FromResult(args.Succeed);
        }

        private bool m_IsDisposed;

        public void Dispose()
        {
            if (!m_IsDisposed) {
                if (InputStream != null) InputStream.Dispose();
                m_IsDisposed = true;
            }
        }
    }
}
