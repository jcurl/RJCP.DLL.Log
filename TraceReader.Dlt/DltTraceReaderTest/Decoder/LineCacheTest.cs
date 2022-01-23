namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class LineCacheTest
    {
        // This test case is of course testing an internal detail, but important so as trying to debug this using the
        // public API can be unintuitive. The test cases depend on some constants within the LineCacheAccessor. If those
        // constants change, some test cases might fail, and these test cases need to be updated accordingly.
        private const int MaxPacket = 16 + 65536;
        private const int CacheSize = MaxPacket * 2;

        [Test]
        public void DefaultLength()
        {
            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(cache.CacheLength, Is.EqualTo(0));
        }

        [Test]
        public void Append()
        {
            byte[] buffer = new byte[12] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0a, 0x0b
            };

            LineCacheAccessor cache = new LineCacheAccessor();
            cache.Append(buffer.AsSpan());
            Assert.That(cache.CacheLength, Is.EqualTo(12));
        }

        [Test]
        public void AppendTooLarge()
        {
            byte[] buffer = new byte[CacheSize + 10];
            new Random().NextBytes(buffer);

            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(() => {
                cache.Append(buffer.AsSpan());
            }, Throws.TypeOf<InsufficientMemoryException>());
            Assert.That(cache.CacheLength, Is.EqualTo(0));
        }
    }
}
