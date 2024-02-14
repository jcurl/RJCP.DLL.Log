namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class SignedIntDltArgTest
    {
        [TestCase(-100, "-100")]
        [TestCase(0, "0")]
        [TestCase(100, "100")]
        [TestCase(256, "256")]
        [TestCase(byte.MaxValue, "255")]
        [TestCase(byte.MinValue, "0")]
        [TestCase(32768, "32768")]
        [TestCase(short.MaxValue, "32767")]
        [TestCase(short.MinValue, "-32768")]
        [TestCase(2147483648, "2147483648")]
        [TestCase(int.MaxValue, "2147483647")]
        [TestCase(int.MinValue, "-2147483648")]
        [TestCase(-2147483649, "-2147483649")]
        [TestCase(-9223372036854775807, "-9223372036854775807")]
        [TestCase(9223372036854775807, "9223372036854775807")]
        public void SignedIntToString(long value, string output)
        {
            SignedIntDltArg signedInt = new(value);
            Assert.That(signedInt.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(signedInt.Append(sb).ToString(), Is.EqualTo(output));
        }

        [TestCase(0, 1)]
        [TestCase(sbyte.MinValue, 1)]
        [TestCase(sbyte.MaxValue, 1)]
        [TestCase(sbyte.MinValue - 1, 2)]
        [TestCase(sbyte.MaxValue + 1, 2)]
        [TestCase(short.MinValue, 2)]
        [TestCase(short.MaxValue, 2)]
        [TestCase(short.MinValue - 1, 4)]
        [TestCase(short.MaxValue + 1, 4)]
        [TestCase(int.MinValue, 4)]
        [TestCase(int.MaxValue, 4)]
        [TestCase((long)int.MinValue - 1, 8)]
        [TestCase((long)int.MaxValue + 1, 8)]
        [TestCase(long.MinValue, 8)]
        [TestCase(long.MaxValue, 8)]
        public void SignedIntSizeFixed(long value, int minSize)
        {
            foreach (int size in new[] { 1, 2, 4, 8 }) {
                if (size >= minSize) {
                    SignedIntDltArg signedInt = new(value, size);
                    Assert.That(signedInt.Data, Is.EqualTo(value));
                    Assert.That(signedInt.DataBytesLength, Is.EqualTo(size));
                }
            }
        }

        [TestCase(0, 1)]
        [TestCase(sbyte.MinValue, 1)]
        [TestCase(sbyte.MaxValue, 1)]
        [TestCase(sbyte.MinValue - 1, 2)]
        [TestCase(sbyte.MaxValue + 1, 2)]
        [TestCase(short.MinValue, 2)]
        [TestCase(short.MaxValue, 2)]
        [TestCase(short.MinValue - 1, 4)]
        [TestCase(short.MaxValue + 1, 4)]
        [TestCase(int.MinValue, 4)]
        [TestCase(int.MaxValue, 4)]
        [TestCase((long)int.MinValue - 1, 8)]
        [TestCase((long)int.MaxValue + 1, 8)]
        [TestCase(long.MinValue, 8)]
        [TestCase(long.MaxValue, 8)]
        public void SignedIntEstimateSize(long value, int expSize)
        {
            SignedIntDltArg signedInt = new(value);
            Assert.That(signedInt.Data, Is.EqualTo(value));
            Assert.That(signedInt.DataBytesLength, Is.EqualTo(expSize));
        }
    }
}
