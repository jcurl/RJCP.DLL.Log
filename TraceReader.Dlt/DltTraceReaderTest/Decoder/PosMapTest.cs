namespace RJCP.Diagnostics.Log.Decoder
{
    using NUnit.Framework;

    [TestFixture]
    public class PosMapTest
    {
        [Test]
        public void DefaultPosition()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendSingle()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(100));
        }

        [Test]
        public void AppendZero()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 0);

            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendZeroConsume()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 0);
            posmap.Consume(0);

            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void ConsumeSingleSubsection()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 100);
            posmap.Consume(50);
            Assert.That(posmap.Position, Is.EqualTo(50));
            Assert.That(posmap.Length, Is.EqualTo(50));
        }

        [Test]
        public void ConsumeSingleAll()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 100);
            posmap.Consume(100);
            Assert.That(posmap.Position, Is.EqualTo(100));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendTwiceConsecutive()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 100);
            posmap.Append(100, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(200));
        }

        [Test]
        public void AppendTwiceGap()
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 100);
            posmap.Append(200, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(200));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        [TestCase(50)]
        public void AppendTwiceConsecutiveConsumeBytes(int size)
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 90);
            posmap.Append(90, 110);

            for (int i = 0; i < 200; i += size) {
                posmap.Consume(size);
                Assert.That(posmap.Position, Is.EqualTo(i + size));
                Assert.That(posmap.Length, Is.EqualTo(200 - i - size));
            }
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        [TestCase(50)]
        public void AppendTwiceGapConsumeBytes(int size)
        {
            PosMapAccessor posmap = new PosMapAccessor();
            posmap.Append(0, 90);
            posmap.Append(100, 110);

            int expectedPos = 0;
            bool jumped = false;
            for (int i = 0; i < 200; i += size) {
                posmap.Consume(size);
                expectedPos += size;
                if (!jumped && expectedPos >= 90) {
                    jumped = true;
                    expectedPos += 10;
                }
                Assert.That(posmap.Position, Is.EqualTo(expectedPos));
                Assert.That(posmap.Length, Is.EqualTo(200 - i - size));
            }
        }

        [Test]
        public void AppendReallocNoWrap()
        {
            PosMapAccessor posMap = new PosMapAccessor(4);
            posMap.Append(0, 10);
            posMap.Append(20, 10);
            posMap.Append(40, 10);
            posMap.Append(60, 10);
            posMap.Append(80, 10);

            Assert.That(posMap.Position, Is.EqualTo(0));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(1)); Assert.That(posMap.Length, Is.EqualTo(49));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(2)); Assert.That(posMap.Length, Is.EqualTo(48));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(3)); Assert.That(posMap.Length, Is.EqualTo(47));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(4)); Assert.That(posMap.Length, Is.EqualTo(46));
            posMap.Consume(5); Assert.That(posMap.Position, Is.EqualTo(9)); Assert.That(posMap.Length, Is.EqualTo(41));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(20)); Assert.That(posMap.Length, Is.EqualTo(40));
            posMap.Consume(8); Assert.That(posMap.Position, Is.EqualTo(28)); Assert.That(posMap.Length, Is.EqualTo(32));
            posMap.Consume(8); Assert.That(posMap.Position, Is.EqualTo(46)); Assert.That(posMap.Length, Is.EqualTo(24));
            posMap.Consume(10); Assert.That(posMap.Position, Is.EqualTo(66)); Assert.That(posMap.Length, Is.EqualTo(14));
            posMap.Consume(14); Assert.That(posMap.Position, Is.EqualTo(90)); Assert.That(posMap.Length, Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void AppendReallocWrap(int when)
        {
            PosMapAccessor posMap = new PosMapAccessor(4);
            posMap.Append(0, 10);
            if (when == 1) posMap.Consume(10);
            posMap.Append(20, 10);
            if (when == 2) posMap.Consume(10);
            posMap.Append(40, 10);
            if (when == 3) posMap.Consume(10);
            posMap.Append(60, 10);
            if (when == 4) posMap.Consume(10);
            posMap.Append(80, 10);
            if (when == 5) posMap.Consume(10);
            posMap.Append(100, 10);
            if (when == 6) posMap.Consume(10);
            posMap.Append(120, 10);
            if (when == 7) posMap.Consume(10);

            Assert.That(posMap.Position, Is.EqualTo(20));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(21)); Assert.That(posMap.Length, Is.EqualTo(59));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(22)); Assert.That(posMap.Length, Is.EqualTo(58));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(23)); Assert.That(posMap.Length, Is.EqualTo(57));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(24)); Assert.That(posMap.Length, Is.EqualTo(56));
            posMap.Consume(5); Assert.That(posMap.Position, Is.EqualTo(29)); Assert.That(posMap.Length, Is.EqualTo(51));
            posMap.Consume(1); Assert.That(posMap.Position, Is.EqualTo(40)); Assert.That(posMap.Length, Is.EqualTo(50));
            posMap.Consume(8); Assert.That(posMap.Position, Is.EqualTo(48)); Assert.That(posMap.Length, Is.EqualTo(42));
            posMap.Consume(8); Assert.That(posMap.Position, Is.EqualTo(66)); Assert.That(posMap.Length, Is.EqualTo(34));
            posMap.Consume(10); Assert.That(posMap.Position, Is.EqualTo(86)); Assert.That(posMap.Length, Is.EqualTo(24));
            posMap.Consume(14); Assert.That(posMap.Position, Is.EqualTo(120)); Assert.That(posMap.Length, Is.EqualTo(10));
            posMap.Consume(10); Assert.That(posMap.Position, Is.EqualTo(130)); Assert.That(posMap.Length, Is.EqualTo(0));
        }
    }
}
