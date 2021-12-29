namespace RJCP.Diagnostics.Log.Decoder
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
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
        [SetUp]
        public void InitializeTestCase()
        {
            // Required to decode ISO-8859-15 when encoded as ASCII.
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        // Tests decoding the same types of messages with different headers. Because of the nature of DLT, we can't test
        // error conditions here as it is implementation defined, and therefore it makes a difference if there is a
        // marker at the start of the packet or not.

        private static readonly int[] ReadChunks = new[] { 0, 1, 2, 3, 5, 10, 100 };
        private readonly DltFactory m_Factory;

        public DltTraceDecoderCommonTest(DltFactoryType factoryType)
        {
            m_Factory = new DltFactory(factoryType);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacket(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
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

                m_Factory.Generate(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
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
                m_Factory.IsLine1(line, 0, 127);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPackets(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
                m_Factory.Generate(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append();
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
                    m_Factory.Generate(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append(),
                    m_Factory.Generate(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append()
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
                m_Factory.IsLine1(line, 0, 127);

                line = await reader.GetLineAsync();
                m_Factory.IsLine2(line, 1, 128);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacketVersion2(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Version(2).Append();
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

                        m_Factory.IsSkippedLine(line, DltTime.Default);
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
                m_Factory.Generate(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append();
                writer.Data(new byte[] { 0x41, 0x41, 0x41, 0x41 });
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltInvalidDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;
                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();
                        m_Factory.IsLine2(line, 0, 127);

                        line = await reader.GetLineAsync();
                        m_Factory.IsSkippedLine(line, DltTestData.Time2);

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
                m_Factory.Generate(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append();
                m_Factory.Generate(writer, DltTestData.Time3, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 3").Append(24);
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(WriteDltPartialDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;
                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();
                        m_Factory.IsLine2(line, 0, 127);

                        line = await reader.GetLineAsync();
                        m_Factory.IsSkippedLine(line, DltTestData.Time2);

                        line = await reader.GetLineAsync();
                        Assert.That(line, Is.Null);
                    }
                }
            }
        }
    }
}
