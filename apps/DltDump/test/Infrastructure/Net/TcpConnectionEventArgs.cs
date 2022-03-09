namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Net.Sockets;

    public class TcpConnectionEventArgs : EventArgs
    {
        public TcpConnectionEventArgs(TcpClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            ConnectedClient = client;
        }

        public TcpClient ConnectedClient { get; }
    }
}
