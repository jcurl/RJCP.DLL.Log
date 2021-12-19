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

        private readonly DltFactory m_Factory;

        public DltTraceDecoderCorruptTest(DltFactoryType factoryType)
        {
            m_Factory = new DltFactory(factoryType);
        }

        private static readonly int[] ReadChunks = new[] { 0, 1, 2, 3, 5, 10, 100 };

        [TestCaseSource(nameof(ReadChunks))]
        public async Task CorruptedVersionPacketReSync(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2377), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message 2").Version(2).Append();
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2378), DltTime.DeviceTime(1.233), DltType.LOG_INFO, "Message 3").Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(CorruptedVersionPacketReSync));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                        Assert.That(line.Count, Is.EqualTo(127));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.231)));
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

                        // Corrupted data, should be a line indicated data is skipped as a new marker is identified
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(1));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                        Assert.That(line.Count, Is.EqualTo(-1));                       // Not available for skipped lines
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));                   // Use the last ECU ID
                        Assert.That(line.SessionId, Is.EqualTo(0));                    // Not available for skipped lines
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.231)));  // Use the last time stamp
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));     // Not available for skipped lines
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));         // Not available for skipped lines
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped"));

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
                        Assert.That(line.Line, Is.EqualTo(2));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2378))));
                        Assert.That(line.Count, Is.EqualTo(129));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.233)));
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
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task RandomDataPacketReSync(int maxBytes)
        {
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376), DltTime.DeviceTime(1.231), DltType.LOG_INFO, "Message 1").Append();
                writer.Random(100);
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.2378), DltTime.DeviceTime(1.233), DltType.LOG_INFO, "Message 3").Append();
                if (maxBytes == 0) await m_Factory.WriteAsync(writer, nameof(RandomDataPacketReSync));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, maxBytes)) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                        Assert.That(line.Count, Is.EqualTo(127));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.231)));
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

                        // Corrupted data, should be a line indicated data is skipped as a new marker is identified
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(1));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376))));
                        Assert.That(line.Count, Is.EqualTo(-1));                       // Not available for skipped lines
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));                   // Use the last ECU ID
                        Assert.That(line.SessionId, Is.EqualTo(0));                    // Not available for skipped lines
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.231)));  // Use the last time stamp
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));     // Not available for skipped lines
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));         // Not available for skipped lines
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped"));

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
                        Assert.That(line.Line, Is.EqualTo(2));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.2378))));
                        Assert.That(line.Count, Is.EqualTo(128));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.233)));
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
            }
        }

        [Test]
        public async Task InvalidLength()
        {
            // This test case checks for when the length is too small. We specially craft the message so that it has an
            // extended header, time stamp, session and ECU ID, so the length is expected to be at least 26 bytes, but
            // specify it to be 25 bytes.
            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.556), DltTime.DeviceTime(1.232), DltType.LOG_INFO, "Message").Length(25).Append();
                writer.Random(256);
                m_Factory.Generate(writer, DltTime.FileTime(2021, 12, 16, 20, 59, 35.5767), DltTime.DeviceTime(1.3), DltType.LOG_INFO, "Message 2").Append();
                await m_Factory.WriteAsync(writer, nameof(InvalidLength));

                using (Stream stream = writer.Stream())
                using (Stream readStream = new ReadLimitStream(stream, new int[] { 4, 16, 4, 500 })) {
                    DltTraceLineBase line;

                    using (ITraceReader<DltTraceLineBase> reader = await m_Factory.DltReaderFactory(readStream)) {
                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(0));
                        Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
                        Assert.That(line.Count, Is.EqualTo(-1));
                        Assert.That(line.EcuId, Is.EqualTo(string.Empty));
                        Assert.That(line.SessionId, Is.EqualTo(0));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(new TimeSpan(0)));
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
                        Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));
                        Assert.That(line.ContextId, Is.EqualTo(string.Empty));
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

                        line = await reader.GetLineAsync();
                        Assert.That(line.Line, Is.EqualTo(1));
                        Assert.That(line.TimeStamp, Is.EqualTo(m_Factory.ExpectedTimeStamp(DltTime.FileTime(2021, 12, 16, 20, 59, 35.5767))));
                        Assert.That(line.Count, Is.EqualTo(128));
                        Assert.That(line.EcuId, Is.EqualTo("ECU1"));
                        Assert.That(line.SessionId, Is.EqualTo(50));
                        Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.3)));
                        Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
                        Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
                        Assert.That(line.ContextId, Is.EqualTo("CTX1"));
                        // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Message 2"));

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
                        Assert.That(line, Is.Null);
                    }
                }
            }
        }
    }
}
