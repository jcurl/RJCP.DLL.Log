namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    public sealed class MemoryOutput : IOutputStream
    {
        public MemoryOutput() : this(true) { }

        public MemoryOutput(bool supportsBinary)
        {
            SupportsBinary = supportsBinary;
        }

        public bool SupportsBinary { get; }

        public string FileName { get; private set; }

        public InputFormat InputFormat { get; private set; }

        public IList<OutputPacket> Lines { get; } = new List<OutputPacket>();

        public void SetInput(string fileName, InputFormat inputFormat)
        {
            FileName = fileName;
            InputFormat = inputFormat;
        }

        public bool Write(DltTraceLineBase line)
        {
            Lines.Add(new OutputPacket(line));
            return true;
        }

        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            Lines.Add(new OutputPacket(line, packet));
            return true;
        }

        public void Dispose()
        {
            Lines.Clear();
        }
    }
}
