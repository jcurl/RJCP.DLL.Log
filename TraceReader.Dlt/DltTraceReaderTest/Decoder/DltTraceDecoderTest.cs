namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.Packet;
    using IO;
    using NUnit.Framework;

    [TestFixture]
    public class DltTraceDecoderTest
    {
        private static readonly int[] ReadChunks = new[] { 0, 1, 2, 3, 5, 10, 100 };

        private static TimeSpan Time(double seconds)
        {
            return new TimeSpan((long)(seconds * TimeSpan.TicksPerSecond));
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacket(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                writer.Verbose().Line(Time(1.232), DltType.LOG_INFO, "Message").Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(readStream);
                }
            }
        }

        [Test]
        public async Task WriteDltPacket()
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Test boundaries of the packet, looking for potential issues
                //  - offset 4 bytes to get the minimum header, with the length
                //  - offset 16 bytes where the standard header is finished, extended header starts
                //  - offset 26 bytes where the first argument starts
                //  - offset 30 bytes payload starts

                writer.Verbose().Line(Time(1.232), DltType.LOG_INFO, "Message").Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new int[] { 4, 12, 10, 4, 2, 1, 1, 5, 1, 1, 1, 1 })) {
                    await WriteDltPacket(readStream);
                }
            }
        }

        private static async Task WriteDltPacket(Stream stream)
        {
            DltTraceLineBase line;

            using (ITraceReader<DltTraceLineBase> reader = await new DltTraceReaderFactory().CreateAsync(stream)) {
                line = await reader.GetLineAsync();
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.TimeStamp, Is.EqualTo(new DateTime(1970, 1, 1)));
                Assert.That(line.Count, Is.EqualTo(127));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(Time(1.232)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                Assert.That(line.Features.TimeStamp, Is.False);       // This format can't decode a storage header
                Assert.That(line.Features.EcuId, Is.True);
                Assert.That(line.Features.SessionId, Is.True);
                Assert.That(line.Features.DeviceTimeStamp, Is.True);
                Assert.That(line.Features.BigEndian, Is.False);
                Assert.That(line.Features.IsVerbose, Is.True);
                Assert.That(line.Features.MessageType, Is.True);
                Assert.That(line.Features.ApplicationId, Is.True);
                Assert.That(line.Features.ContextId, Is.True);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPackets(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                writer.Verbose().Line(Time(1.232), DltType.LOG_INFO, "Message").Append();
                writer.Verbose().Line(Time(1.234), DltType.LOG_WARN, "Warning").Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPackets(readStream);
                }
            }
        }

        [Test]
        public async Task WriteDltPackets()
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Test boundaries of the packet, looking for potential issues
                //  - offset 4 bytes to get the minimum header, with the length
                //  - offset 16 bytes where the standard header is finished, extended header starts
                //  - offset 26 bytes where the first argument starts
                //  - offset 30 bytes payload starts

                List<int> plen = new List<int> {
                    writer.Verbose().Line(Time(1.232), DltType.LOG_INFO, "Message").Append(),
                    writer.Verbose().Line(Time(1.234), DltType.LOG_WARN, "Warning").Append()
                };
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, plen.ToArray())) {
                    await WriteDltPackets(readStream);
                }
            }
        }

        private static async Task WriteDltPackets(Stream stream)
        {
            DltTraceLineBase line;

            using (ITraceReader<DltTraceLineBase> reader = await new DltTraceReaderFactory().CreateAsync(stream)) {
                line = await reader.GetLineAsync();
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.TimeStamp, Is.EqualTo(new DateTime(1970, 1, 1)));
                Assert.That(line.Count, Is.EqualTo(127));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(Time(1.232)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                Assert.That(line.Features.TimeStamp, Is.False);       // This format can't decode a storage header
                Assert.That(line.Features.EcuId, Is.True);
                Assert.That(line.Features.SessionId, Is.True);
                Assert.That(line.Features.DeviceTimeStamp, Is.True);
                Assert.That(line.Features.BigEndian, Is.False);
                Assert.That(line.Features.IsVerbose, Is.True);
                Assert.That(line.Features.MessageType, Is.True);
                Assert.That(line.Features.ApplicationId, Is.True);
                Assert.That(line.Features.ContextId, Is.True);

                line = await reader.GetLineAsync();
                Assert.That(line.Line, Is.EqualTo(1));
                Assert.That(line.TimeStamp, Is.EqualTo(new DateTime(1970, 1, 1)));
                Assert.That(line.Count, Is.EqualTo(128));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(Time(1.234)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Warning"));

                Assert.That(line.Features.TimeStamp, Is.False);       // This format can't decode a storage header
                Assert.That(line.Features.EcuId, Is.True);
                Assert.That(line.Features.SessionId, Is.True);
                Assert.That(line.Features.DeviceTimeStamp, Is.True);
                Assert.That(line.Features.BigEndian, Is.False);
                Assert.That(line.Features.IsVerbose, Is.True);
                Assert.That(line.Features.MessageType, Is.True);
                Assert.That(line.Features.ApplicationId, Is.True);
                Assert.That(line.Features.ContextId, Is.True);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacketVersion2(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                writer.Verbose().Line(Time(1.232), DltType.LOG_INFO, "Message").Version(2).Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await new DltTraceReaderFactory().CreateAsync(readStream)) {
                        line = await reader.GetLineAsync();

                        // The DLT packet is unknown, so it is skipped, looking for a valid packet, of which none can be
                        // found. See the white paper `DLT.Format.Problems.md` which describes the problems and why
                        // decoding this packet is almost hopeless and that decoding packets after this one is
                        // implementation defined.

                        // TODO: This will change when skipped bytes are implemented in flush, as we should get a line
                        // indicating skipped data.
                        Assert.That(line, Is.Null);
                    }
                }
            }
        }
    }
}
