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
            if (handler != null) handler(sender, args);
        }

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler != null) handler(sender, args);
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
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            if (InputStream != null)
                throw new InvalidOperationException("TestPacketReader already opened");

            ConnectSuccessEventArgs createArgs = new ConnectSuccessEventArgs();
            OnOpenEvent(this, createArgs);
            if (!createArgs.Succeed) throw new InputStreamException("TestPacketReader creation failed");

            InputPacket = new EmptyPacketReceiver();
        }

        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(NullInputStream));

            ConnectSuccessEventArgs args = new ConnectSuccessEventArgs();
            OnConnectEvent(this, args);
            return Task.FromResult(args.Succeed);
        }

        public void Close()
        {
            if (InputPacket != null) InputPacket.Dispose();
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
