namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A basic test for writing DLT packets.
    /// </summary>
    public class TestOutputBase : OutputBase, IOutputStream
    {
        public TestOutputBase(string fileName) : base(fileName) { }

        public TestOutputBase(string fileName, bool force) : base(fileName, 0, force) { }

        public TestOutputBase(string fileName, long split, bool force) : base(fileName, split, force) { }

        public TestOutputBase(string fileName, InputFiles inputs, long split, bool force) : base(fileName, inputs, split, force) { }

        public bool SupportsBinary { get { return true; } }

        public bool ShowPosition { get; set; }

        private InputFormat m_InputFormat;

        public void SetInput(string fileName, InputFormat inputFormat)
        {
            m_InputFormat = inputFormat;
            SetInput(fileName);
        }

        private static readonly byte[] StorageHeader = new byte[] {
            0x44, 0x4C, 0x54, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x45, 0x43, 0x55, 0x31
        };

        public bool Write(DltTraceLineBase line)
        {
            if (ShowPosition) {
                Write(line.TimeStamp, "{0:x8}: {1}", line.Position, line.ToString());
            } else {
                Write(line.TimeStamp, line.ToString());
            }
            return true;
        }

        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            switch (m_InputFormat) {
            case InputFormat.File:
                Write(line.TimeStamp, packet);
                break;
            case InputFormat.Network:
                Write(line.TimeStamp, StorageHeader, packet);
                break;
            case InputFormat.Serial:
                Write(line.TimeStamp, StorageHeader, packet[4..]);
                break;
            default:
                throw new InvalidOperationException("Unknown Format");
            }
            return true;
        }
    }
}
