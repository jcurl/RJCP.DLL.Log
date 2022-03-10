﻿namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;

    public sealed class TestNetworkStream : IInputStream
    {
        private readonly bool m_ConnectResult;

        public TestNetworkStream(bool connectResult)
        {
            InputStream = new MemoryStream(Array.Empty<byte>());
            m_ConnectResult = connectResult;
        }

        public string Scheme { get { return "net"; } }

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
