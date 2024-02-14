namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using Domain.OutputStream;
    using NUnit.Framework;

    [TestFixture]
    public class DltSerialTraceFilterDecoderTest
    {
        private static readonly byte[] Data = new byte[] {
            0x44, 0x4C, 0x53, 0x01, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };

        private static readonly byte[] SkipData = new byte[] {
            0x00, 0x00
        };

        [TestCase(false)]
        [TestCase(true)]
        public void WriteOutput(bool online)
        {
            MemoryOutput memOutput = new();
            DltSerialTraceFilterDecoder decoder = new(memOutput, online);

            decoder.Decode(Data.AsSpan(), 0);
            decoder.Flush();
            Assert.That(memOutput.Lines, Has.Count.EqualTo(1));
            Assert.That(memOutput.Lines[0].Packet, Has.Length.EqualTo(Data.Length)); // DltOutput writes the storage header, not memOutput.
            Assert.That(memOutput.Lines[0].Line.EcuId, Is.EqualTo("ECU1"));

            if (online) {
                Assert.That(memOutput.Lines[0].Line.TimeStamp, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-30)));
            } else {
                // 1970/1/1
                Assert.That(memOutput.Lines[0].Line.TimeStamp.Ticks, Is.EqualTo(621355968000000000));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void WriteLineOutput(bool online)
        {
            MemoryOutput memOutput = new();
            DltSerialTraceFilterDecoder decoder = new(memOutput, online);

            decoder.Decode(SkipData.AsSpan(), 0);
            decoder.Flush();
            Assert.That(memOutput.Lines, Has.Count.EqualTo(1));
            Assert.That(memOutput.Lines[0].Packet, Is.Null);
            Assert.That(memOutput.Lines[0].Line.EcuId, Is.EqualTo(""));
            Assert.That(memOutput.Lines[0].Line.ApplicationId, Is.EqualTo("SKIP"));
            Assert.That(memOutput.Lines[0].Line.ContextId, Is.EqualTo("SKIP"));

            if (online) {
                Assert.That(memOutput.Lines[0].Line.TimeStamp, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-30)));
            } else {
                // 1970/1/1
                Assert.That(memOutput.Lines[0].Line.TimeStamp.Ticks, Is.EqualTo(621355968000000000));
            }
        }

        [Test]
        public void NullOutput()
        {
            Assert.That(() => {
                _ = new DltSerialTraceFilterDecoder(null, false);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TextOutput()
        {
            MemoryOutput memOutput = new(false);
            Assert.That(() => {
                _ = new DltSerialTraceFilterDecoder(memOutput, false);
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
