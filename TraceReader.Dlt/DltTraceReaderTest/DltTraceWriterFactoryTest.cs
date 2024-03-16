namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.Args;
    using Encoder;
    using NUnit.Framework;
    using RJCP.CodeQuality.IO;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class DltTraceWriterFactoryTest
    {
        private static readonly byte[] Expected = new byte[] {
            0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
            0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
            0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
        };

        private static readonly IDltLineBuilder Builder = new DltLineBuilder()
            .SetApplicationId("APP1")
            .SetContextId("CTX1")
            .SetDltType(DltType.LOG_INFO)
            .SetBigEndian(false)
            .AddArguments(new IDltArg[] {
                new StringDltArg("Temperature is:"),
                new SignedIntDltArg(45, 2)
            });

        [Test]
        public async Task CreateStream()
        {
            using (MemoryStream stream = new()) {
                ITraceWriterFactory<DltTraceLineBase> writerFactory = new DltTraceWriterFactory();
                using (ITraceWriter<DltTraceLineBase> writer = await writerFactory.CreateAsync(stream)) {
                    Assert.That(await writer.WriteLineAsync(Builder.GetResult()), Is.True);
                }

                byte[] buffer = stream.ReadStream();
                Assert.That(buffer, Is.EqualTo(Expected));
            }
        }

        [Test]
        public async Task CreateStreamNull()
        {
            Stream stream = null;
            ITraceWriterFactory<DltTraceLineBase> writerFactory = new DltTraceWriterFactory();
            await Assert.ThatAsync(async () => {
                _ = await writerFactory.CreateAsync(stream);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task CreateFile()
        {
            using ScratchPad pad = Deploy.ScratchPad();
            string fileName = Path.Combine(pad.Path, "TestFile.dlt");

            ITraceWriterFactory<DltTraceLineBase> writerFactory = new MyWriter();
            using (ITraceWriter<DltTraceLineBase> writer = await writerFactory.CreateAsync(fileName)) {
                Assert.That(await writer.WriteLineAsync(Builder.GetResult()), Is.True);
            }

            using (FileStream file = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                byte[] buffer = file.ReadStream();
                Assert.That(buffer, Is.EqualTo(Expected));
            }
        }

        [Test]
        public async Task CreateFileNull()
        {
            string fileName = null;
            ITraceWriterFactory<DltTraceLineBase> writerFactory = new DltTraceWriterFactory();
            await Assert.ThatAsync(async () => {
                _ = await writerFactory.CreateAsync(fileName);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private class MyWriter : DltTraceWriterFactory
        {
            public MyWriter() : base(new DltTraceEncoderFactory()) { }
        }

        [Test]
        public async Task CustomEncoderFactory()
        {
            using (MemoryStream stream = new()) {
                ITraceWriterFactory<DltTraceLineBase> writerFactory = new MyWriter();
                using (ITraceWriter<DltTraceLineBase> writer = await writerFactory.CreateAsync(stream)) {
                    Assert.That(await writer.WriteLineAsync(Builder.GetResult()), Is.True);
                }

                byte[] buffer = stream.ReadStream();
                Assert.That(buffer, Is.EqualTo(Expected));
            }
        }

        private class MyWriterNull : DltTraceWriterFactory
        {
            public MyWriterNull() : base(null) { }
        }

        [Test]
        public void CustomEncoderFactoryNull()
        {
            Assert.That(() => {
                _ = new MyWriterNull();
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
