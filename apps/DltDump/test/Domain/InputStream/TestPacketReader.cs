namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;

    public sealed class TestPacketReader : IInputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPacketReader"/> class.
        /// </summary>
        public TestPacketReader() { }

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

        public string Scheme { get { return "pkt"; } }

        public string Connection { get { return "pkt://127.0.0.1"; } }

        public string InputFileName { get { return null; } }

        public bool IsLiveStream { get { return true; } }

        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        public bool RequiresConnection { get { return false; } }

        public Stream InputStream { get { return null; } }

        public IPacket InputPacket { get; set; }

        public void Open()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            if (InputStream is not null)
                throw new InvalidOperationException("TestPacketReader already opened");

            ConnectSuccessEventArgs createArgs = new();
            OnOpenEvent(this, createArgs);
            if (!createArgs.Succeed) throw new InputStreamException("TestPacketReader creation failed");

            InputPacket = new EmptyPacketReceiver();
        }

        public Task<bool> ConnectAsync()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            ConnectSuccessEventArgs args = new();
            OnConnectEvent(this, args);
            return Task.FromResult(args.Succeed);
        }

        public void Close()
        {
            if (InputPacket is object) InputPacket.Dispose();
            InputPacket = null;
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
