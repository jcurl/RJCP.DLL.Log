namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using Domain.OutputStream;
    using NUnit.Framework;

    [TestFixture]
    public class DltFileTraceFilterDecoderTest
    {
        private static readonly byte[] Data = new byte[] {
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };

        [Test]
        public void WriteOutput()
        {
            MemoryOutput memOutput = new MemoryOutput();
            DltFileTraceFilterDecoder decoder = new DltFileTraceFilterDecoder(memOutput);

            decoder.Decode(Data.AsSpan(), 0);
            Assert.That(memOutput.Lines.Count, Is.EqualTo(1));
            Assert.That(memOutput.Lines[0].Packet.Length, Is.EqualTo(Data.Length));
            Assert.That(memOutput.Lines[0].Line.EcuId, Is.EqualTo("ECU1"));
        }

        [Test]
        public void NullOutput()
        {
            Assert.That(() => {
                _ = new DltFileTraceFilterDecoder(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TextOutput()
        {
            MemoryOutput memOutput = new MemoryOutput(false);
            Assert.That(() => {
                _ = new DltFileTraceFilterDecoder(memOutput);
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
