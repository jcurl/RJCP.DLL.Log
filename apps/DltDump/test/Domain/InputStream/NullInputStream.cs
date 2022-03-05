﻿namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class NullInputStream : IInputStream
    {
        public NullInputStream()
        {
            InputStream = new MemoryStream(Array.Empty<byte>());
        }

        public Stream InputStream { get; }

        public bool IsLiveStream
        {
            get { return false; }
        }

        public InputFormat SuggestedFormat { get { return InputFormat.File; } }

        public Task<bool> ConnectAsync()
        {
            return Task.FromResult(true);
        }

        public void Dispose() { /* Nothing to do */ }
    }
}
