namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Diagnostics.Log.Dlt;

    public sealed class OutputPacket
    {
        public OutputPacket(DltTraceLineBase line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            Line = line;
        }

        public OutputPacket(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            Line = line;
            Packet = packet.ToArray();
        }

        public DltTraceLineBase Line { get; }

        public byte[] Packet { get; }
    }
}
