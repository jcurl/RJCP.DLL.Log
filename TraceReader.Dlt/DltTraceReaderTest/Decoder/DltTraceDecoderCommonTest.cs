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

    [TestFixture(DltFactoryType.Standard)]
    [TestFixture(DltFactoryType.File)]
    [TestFixture(DltFactoryType.Serial)]
    public class DltTraceDecoderCommonTest
    {
        // Tests decoding the same types of messages with different headers. Because of the nature of DLT, we can't test
        // error conditions here as it is implementation defined, and therefore it makes a difference if there is a
        // marker at the start of the packet or not.

        private readonly DltFactory m_Factory;

        public DltTraceDecoderCommonTest(DltFactoryType factoryType)
        {
            m_Factory = new DltFactory(factoryType);
        }

        private static readonly int[] ReadChunks = new[] { 0, 1, 2, 3, 5, 10, 100 };

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacket(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltPacket));

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

                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new int[] { 4, 12, 10, 4, 2, 1, 1, 5, 1, 1, 1, 1 })) {
                    await WriteDltPacket(readStream);
                }
            }
        }

        private async Task WriteDltPacket(Stream stream)
        {
            DltTraceLineBase line;

            using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(stream)) {
                line = await reader.GetLineAsync();
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                Assert.That(line.Count, Is.EqualTo(127));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
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
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append();
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.234), DltType.LOG_WARN, "Warning").Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltPackets));

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
                    m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append(),
                    m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.234), DltType.LOG_WARN, "Warning").Append()
                };
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, plen.ToArray())) {
                    await WriteDltPackets(readStream);
                }
            }
        }

        private async Task WriteDltPackets(Stream stream)
        {
            DltTraceLineBase line;

            using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(stream)) {
                line = await reader.GetLineAsync();
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                Assert.That(line.Count, Is.EqualTo(127));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
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
                Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.556))));
                Assert.That(line.Count, Is.EqualTo(128));
                Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                Assert.That(line.SessionId, Is.EqualTo(50));
                Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.234)));
                Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Warning"));

                Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
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
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Version(2).Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltPacketVersion2));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();

                        // The DLT packet is unknown, so it is skipped, looking for a valid packet, of which none can be
                        // found. See the white paper `DLT.Format.Problems.md` which describes the problems and why
                        // decoding this packet is almost hopeless and that decoding packets after this one is
                        // implementation defined.

                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
                        Assert.That(line.Count, Is.EqualTo(-1));                       // Not available for skipped lines
                        Assert.That(line.EcuId, Is.EqualTo(string.Empty));             // Use the last ECU ID, which there is none
                        Assert.That(line.SessionId, Is.EqualTo(0));                    // Not available for skipped lines
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(new TimeSpan(0)));  // Use the last time stamp
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));     // Not available for skipped lines
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));         // Not available for skipped lines
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped"));

                        Assert.That(line.Features.TimeStamp, Is.False);
                        Assert.That(line.Features.EcuId, Is.False);
                        Assert.That(line.Features.SessionId, Is.False);
                        Assert.That(line.Features.DeviceTimeStamp, Is.False);
                        Assert.That(line.Features.BigEndian, Is.False);
                        Assert.That(line.Features.IsVerbose, Is.True);
                        Assert.That(line.Features.MessageType, Is.False);
                        Assert.That(line.Features.ApplicationId, Is.False);
                        Assert.That(line.Features.ContextId, Is.False);
                    }
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltInvalidDataAtEnd(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append();
                writer.Data(new byte[] { 0x41, 0x41, 0x41, 0x41 });
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltInvalidDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(stream)) {
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.556))));
                        Assert.That(line.Count, Is.EqualTo(127));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                        Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                        Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                        Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
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
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.556))));
                        Assert.That(line.Count, Is.EqualTo(-1));                       // Not available for skipped lines
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));                   // Use the last ECU ID
                        Assert.That(line.SessionId, Is.EqualTo(0));                    // Not available for skipped lines
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));  // Use the last time stamp
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));     // Not available for skipped lines
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));         // Not available for skipped lines
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped 4 bytes"));

                        Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
                        Assert.That(line.Features.EcuId, Is.False);
                        Assert.That(line.Features.SessionId, Is.False);
                        Assert.That(line.Features.DeviceTimeStamp, Is.True);
                        Assert.That(line.Features.BigEndian, Is.False);
                        Assert.That(line.Features.IsVerbose, Is.True);
                        Assert.That(line.Features.MessageType, Is.False);
                        Assert.That(line.Features.ApplicationId, Is.False);
                        Assert.That(line.Features.ContextId, Is.False);

                        line = await reader.GetLineAsync();
                        Assert.That(line, Is.Null);
                    }
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPartialDataAtEnd(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Append();
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.5767), DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2").Append(24);
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltPartialDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(stream)) {
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.556))));
                        Assert.That(line.Count, Is.EqualTo(127));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                        Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                        Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message"));

                        Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
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
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.556))));
                        Assert.That(line.Count, Is.EqualTo(-1));                       // Not available for skipped lines
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));                   // Use the last ECU ID
                        Assert.That(line.SessionId, Is.EqualTo(0));                    // Not available for skipped lines
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));  // Use the last time stamp
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));     // Not available for skipped lines
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));         // Not available for skipped lines
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped 4 bytes"));

                        Assert.That(line.Features.TimeStamp, Is.EqualTo(m_Factory.FactoryType == DltFactoryType.File));
                        Assert.That(line.Features.EcuId, Is.False);
                        Assert.That(line.Features.SessionId, Is.False);
                        Assert.That(line.Features.DeviceTimeStamp, Is.True);
                        Assert.That(line.Features.BigEndian, Is.False);
                        Assert.That(line.Features.IsVerbose, Is.True);
                        Assert.That(line.Features.MessageType, Is.False);
                        Assert.That(line.Features.ApplicationId, Is.False);
                        Assert.That(line.Features.ContextId, Is.False);

                        line = await reader.GetLineAsync();
                        Assert.That(line, Is.Null);
                    }
                }
            }
        }
    }
}
