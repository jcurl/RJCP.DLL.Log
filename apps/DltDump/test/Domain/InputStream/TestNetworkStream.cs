namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;

    public sealed class TestNetworkStream : IInputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestNetworkStream"/> class.
        /// </summary>
        /// <param name="requiredAttempts">
        /// The required attempts. If negative, it will always fail. If zero, it will always succeed. If more than zero,
        /// the first number of attempts will fail.
        /// </param>
        public TestNetworkStream()
        {
            InputStream = new MemoryStream(Array.Empty<byte>());
        }

        public event EventHandler<ConnectSuccessEventArgs> ConnectEvent;

        private void OnConnectEvent(object sender, ConnectSuccessEventArgs args)
        {
            EventHandler<ConnectSuccessEventArgs> handler = ConnectEvent;
            if (handler != null) handler(sender, args);
        }

        public string Scheme { get { return "net"; } }

        public Stream InputStream { get; }

        public bool IsLiveStream
        {
            get { return true; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        public bool RequiresConnection { get { return true; } }

        public Task<bool> ConnectAsync()
        {
            ConnectSuccessEventArgs args = new ConnectSuccessEventArgs();
            OnConnectEvent(this, args);
            return Task.FromResult(args.Succeed);
        }

        public void Dispose() { /* Nothing to do */ }
    }
}
