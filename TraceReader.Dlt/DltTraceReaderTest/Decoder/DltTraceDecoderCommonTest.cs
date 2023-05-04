namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.NonVerbose;
    using Dlt.Packet;
    using IO;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Dlt.Args;

    [TestFixture(DltFactoryType.Standard)]
    [TestFixture(DltFactoryType.File)]
    [TestFixture(DltFactoryType.Serial)]
    public class DltTraceDecoderCommonTest
    {
        // Tests decoding the same types of messages with different headers. Because of the nature of DLT, we can't test
        // error conditions here as it is implementation defined, and therefore it makes a difference if there is a
        // marker at the start of the packet or not.

        private static readonly int[] ReadChunks = { 0, 1, 2, 3, 5, 10, 100 };
        private static readonly int[] ReadChunksMin = { 0, 100 };
        private readonly DltFactoryType m_FactoryType;

        public DltTraceDecoderCommonTest(DltFactoryType factoryType)
        {
            m_FactoryType = factoryType;
        }

        private DltFactory GetFactory() { return new DltFactory(m_FactoryType); }

        private DltFactory GetFactory(IFrameMap map) { return new DltFactory(m_FactoryType, map); }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacket(int maxBytes)
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltPacket));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(factory, readStream);
                }
            }
        }

        [Test]
        public async Task WriteDltPacket()
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Test boundaries of the packet, looking for potential issues
                //  - offset 4 bytes to get the minimum header, with the length
                //  - offset 16 bytes where the standard header is finished, extended header starts
                //  - offset 26 bytes where the first argument starts
                //  - offset 30 bytes payload starts

                factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new[] { 4, 12, 10, 4, 2, 1, 1, 5, 1, 1, 1, 1 })) {
                    await WriteDltPacket(factory, readStream);
                }
            }
        }

        private static Task WriteDltPacket(DltFactory factory, Stream stream)
        {
            return WriteDltPacket(factory, stream, false);
        }

        private static async Task WriteDltPacket(DltFactory factory, Stream stream, bool nv)
        {
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();
                factory.IsLine1(line, 0, 127, nv);
                Assert.That(line.Position, Is.EqualTo(0));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPackets(int maxBytes)
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                List<int> plen = new List<int> {
                    factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append(),
                    factory.Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append()
                };
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltPackets));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPackets(readStream, plen.ToArray());
                }
            }
        }

        [Test]
        public async Task WriteDltPackets()
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Test boundaries of the packet, looking for potential issues
                //  - offset 4 bytes to get the minimum header, with the length
                //  - offset 16 bytes where the standard header is finished, extended header starts
                //  - offset 26 bytes where the first argument starts
                //  - offset 30 bytes payload starts

                List<int> plen = new List<int> {
                    factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append(),
                    factory.Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append()
                };
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, plen.ToArray())) {
                    await WriteDltPackets(readStream, plen.ToArray());
                }
            }
        }

        private async Task WriteDltPackets(Stream stream, int[] plen)
        {
            DltFactory factory = GetFactory();
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();
                factory.IsLine1(line, 0, 127);
                Assert.That(line.Position, Is.EqualTo(0));

                line = await reader.GetLineAsync();
                factory.IsLine2(line, 1, 128);
                Assert.That(line.Position, Is.EqualTo(plen[0]));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPacketVersion2(int maxBytes)
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                int l1 = factory.Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Version(2).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltPacketVersion2));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // The DLT packet is unknown, so it is skipped, looking for a valid packet, of which none can be
                    // found. See the white paper `DLT.Format.Problems.md` which describes the problems and why
                    // decoding this packet is almost hopeless and that decoding packets after this one is
                    // implementation defined.

                    factory.IsSkippedLine(line, DltTime.Default, l1);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltInvalidDataAtEnd(int maxBytes)
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                int p1 = factory.Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append();
                int d = writer.Data(new byte[] { 0x41, 0x41, 0x41, 0x41 });
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltInvalidDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    factory.IsLine2(line, 0, 127);
                    Assert.That(line.Position, Is.EqualTo(0));

                    line = await reader.GetLineAsync();
                    factory.IsSkippedLine(line, DltTestData.Time2, d);
                    Assert.That(line.Position, Is.EqualTo(p1));

                    line = await reader.GetLineAsync();
                    Assert.That(line, Is.Null);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltPartialDataAtEnd(int maxBytes)
        {
            DltFactory factory = GetFactory();
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                int l1 = factory.Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_WARN, "Warning").Append();
                int l2 = factory.Verbose(writer, DltTestData.Time3, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 3").Append(24);
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltPartialDataAtEnd));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    factory.IsLine2(line, 0, 127);
                    Assert.That(line.Position, Is.EqualTo(0));

                    line = await reader.GetLineAsync();
                    factory.IsSkippedLine(line, DltTestData.Time2, l2);
                    Assert.That(line.Position, Is.EqualTo(l1));

                    line = await reader.GetLineAsync();
                    Assert.That(line, Is.Null);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunksMin))]
        [Repeat(5)]
        public async Task RandomData(int maxBytes)
        {
            DltFactory factory = GetFactory();
            // 16MB of random data. The serial and file will pass this very quickly as it only needs to look for a
            // marker. The TCP based encoding must treat every single byte as a valid input, which can be very slow.
            byte[] data = new byte[16 * 1024 * 1024];
            new Random().NextBytes(data);

            using (Stream readStream = new ReadLimitStream(data, maxBytes))
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(readStream)) {
                DltTraceLineBase line;
                do {
                    line = await reader.GetLineAsync();
                } while (line != null);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerbose(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU1", "APP1", "CTX1", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231),
                    new byte[] { 0x01, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x31, 0x00 }
                ).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(factory, readStream, true);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseEcuOverride(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP1", "CTX1", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231),
                    new byte[] { 0x01, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x31, 0x00 }
                ).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(factory, readStream, true);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseExtHdr(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU1", "APP1", "CTX1", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                factory.NonVerboseExt(writer, DltTestData.Time1, DltTime.DeviceTime(1.231),
                    new byte[] { 0x01, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x31, 0x00 }
                ).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(factory, readStream, true);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseNoEcu(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU1", "APP1", "CTX1", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231),
                    new byte[] { 0x01, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x31, 0x00 }
                ).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    await WriteDltPacket(factory, readStream, true);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseNoPayload(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), Array.Empty<byte>()).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.Empty);          // Couldn't decode the message id.
                    Assert.That(nvLine.ApplicationId, Is.Empty);  // Couldn't decode the message id.
                    Assert.That(nvLine.ContextId, Is.Empty);      // Couldn't decode the message id.
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(0));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseNoMessage(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00 }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.Empty);          // Couldn't decode the message id.
                    Assert.That(nvLine.ApplicationId, Is.Empty);  // Couldn't decode the message id.
                    Assert.That(nvLine.ContextId, Is.Empty);      // Couldn't decode the message id.
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(0));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseMessageNoArg(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00 }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.EqualTo("APP2"));
                    Assert.That(nvLine.ContextId, Is.EqualTo("CTX2"));
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(0));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseMessageNoStringArgPayload(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00 }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.EqualTo("APP2"));
                    Assert.That(nvLine.ContextId, Is.EqualTo("CTX2"));
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(2));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseLengthMismatchTooShort(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_SINT32", 4));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00 }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.EqualTo("APP2"));
                    Assert.That(nvLine.ContextId, Is.EqualTo("CTX2"));
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(2));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseLengthMismatchTooLong(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_SINT32", 4));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0xFF }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.EqualTo("APP2"));
                    Assert.That(nvLine.ContextId, Is.EqualTo("CTX2"));
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(5));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseNoMap(int maxBytes)
        {
            DltFactory factory = GetFactory();

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0xFF }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.Empty);
                    Assert.That(nvLine.ApplicationId, Is.Empty);
                    Assert.That(nvLine.ContextId, Is.Empty);
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(5));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseEcuIdFallback(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, null, "APP2", "CTX2", DltType.LOG_INFO, new TestPdu("S_SINT32", 4));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0xFF }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.Empty);
                    Assert.That(nvLine.ApplicationId, Is.EqualTo("APP2"));
                    Assert.That(nvLine.ContextId, Is.EqualTo("CTX2"));
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(5));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseAppIdFallback(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU2", null, null, DltType.LOG_INFO, new TestPdu("S_SINT32", 4));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0xFF }).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.Empty);
                    Assert.That(nvLine.ContextId, Is.Empty);
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(5));
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task WriteDltNonVerboseNoMessageId(int maxBytes)
        {
            TestFrameMap map = new TestFrameMap()
                .Add(1, "ECU1", "APP1", "CTX1", DltType.LOG_INFO, new TestPdu("S_STRG_UTF8", 0));
            DltFactory factory = GetFactory(map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU2", Counter = 127, SessionId = 50
            }) {
                factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231),
                    new byte[] { 0x02, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x31, 0x00 }
                ).Append();
                if (maxBytes == 0) await factory.WriteAsync(writer, nameof(WriteDltNonVerbose));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();

                    // Fallback
                    Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                    DltNonVerboseTraceLine nvLine = (DltNonVerboseTraceLine)line;
                    Assert.That(nvLine.EcuId, Is.EqualTo("ECU2"));
                    Assert.That(nvLine.ApplicationId, Is.Empty);
                    Assert.That(nvLine.ContextId, Is.Empty);
                    Assert.That(nvLine.Arguments.Count, Is.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                    NonVerboseDltArg arg = (NonVerboseDltArg)nvLine.Arguments[0];
                    Assert.That(arg.Data.Length, Is.EqualTo(12));
                }
            }
        }
    }
}
