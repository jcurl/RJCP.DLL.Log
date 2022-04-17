namespace RJCP.App.DltDump.Domain.Dlt
{
    using System.IO;
    using System.Threading.Tasks;
    using Domain.OutputStream;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class DltTraceReaderFactoryTest
    {
        private readonly string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        // Ensures the factory returns the correct reader instantiating the correct decoder.

        [Test]
        public async Task GetFileDecoder()
        {
            DltDumpTraceReaderFactory factory = new DltDumpTraceReaderFactory() {
                InputFormat = InputFormat.File
            };
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
            ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file);

            TraceReaderAccessor<DltTraceLineBase> readerAcc = new TraceReaderAccessor<DltTraceLineBase>(reader);
            Assert.That(readerAcc.Decoder, Is.TypeOf<DltSerialTraceFilterDecoder>());
        }
    }
}
