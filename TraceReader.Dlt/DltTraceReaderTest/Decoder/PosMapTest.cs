namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PosMapTest
    {
        [Test]
        public void DefaultPosition()
        {
            PosMapAccessor posmap = new();
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendSingle()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(100));
        }

        [Test]
        public void AppendZero()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 0);

            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendZeroConsume()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 0);
            posmap.Consume(0);

            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void ConsumeSingleSubsection()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 100);
            posmap.Consume(50);
            Assert.That(posmap.Position, Is.EqualTo(50));
            Assert.That(posmap.Length, Is.EqualTo(50));
        }

        [Test]
        public void ConsumeSingleAll()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 100);
            posmap.Consume(100);
            Assert.That(posmap.Position, Is.EqualTo(100));
            Assert.That(posmap.Length, Is.EqualTo(0));
        }

        [Test]
        public void AppendTwiceConsecutive()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 100);
            posmap.Append(100, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(200));
        }

        [Test]
        public void AppendTwiceGap()
        {
            PosMapAccessor posmap = new();
            posmap.Append(0, 100);
            posmap.Append(200, 100);
            Assert.That(posmap.Position, Is.EqualTo(0));
            Assert.That(posmap.Length, Is.EqualTo(200));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AppendGapRandom(bool overlap)
        {
            // Tests if the position is based on packets, and those packets are not in order, e.g. as obtained by
            // Ethernet and then it needs rejoining after fragmentation.

            PosMapAccessor posmap = new();
            posmap.Append(150, 50);
            posmap.Append(10, 50);
            Assert.That(posmap.Position, Is.EqualTo(150));
            Assert.That(posmap.Length, Is.EqualTo(100));

            posmap.Consume(45);
            Assert.That(posmap.Position, Is.EqualTo(195));
            Assert.That(posmap.Length, Is.EqualTo(55));

            if (overlap) {
                posmap.Consume(10);
                Assert.That(posmap.Position, Is.EqualTo(15));
                Assert.That(posmap.Length, Is.EqualTo(45));
            } else {
                posmap.Consume(5);
                Assert.That(posmap.Position, Is.EqualTo(10));
                Assert.That(posmap.Length, Is.EqualTo(50));

                posmap.Consume(5);
                Assert.That(posmap.Position, Is.EqualTo(15));
                Assert.That(posmap.Length, Is.EqualTo(45));
            }

            posmap.Consume(40);
            Assert.That(posmap.Position, Is.EqualTo(55));
            Assert.That(posmap.Length, Is.EqualTo(5));

            posmap.Consume(5);
            Assert.That(posmap.Position, Is.EqualTo(60));
            Assert.That(posmap.Length, Is.EqualTo(0));

            posmap.Append(275, 25);
            Assert.That(posmap.Position, Is.EqualTo(275));
            Assert.That(posmap.Length, Is.EqualTo(25));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        [TestCase(50)]
        public void AppendTwiceConsecutiveConsumeBytes(int size)
        {
            PosMapAccessor posmap = new();
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
            PosMapAccessor posmap = new();
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
            PosMapAccessor posMap = new(4);
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
            PosMapAccessor posMap = new(4);
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

        [Test]
        public void ConsumeTooMuch()
        {
            PosMapAccessor posMap = new();
#if DEBUG
            Assert.That(() => {
                posMap.Consume(1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
#else
            // Should be ignored in release mode.
            posMap.Consume(1);
#endif
        }
    }
}
