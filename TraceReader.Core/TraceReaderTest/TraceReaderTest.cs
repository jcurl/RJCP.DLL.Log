namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using Decoder;
    using NUnit.Framework;
    using RJCP.CodeQuality.IO;

    [TestFixture]
    public class TraceReaderTest
    {
        [Test]
        public void WriteOnlyStream()
        {
            Assert.That(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync(new WriteOnlyStream());
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullStream()
        {
            Assert.That(() => {
                _ = new TraceReader<ITraceLine>(null, new TextDecoder());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullDecoder()
        {
            using (MemoryStream stream = new()) {
                Assert.That(() => {
                    _ = new TraceReader<ITraceLine>(stream, null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void NullStreamAndDecoder()
        {
            Assert.That(() => {
                _ = new TraceReader<ITraceLine>(null, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void StreamOwnershipDefault()
        {
            SimpleStream stream = new();

            using (stream) {
                using (TraceReader<ITraceLine> reader = new(stream, new TextDecoder())) {
                    /* Nothing to do, just wait for it to dispose */
                }

                // We own the stream, it shouldn't be disposed
                Assert.That(stream.IsDisposed, Is.False);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StreamOwnership(bool owner)
        {
            SimpleStream stream = new();

            using (stream) {
                using (TraceReader<ITraceLine> reader = new(stream, new TextDecoder(), owner)) {
                    /* Nothing to do, just wait for it to dispose */
                }

                Assert.That(stream.IsDisposed, Is.EqualTo(owner));
            }
        }
    }
}
