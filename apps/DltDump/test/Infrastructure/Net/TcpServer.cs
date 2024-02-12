namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public sealed class TcpServer : IDisposable
    {
        private readonly object m_StartLock = new object();
        private readonly TcpListener m_Listener;

        public TcpServer(IPAddress localAddress, int port)
        {
            ArgumentNullException.ThrowIfNull(localAddress);
            if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            m_Listener = new TcpListener(localAddress, port) {
                // This allows a new instance to start immediately after it's closed, ignoring the linger state.
                ExclusiveAddressUse = false
            };
        }

        public event EventHandler<TcpConnectionEventArgs> ClientConnected;

        private void OnClientConnected(object sender, TcpConnectionEventArgs args)
        {
            EventHandler<TcpConnectionEventArgs> handler = ClientConnected;
            if (handler is object) handler(sender, args);
        }

        private bool m_Started;

        public async Task ListenAsync()
        {
            lock (m_StartLock) {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(nameof(TcpServer));

                m_Listener.Start(1);
                m_Started = true;
            }

            while (!m_IsDisposed) {
                TcpClient client;
                try {
                    client = await m_Listener.AcceptTcpClientAsync();
                } catch (ObjectDisposedException) {
                    continue;
                }
                OnClientConnected(this, new TcpConnectionEventArgs(client));
            }
        }

        private bool m_IsDisposed;

        public void Dispose()
        {
            if (!m_IsDisposed) {
                lock (m_StartLock) {
                    m_IsDisposed = true;
                    if (m_Started) m_Listener.Stop();
                }
            }
        }
    }
}
