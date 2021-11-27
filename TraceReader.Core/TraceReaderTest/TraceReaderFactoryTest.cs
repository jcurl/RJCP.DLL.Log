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
                _ = await new EmptyTracedReaderFactory().CreateAsync((Stream)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullFile()
        {
            Assert.That(async () => {
                _ = await new EmptyTracedReaderFactory().CreateAsync((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NotFoundFile()
        {
            Assert.That(async () => {
                _ = await new EmptyTracedReaderFactory().CreateAsync("FileNotFound.txt");
            }, Throws.TypeOf<FileNotFoundException>());
        }
    }
}
