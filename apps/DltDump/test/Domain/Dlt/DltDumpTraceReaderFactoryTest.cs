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

    [TestFixture]
    public class DltDumpTraceReaderFactoryTest
    {
        private readonly string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltFileTraceDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltFileTraceFilterDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltTraceDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltTraceDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltNetworkTraceFilterDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltNetworkTraceFilterDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceFilterDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceFilterDecoder>());
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
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

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
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
