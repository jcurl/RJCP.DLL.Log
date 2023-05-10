namespace RJCP.Diagnostics.Log
{
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.Args;
    using Dlt.NonVerbose;
    using NUnit.Framework;

    [TestFixture]
    public class DltFileTraceReaderFactoryTest
    {
        private static readonly byte[] FileVerbose = {
            0x44, 0x4C, 0x54, 0x01, 0x19, 0x90, 0xC4, 0x5E, 0x0A, 0x10, 0x0B, 0x00, 0x45, 0x47, 0x55, 0x31, // Storage Header
            0x35, 0x01, 0x00, 0x1E, 0x45, 0x47, 0x55, 0x31, 0x00, 0x01, 0x15, 0x99,                         // Standard Header
            0x21, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,                                     // Extended Header
            0x43, 0x00, 0x00, 0x00, 0xDE, 0xAD, 0xBE, 0xEF                                                  // Verbose Payload
        };

        private static readonly byte[] FileNonVerbose = {
            0x44, 0x4C, 0x54, 0x01, 0x19, 0x90, 0xC4, 0x5E, 0x0A, 0x10, 0x0B, 0x00, 0x45, 0x47, 0x55, 0x31, // Storage Header
            0x34, 0x01, 0x00, 0x14, 0x45, 0x47, 0x55, 0x31, 0x00, 0x01, 0x15, 0x99,                         // Standard Header
            0x01, 0x00, 0x00, 0x00, 0xDE, 0xAD, 0xBE, 0xEF                                                  // Nonverbose Payload
        };

        [Test]
        public async Task FactoryDefault()
        {
            ITraceReaderFactory<DltTraceLineBase> factory = new DltFileTraceReaderFactory();
            using (MemoryStream stream = new MemoryStream(FileVerbose))
            using (ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line, Is.Not.Null);
                Assert.That(line, Is.TypeOf<DltTraceLine>());
                DltTraceLine dltLine = (DltTraceLine)line;

                Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
                Assert.That(dltLine.Arguments[0], Is.TypeOf<UnsignedIntDltArg>());
                UnsignedIntDltArg arg = (UnsignedIntDltArg)dltLine.Arguments[0];
                Assert.That(arg.DataUnsigned, Is.EqualTo(0xEFBEADDEU));
            }
        }

        [Test]
        public async Task FactoryNonVerbose()
        {
            IFrameMap map = new TestFrameMap().Add(1, new TestPdu("S_UINT32", 4));

            ITraceReaderFactory<DltTraceLineBase> factory = new DltFileTraceReaderFactory(map);
            using (MemoryStream stream = new MemoryStream(FileNonVerbose))
            using (ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line, Is.Not.Null);
                Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                DltNonVerboseTraceLine dltLine = (DltNonVerboseTraceLine)line;

                Assert.That(dltLine.MessageId, Is.EqualTo(1));
                Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
                Assert.That(dltLine.Arguments[0], Is.TypeOf<UnsignedIntDltArg>());
                UnsignedIntDltArg arg = (UnsignedIntDltArg)dltLine.Arguments[0];
                Assert.That(arg.DataUnsigned, Is.EqualTo(0xEFBEADDEU));
            }
        }

        [Test]
        public async Task FactoryNonVerboseNullMap()
        {
            ITraceReaderFactory<DltTraceLineBase> factory = new DltFileTraceReaderFactory(null);
            using (MemoryStream stream = new MemoryStream(FileNonVerbose))
            using (ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();
                Assert.That(line, Is.Not.Null);
                Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                DltNonVerboseTraceLine dltLine = (DltNonVerboseTraceLine)line;

                Assert.That(dltLine.MessageId, Is.EqualTo(1));
                Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
                Assert.That(dltLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                NonVerboseDltArg arg = (NonVerboseDltArg)dltLine.Arguments[0];
                Assert.That(arg.Data, Is.EqualTo(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }));
            }
        }
    }
}
