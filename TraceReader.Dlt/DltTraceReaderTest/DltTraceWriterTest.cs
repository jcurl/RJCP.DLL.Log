namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using Encoder;
    using IO;
    using NUnit.Framework;

    [TestFixture]
    public class DltTraceWriterTest
    {
        [Test]
        public void NullStream()
        {
            Assert.That(() => {
                _ = new DltTraceWriter(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullStream2()
        {
            Assert.That(() => {
                _ = new DltTraceWriter(null, new DltTraceEncoder());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NullStream3(bool owner)
        {
            Assert.That(() => {
                _ = new DltTraceWriter(null, new DltTraceEncoder(), owner);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullEncoder()
        {
            using (MemoryStream stream = new MemoryStream()) {
                Assert.That(() => {
                    _ = new DltTraceWriter(stream, null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NullEncoder2(bool owner)
        {
            using (MemoryStream stream = new MemoryStream()) {
                Assert.That(() => {
                    _ = new DltTraceWriter(stream, null, owner);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void ReadOnlyStream()
        {
            byte[] data = new byte[65535];
            using (MemoryStream stream = new MemoryStream(data, false)) {
                Assert.That(() => {
                    _ = new DltTraceWriter(stream);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void StreamOwnershipDefault()
        {
            MemoryStreamDisposed stream = new MemoryStreamDisposed();

            using (stream) {
                using (DltTraceWriter writer = new DltTraceWriter(stream)) {
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
            MemoryStreamDisposed stream = new MemoryStreamDisposed();

            using (stream) {
                using (DltTraceWriter writer = new DltTraceWriter(stream, new DltTraceEncoder(), owner)) {
                    /* Nothing to do, just wait for it to dispose */
                }

                Assert.That(stream.IsDisposed, Is.EqualTo(owner));
            }
        }
    }
}
