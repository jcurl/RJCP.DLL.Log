namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using System.IO;
    using System.Threading.Tasks;
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
    }
}
