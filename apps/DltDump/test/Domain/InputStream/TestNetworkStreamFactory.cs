namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public sealed class TestNetworkStreamFactory : InputStreamFactoryBase
    {
        public event EventHandler<ConnectSuccessEventArgs> OpenEvent;

        public event EventHandler<ConnectSuccessEventArgs> ConnectEvent;

        private void OnOpenEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = OpenEvent;
            if (handler is object) handler(sender, args);
        }

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler is object) handler(sender, args);
        }

        public override IInputStream Create(Uri uri)
        {
            TestNetworkStream inputStream = new TestNetworkStream();
            inputStream.OpenEvent += OnOpenEvent;
            inputStream.ConnectEvent += OnConnectEvent;
            return inputStream;
        }
    }
}
