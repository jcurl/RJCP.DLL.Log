namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public sealed class TestNetworkStreamFactory : InputStreamFactoryBase
    {
        public event EventHandler<ConnectSuccessEventArgs> CreateEvent;

        public event EventHandler<ConnectSuccessEventArgs> ConnectEvent;

        private void OnCreateEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = CreateEvent;
            if (handler != null) handler(sender, args);
        }

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler != null) handler(sender, args);
        }

        public override IInputStream Create(Uri uri)
        {
            ConnectSuccessEventArgs createArgs = new ConnectSuccessEventArgs();
            OnCreateEvent(this, createArgs);
            if (!createArgs.Succeed) throw new InputStreamException("TestNetworkStream creation failed");

            TestNetworkStream inputStream = new TestNetworkStream();
            inputStream.ConnectEvent += (s, e) => {
                OnConnectEvent(s, e);
            };
            return inputStream;
        }
    }
}
