namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Net.Sockets;

    public class TcpConnectionEventArgs : EventArgs
    {
        public TcpConnectionEventArgs(TcpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            ConnectedClient = client;
        }

        public TcpClient ConnectedClient { get; }
    }
}
