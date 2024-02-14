namespace RJCP.Diagnostics.Log.Dlt
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class IdHashListTest
    {
        [Test]
        public void IdHashStress()
        {
            IdHashListAccessor idHashList = new();
            Random rand = new();
            int[] idList = GenerateIds();

            int count = 500000;
            for (int i = 0; i < count; i++) {
                int entry = rand.Next(idList.Length);
                string value = idHashList.ParseId(idList[entry]);
                Assert.That(value, Is.Not.Null);
            }
        }

        private static int[] GenerateIds()
        {
            Random rand = new();
            int[] idList = new int[400];
            for (int i = 0; i < idList.Length; i++) {
                for (int j = 0; j < 4; j++) {
                    byte c = (byte)rand.Next(0x1F, 0x7F);
                    if (c >= 0x20) {
                        idList[i] |= unchecked(c << (j * 8));
                    }
                }
            }
            return idList;
        }

        [TestCase(0x61616161, "aaaa")]
        [TestCase(0x41414141, "AAAA")]
        [TestCase(0x41424344, "ABCD")]
        [TestCase(0x00424344, "")]        // MSByte is the first char, so NUL-terminator
        [TestCase(0x41424300, "ABC")]
        [TestCase(0x41420044, "AB")]
        [TestCase(0x41004344, "A")]
        [TestCase(0x20312032, " 1 2")]
        [TestCase(unchecked((int)0xa0a1a2a3), " !\"#")] // The 8th bit is stripped to be ASCII
        public void ParseId(int value, string expected)
        {
            IdHashListAccessor idHashList = new();
            Assert.That(idHashList.ParseId(value), Is.EqualTo(expected));
        }

        [Test]
        public void ParseIdCollision()
        {
            IdHashListAccessor idHashList = new();
            Assert.That(idHashList.ParseId(0x41414242), Is.EqualTo("AABB"));
            Assert.That(idHashList.ParseId(0x42424141), Is.EqualTo("BBAA"));

            // Just to make sure that the same data is returned.
            Assert.That(idHashList.ParseId(0x41414242), Is.EqualTo("AABB"));
            Assert.That(idHashList.ParseId(0x42424141), Is.EqualTo("BBAA"));
        }
    }
}
