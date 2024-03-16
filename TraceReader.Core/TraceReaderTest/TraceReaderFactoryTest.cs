namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class TraceReaderFactoryTest
    {
        [Test]
        public async Task NullStream()
        {
            await Assert.ThatAsync(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync((Stream)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task NullFile()
        {
            await Assert.ThatAsync(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task NotFoundFile()
        {
            await Assert.ThatAsync(async () => {
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
