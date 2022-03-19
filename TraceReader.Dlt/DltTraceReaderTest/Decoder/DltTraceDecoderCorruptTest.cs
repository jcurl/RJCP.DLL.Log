namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.Packet;
    using IO;
    using NUnit.Framework;

    [TestFixture(DltFactoryType.File)]
    [TestFixture(DltFactoryType.Serial)]
    public class DltTraceDecoderCorruptTest
    {
        // Corrupted packet testing is done on DLT packets with a marker (DLS\1 or DLT\1), as these are more predictable
        // to test (the marker is used to find the next packet, instead of the content of a packet potentially being
        // interpreted as the start of a new packet).

        private static readonly int[] ReadChunks = { 0, 1, 2, 3, 5, 10, 100 };
        private readonly DltFactory m_Factory;

        public DltTraceDecoderCorruptTest(DltFactoryType factoryType)
        {
            m_Factory = new DltFactory(factoryType);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task CorruptedVersionPacketReSync(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1")
                    .Append();
                int l2 = m_Factory
                    .Verbose(writer, DltTestData.Time2, DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message 2")
                    .Version(2).Append();
                m_Factory.Verbose(writer, DltTestData.Time3, DltTime.DeviceTime(1.233), DltType.LOG_INFO, "Message 3")
                    .Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(CorruptedVersionPacketReSync));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    m_Factory.IsLine1(line, 0, 127);

                    // Corrupted data, should be a line indicated data is skipped as a new marker is identified
                    line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(1));
                    m_Factory.IsSkippedLine(line, DltTestData.Time1, l2);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine3(line, 2, 129);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task RandomDataPacketReSync(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1")
                    .Append();
                int r = writer.Random(100);
                m_Factory.Verbose(writer, DltTestData.Time3, DltTime.DeviceTime(1.233), DltType.LOG_INFO, "Message 3")
                    .Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(RandomDataPacketReSync));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    m_Factory.IsLine1(line, 0, 127);

                    // Corrupted data, should be a line indicated data is skipped as a new marker is identified
                    line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(1));
                    m_Factory.IsSkippedLine(line, DltTestData.Time1, r);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine3(line, 2, 128);
                }
            }
        }

        [Test]
        public async Task InvalidLengthTooShort()
        {
            // This test case checks for when the length is too small. We specially craft the message so that it has an
            // extended header, time stamp, session and ECU ID, so the length is expected to be at least 26 bytes, but
            // specify it to be 25 bytes.
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                int l1 = m_Factory
                    .Verbose(writer, DltTestData.Time4, DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message")
                    .Length(25).Append();
                int r = writer.Random(256);
                m_Factory.Verbose(writer, DltTestData.Time5, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2")
                    .Append();
                await m_Factory.WriteAsync(writer, nameof(InvalidLengthTooShort));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new[] { 4, 16, 4, 500 }))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(0));
                    m_Factory.IsSkippedLine(line, DltTime.Default, l1 + r);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine5(line, 1, 128);

                    line = await reader.GetLineAsync();
                    Assert.That(line, Is.Null);
                }
            }
        }

        [Test]
        public async Task InvalidLengthTooLong()
        {
            // By decoding the verbose payload, we check the contents against the packet length. If they don't match,
            // then we consider the packet as invalid. This uses a different heuristic to `InvalidLengthTooShort`, where
            // here the length is still plausible after decoding only the standard header.
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                int l1 = m_Factory
                    .Verbose(writer, DltTestData.Time4, DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message")
                    .Length(46).Append();
                int r = writer.Random(256);
                m_Factory.Verbose(writer, DltTestData.Time5, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2")
                    .Append();
                await m_Factory.WriteAsync(writer, nameof(InvalidLengthTooLong));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new[] { 4, 16, 4, 500 }))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(0));
                    m_Factory.IsSkippedLine(line, DltTestData.Time4, l1 + r);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine5(line, 1, 128);

                    line = await reader.GetLineAsync();
                    Assert.That(line, Is.Null);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task InvalidLengthNoPayload(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Number of arguments is 1, but there is no payload. The packet length is correct. Should result in an invalid packet.
                int l1 = m_Factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.232), DltType.LOG_VERBOSE, 1,
                    Array.Empty<byte>()).Append();
                m_Factory.Verbose(writer, DltTestData.Time5, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2")
                    .Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(0));
                    m_Factory.IsSkippedLine(line, DltTestData.Time1, l1);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine5(line, 1, 128);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadSIntTooSmall(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x25);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadSIntTooBig(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x21);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadSIntSizeUndefined(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x20);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadUIntTooSmall(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x45);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadUIntTooBig(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x41);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadUIntSizeUndefined(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x40);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadFloatTooSmall(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x85);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadFloatTooBig(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x82);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadFloatSizeUndefined(int maxBytes)
        {
            return CorruptPayload32bitNum(maxBytes, 0x80);
        }

        private async Task CorruptPayload32bitNum(int maxBytes, byte typeInfo)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Number of arguments is 1, but there is no payload. The packet length is correct. Should result in an invalid packet.
                int l1 = m_Factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.232), DltType.LOG_VERBOSE, 1,
                    new byte[] { typeInfo, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0x3F }).Append();
                m_Factory.Verbose(writer, DltTestData.Time5, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2")
                    .Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(0));
                    m_Factory.IsSkippedLine(line, DltTestData.Time1, l1);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine5(line, 1, 128);
                }
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadStringLengthTooBig(int maxBytes)
        {
            return CorruptPayloadStringLength(maxBytes, 0x82, 10);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadStringLengthTooSmall(int maxBytes)
        {
            return CorruptPayloadStringLength(maxBytes, 0x82, 5);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadRawLengthTooBig(int maxBytes)
        {
            return CorruptPayloadStringLength(maxBytes, 0x04, 10);
        }

        [TestCaseSource(nameof(ReadChunks))]
        public Task CorruptPayloadRawLengthTooSmall(int maxBytes)
        {
            return CorruptPayloadStringLength(maxBytes, 0x04, 5);
        }

        private async Task CorruptPayloadStringLength(int maxBytes, byte typeInfo, byte length)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                // Number of arguments is 1, but there is no payload. The packet length is correct. Should result in an invalid packet.
                int l1 = m_Factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.232), DltType.LOG_VERBOSE, 1,
                    new byte[] {
                        0x00, typeInfo, 0x00, 0x00, length, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
                    }).Append();
                m_Factory.Verbose(writer, DltTestData.Time5, DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2")
                    .Append();
                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes))
                using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                    DltTraceLineBase line = await reader.GetLineAsync();
                    Assert.That(line.Line, Is.EqualTo(0));
                    m_Factory.IsSkippedLine(line, DltTestData.Time1, l1);

                    line = await reader.GetLineAsync();
                    m_Factory.IsLine5(line, 1, 128);
                }
            }
        }
    }
}