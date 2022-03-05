namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class TestNetworkStream : IInputStream
    {
        private readonly bool m_ConnectResult;

        public TestNetworkStream(bool connectResult)
        {
            InputStream = new MemoryStream(Array.Empty<byte>());
            m_ConnectResult = connectResult;
        }

        public Stream InputStream { get; }

        public bool IsLiveStream
        {
            get { return true; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        public Task<bool> ConnectAsync()
        {
            return Task.FromResult(m_ConnectResult);
        }

        public void Dispose() { /* Nothing to do */ }
    }
}
