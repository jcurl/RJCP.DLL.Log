namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.OutputStream;
    using NUnit.Framework;
    using RJCP.App.DltDump.Infrastructure.IO;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

    [TestFixture]
    public class DltDumpTraceReaderFactoryTest
    {
        private readonly string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        private readonly static byte[] StorageHeader = new byte[] {
            0x44, 0x4C, 0x54, 0x01,     // DLT1
            0xB7, 0xA8, 0xBB, 0x61, 0x20, 0xA0, 0x03, 0x00, // Time stamp
            0x45, 0x43, 0x55, 0x31      // ECU
        };
        private readonly static byte[] SerialHeader = new byte[] { 0x44, 0x4C, 0x53, 0x01 };
        private readonly static byte[] VerbosePayload = new byte[] {
            0x3D,                       // HTYP
            0x7F,                       // MCNT
            0x00, 0x2B,                 // LEN
            0x45, 0x43, 0x55, 0x31,     // ECU
            0x00, 0x00, 0x00, 0x32,     // SEID
            0x00, 0x00, 0x30, 0x16,     // TMSP
            0x41,                       // MSIN
            0x01,                       // NOAR
            0x41, 0x50, 0x50, 0x31,     // APID
            0x43, 0x54, 0x58, 0x31,     // CTID
            0x00, 0x82, 0x00, 0x00,     // Type Info (UTF8 string)
            0x0B, 0x00,                 // String Length
            0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };
        private readonly static byte[] NonVerbosePayload = new byte[] {
            0x3D,                       // HTYP
            0x7F,                       // MCNT
            0x00, 0x2B,                 // LEN
            0x45, 0x43, 0x55, 0x31,     // ECU
            0x00, 0x00, 0x00, 0x32,     // SEID
            0x00, 0x00, 0x30, 0x16,     // TMSP
            0x40,                       // MSIN (NonVerbose)
            0x00,                       // NOAR
            0x41, 0x50, 0x50, 0x31,     // APID
            0x43, 0x54, 0x58, 0x31,     // CTID
            0x01, 0x00, 0x00, 0x00,     // Message Id
            0x0B, 0x00,                 // String Length
            0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x31, 0x00
        };

        private readonly static IFrameMap FrameMap = new TestFrameMap()
            .Add(1, new TestPdu("S_STRG_UTF8", 0));

        private enum TestLineType
        {
            Verbose,
            NonVerbose
        }

        private static Stream GetTestStream(InputFormat streamType, TestLineType lineType)
        {
            MemoryStream stream = new MemoryStream();

            switch (streamType) {
            case InputFormat.Network:
                break;
            case InputFormat.File:
                stream.Write(StorageHeader);
                break;
            case InputFormat.Serial:
                stream.Write(SerialHeader);
                break;
            default:
                throw new ArgumentException("Invalid stream type");
            }

            switch (lineType) {
            case TestLineType.Verbose:
                stream.Write(VerbosePayload);
                break;
            case TestLineType.NonVerbose:
                stream.Write(NonVerbosePayload);
                break;
            default:
                throw new ArgumentException("Invalid line type");
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        // Ensures the factory returns the correct reader instantiating the correct decoder.

        [Test]
        public void NullFileName()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File
            };

            Assert.That(async () => {
                _ = await factory.CreateAsync((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullStream()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File
            };

            Assert.That(async () => {
                _ = await factory.CreateAsync((Stream)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullPacket()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true
            };

            Assert.That(async () => {
                _ = await factory.CreateAsync((IPacket)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task GetFileDecoder()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.File));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltFileTraceDecoder>());
        }

        [Test]
        public async Task GetFileDecoderAndDecodeVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File
            };

            using (Stream stream = GetTestStream(InputFormat.File, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("2021/12/16 21:59:35.237600 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetFileDecoderAndDecodeNonVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.File, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("2021/12/16 21:59:35.237600 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetFileFilterDecoder()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.File));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltFileTraceFilterDecoder>());
        }

        [Test]
        public async Task GetFileFilterDecoderAndDecodeVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File,
                OutputStream = output
            };

            using (Stream stream = GetTestStream(InputFormat.File, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("2021/12/16 21:59:35.237600 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetFileFilterDecoderAndDecodeNonVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File,
                OutputStream = output,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.File, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("2021/12/16 21:59:35.237600 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetTcpDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltTraceDecoder>());
        }

        [Test]
        public async Task GetTcpDecoderOnlineAndDecodeVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Message 00"));
            }
        }

        [Test]
        public async Task GetTcpDecoderOnlineAndDecodeNonVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("[1] Message 01"));
            }
        }

        [Test]
        public async Task GetTcpDecoderOffline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltTraceDecoder>());
        }

        [Test]
        public async Task GetTcpDecoderOfflineAndDecodeVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetTcpDecoderOfflineAndDecodeNonVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetTcpFilterDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltNetworkTraceFilterDecoder>());
        }

        [Test]
        public async Task GetTcpFilterOnlineDecoderAndDecodeVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true,
                OutputStream = output
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.Text, Is.EqualTo("Message 00"));
            }
        }

        [Test]
        public async Task GetTcpFilterOnlineDecoderAndDecodeNonVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true,
                OutputStream = output,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.Text, Is.EqualTo("[1] Message 01"));
            }
        }

        [Test]
        public async Task GetTcpFilterDecoderOffline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltNetworkTraceFilterDecoder>());
        }

        [Test]
        public async Task GetTcpFilterOfflineDecoderAndDecodeVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false,
                OutputStream = output
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetTcpFilterOfflineDecoderAndDecodeNonVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = false,
                OutputStream = output,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Network, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetSerialDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Serial));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceDecoder>());
        }

        [Test]
        public async Task GetSerialDecoderOnlineAndDecodeVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Message 00"));
            }
        }

        [Test]
        public async Task GetSerialDecoderOnlineAndDecodeNonVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("[1] Message 01"));
            }
        }

        [Test]
        public async Task GetSerialDecoderOffline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Serial));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceDecoder>());
        }

        [Test]
        public async Task GetSerialDecoderOfflineAndDecodeVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetSerialDecoderOfflineAndDecodeNonVerbose()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetSerialFilterDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Serial));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceFilterDecoder>());
        }

        [Test]
        public async Task GetSerialFilterOnlineDecoderAndDecodeVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true,
                OutputStream = output
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.Text, Is.EqualTo("Message 00"));
            }
        }

        [Test]
        public async Task GetSerialFilterOnlineDecoderAndDecodeNonVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = true,
                OutputStream = output,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.Text, Is.EqualTo("[1] Message 01"));
            }
        }

        [Test]
        public async Task GetSerialFilterDecoderOffline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Serial));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceFilterDecoder>());
        }

        [Test]
        public async Task GetSerialFilterOfflineDecoderAndDecodeVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false,
                OutputStream = output
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.Verbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info verbose 1 Message 00"));
            }
        }

        [Test]
        public async Task GetSerialFilterOfflineDecoderAndDecodeNonVerbose()
        {
            MemoryOutput output = new MemoryOutput();
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Serial,
                OnlineMode = false,
                OutputStream = output,
                FrameMap = FrameMap
            };

            using (Stream stream = GetTestStream(InputFormat.Serial, TestLineType.NonVerbose)) {
                ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream);
                Assert.That(await reader.GetLineAsync(), Is.Null);
                DltTraceLineBase line = output.Lines[0].Line;
                Assert.That(line.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 1.2310 127 ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 01"));
            }
        }

        [Test]
        public async Task GetPcapDecoder()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Pcap,
                OnlineMode = false,
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Pcap));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltPcapTraceDecoder>());
        }

        [Test]
        public async Task GetPcapDecoderFilterWriter()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Pcap,
                OnlineMode = false,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Pcap));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltPcapTraceDecoder>());
        }

        [Test]
        public async Task GetPcapDecoderFilter()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Pcap,
                OnlineMode = false,
                OutputStream = new MemoryOutput(false)
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Pcap));
            Assert.That(factory.OnlineMode, Is.False);
            Assert.That(factory.OutputStream, Is.Not.Null);

            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor readerAcc = new TraceReaderAccessor(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltPcapTraceDecoder>());
        }

        [Test]
        public async Task GetPacketTcpDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Null);

            IPacket packet = new EmptyPacketReceiver();
            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(packet);

            // This reader is used for packets, and decoders aren't created yet. So we create one to test the type of
            // decoder that will be used.
            TracePacketReaderAccessor readerAcc = new TracePacketReaderAccessor(reader);
            Assert.That(readerAcc.CreateDecoder(), Is.TypeOf<DltTraceDecoder>());
        }

        [Test]
        public async Task GetPacketTcpFilterDecoderOnline()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.Network,
                OnlineMode = true,
                OutputStream = new MemoryOutput()
            };
            Assert.That(factory.InputFormat, Is.EqualTo(InputFormat.Network));
            Assert.That(factory.OnlineMode, Is.True);
            Assert.That(factory.OutputStream, Is.Not.Null);

            IPacket packet = new EmptyPacketReceiver();
            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(packet);

            // This reader is used for packets, and decoders aren't created yet. So we create one to test the type of
            // decoder that will be used.
            TracePacketReaderAccessor readerAcc = new TracePacketReaderAccessor(reader);
            Assert.That(readerAcc.CreateDecoder(), Is.TypeOf<DltNetworkTraceFilterDecoder>());
        }
    }
}
