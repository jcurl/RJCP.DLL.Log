namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public sealed class TestPacketReaderFactory : InputStreamFactoryBase
    {
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

        public override IInputStream Create(Uri uri)
        {
            TestPacketReader inputPacket = new();
            inputPacket.OpenEvent += OnOpenEvent;
            inputPacket.ConnectEvent += OnConnectEvent;
            return inputPacket;
        }
    }
}
