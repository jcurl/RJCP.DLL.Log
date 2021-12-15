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
            Assert.That(cache.Length, Is.EqualTo(0));
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
            Assert.That(cache.Length, Is.EqualTo(12));
        }

        [Test]
        public void AppendTooLarge()
        {
            byte[] buffer = new byte[CacheSize + 10];
            new Random().NextBytes(buffer);

            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(cache.Append(buffer.AsSpan()), Is.EqualTo(10));
            Assert.That(cache.Length, Is.EqualTo(CacheSize));

            // The first 10 bytes that overflowed are discarded
            ReadOnlySpan<byte> cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[10..]));
        }

        [Test]
        public void AppendTooLargeSecondOp()
        {
            byte[] buffer = new byte[CacheSize + 10];
            new Random().NextBytes(buffer);

            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(cache.Append(buffer.AsSpan(0, MaxPacket + 50)), Is.EqualTo(0));
            Assert.That(cache.Length, Is.EqualTo(MaxPacket + 50));

            Assert.That(cache.Append(buffer.AsSpan(MaxPacket + 50, buffer.Length - MaxPacket - 50)), Is.EqualTo(10));
            Assert.That(cache.Length, Is.EqualTo(CacheSize));

            // The first 10 bytes that overflowed are discarded
            ReadOnlySpan<byte> cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[10..]));
        }

        [Test]
        public void AppendOverflowCopyRequired()
        {
            byte[] buffer = new byte[CacheSize + 10];
            new Random().NextBytes(buffer);

            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(cache.Append(buffer.AsSpan(0, MaxPacket + 50)), Is.EqualTo(0));
            Assert.That(cache.Length, Is.EqualTo(MaxPacket + 50));

            Assert.That(cache.Consume(MaxPacket), Is.EqualTo(MaxPacket));
            Assert.That(cache.Length, Is.EqualTo(50));

            // No data loss, as we should have enough space
            Assert.That(cache.Append(buffer.AsSpan(MaxPacket + 50, buffer.Length - MaxPacket - 50)), Is.EqualTo(0));
            Assert.That(cache.Length, Is.EqualTo(buffer.Length - MaxPacket));

            // The data should be the data that wasn't consumed
            ReadOnlySpan<byte> cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[MaxPacket..]));
        }

        [Test]
        public void AppendOverflowCopyTooLarge()
        {
            byte[] buffer = new byte[CacheSize * 2];
            new Random().NextBytes(buffer);

            ReadOnlySpan<byte> cached;
            LineCacheAccessor cache = new LineCacheAccessor();
            Assert.That(cache.Append(buffer.AsSpan(0, MaxPacket + 50)), Is.EqualTo(0));
            Assert.That(cache.Length, Is.EqualTo(MaxPacket + 50));
            cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[0..(MaxPacket + 50)]));

            Assert.That(cache.Consume(MaxPacket), Is.EqualTo(MaxPacket));
            Assert.That(cache.Length, Is.EqualTo(50));
            cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[MaxPacket..(MaxPacket + 50)]));

            // The cache has CacheSize - 50 bytes free.   || MaxPacket Free | 50 | MaxPacket-50 Free ||
            // We now add so much data, it should first copy, then discard also.
            Assert.That(cache.Append(buffer.AsSpan(MaxPacket + 50, CacheSize)), Is.EqualTo(50));
            Assert.That(cache.Length, Is.EqualTo(CacheSize));

            // The data should be the data that wasn't consumed
            cached = cache.GetCache();
            Assert.That(cached.ToArray(), Is.EqualTo(buffer[(MaxPacket + 50)..(MaxPacket + 50 + CacheSize)]));
        }
    }
}
