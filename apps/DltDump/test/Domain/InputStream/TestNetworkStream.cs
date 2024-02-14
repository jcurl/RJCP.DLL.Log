namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;

    public sealed class TestNetworkStream : IInputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestNetworkStream"/> class.
        /// </summary>
        public TestNetworkStream() { }

        public event EventHandler<ConnectSuccessEventArgs> OpenEvent;

        public event EventHandler<ConnectSuccessEventArgs> ConnectEvent;

        private void OnOpenEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = OpenEvent;
            if (handler is not null) handler(sender, args);
        }

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler is not null) handler(sender, args);
        }

        public string Scheme { get { return "net"; } }

        public string Connection { get { return "net://127.0.0.1"; } }

        public string InputFileName { get { return null; } }

        public bool IsLiveStream
        {
            get { return true; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        public bool RequiresConnection { get { return true; } }

        public Stream InputStream { get; private set; }

        public IPacket InputPacket { get { return null; } }

        public void Open()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            if (InputStream is not null)
                throw new InvalidOperationException("TestNetworkStream already opened");

            ConnectSuccessEventArgs createArgs = new();
            OnOpenEvent(this, createArgs);
            if (!createArgs.Succeed) throw new InputStreamException("TestNetworkStream creation failed");

            InputStream = new MemoryStream(Array.Empty<byte>());
        }

        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            ConnectSuccessEventArgs args = new();
            OnConnectEvent(this, args);
            return Task.FromResult(args.Succeed);
        }

        public void Close()
        {
            if (InputStream is not null) InputStream.Dispose();
            InputStream = null;
        }

        private bool m_IsDisposed;

        public void Dispose()
        {
            if (!m_IsDisposed) {
                Close();
                m_IsDisposed = true;
            }
        }
    }
}
