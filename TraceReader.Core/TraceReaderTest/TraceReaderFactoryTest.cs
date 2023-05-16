namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TraceReaderFactoryTest
    {
        [Test]
        public void NullStream()
        {
            Assert.That(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync((Stream)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullFile()
        {
            Assert.That(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NotFoundFile()
        {
            Assert.That(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync("FileNotFound.txt");
            }, Throws.TypeOf<FileNotFoundException>());
        }

        private class MyReaderNull : TraceReaderFactory<ITraceLine>
        {
            public MyReaderNull() : base(null) { }
        }

        [Test]
        public void CustomEncoderFactoryNull()
        {
            Assert.That(() => {
                _ = new MyReaderNull();
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
