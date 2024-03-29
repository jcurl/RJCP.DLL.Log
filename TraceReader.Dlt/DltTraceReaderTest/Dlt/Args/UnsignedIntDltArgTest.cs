﻿namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class UnsignedIntDltArgTest
    {
        [TestCase(0ul, "0")]
        [TestCase(100ul, "100")]
        [TestCase(uint.MaxValue, "4294967295")]
        [TestCase(ushort.MaxValue, "65535")]
        [TestCase(byte.MaxValue, "255")]
        [TestCase(ulong.MaxValue, "18446744073709551615")]
        [TestCase(0x8FFFFFFFFFFFFFFFul, "10376293541461622783")]
        [TestCase(0x7FFFFFFFFFFFFFFFul, "9223372036854775807")]
        [TestCase(0x8FFFFFFFul, "2415919103")]
        [TestCase(0x7FFFFFFFul, "2147483647")]
        [TestCase(0x8FFFul, "36863")]
        [TestCase(0x7FFFul, "32767")]
        [TestCase(0x8Ful, "143")]
        [TestCase(0x7Ful, "127")]
        [TestCase(0xFFul, "255")]
        public void UnsignedUlong(ulong value, string output)
        {
            UnsignedIntDltArg unsignedInt = new(value);
            Assert.That(unsignedInt.ToString(), Is.EqualTo(output));
            Assert.That(unsignedInt.Data, Is.EqualTo(unchecked((long)value)));
            Assert.That(unsignedInt.DataUnsigned, Is.EqualTo(value));

            StringBuilder sb = new();
            Assert.That(unsignedInt.Append(sb).ToString(), Is.EqualTo(output));
        }

        [TestCase(-1, "18446744073709551615")]
        [TestCase(0, "0")]
        [TestCase(100, "100")]
        [TestCase(-2L, "18446744073709551614")]
        [TestCase(2L, "2")]
        [TestCase(long.MaxValue, "9223372036854775807")]
        [TestCase(long.MinValue, "9223372036854775808")]
        [TestCase(uint.MaxValue, "4294967295")]
        [TestCase(ushort.MaxValue, "65535")]
        [TestCase(byte.MaxValue, "255")]
        [TestCase(-8070450532247928833, "10376293541461622783")]
        [TestCase(0x8FFFFFFF, "2415919103")]
        [TestCase(0x7FFFFFFF, "2147483647")]
        [TestCase(0x8FFF, "36863")]
        [TestCase(0x7FFF, "32767")]
        [TestCase(0x8F, "143")]
        [TestCase(0x7F, "127")]
        [TestCase(0xFF, "255")]
        public void UnsignedLong(long value, string output)
        {
            UnsignedIntDltArg unsignedInt = new(value);
            Assert.That(unsignedInt.ToString(), Is.EqualTo(output));
            Assert.That(unsignedInt.Data, Is.EqualTo(value));
            Assert.That(unsignedInt.DataUnsigned, Is.EqualTo(unchecked((ulong)value)));

            StringBuilder sb = new();
            Assert.That(unsignedInt.Append(sb).ToString(), Is.EqualTo(output));
        }

        [TestCase((ulong)0, 1)]
        [TestCase(byte.MaxValue, 1)]
        [TestCase((ulong)byte.MaxValue - 1, 1)]
        [TestCase((ulong)byte.MaxValue + 1, 2)]
        [TestCase(ushort.MaxValue, 2)]
        [TestCase((ulong)ushort.MaxValue - 1, 2)]
        [TestCase((ulong)ushort.MaxValue + 1, 4)]
        [TestCase(uint.MaxValue, 4)]
        [TestCase(uint.MaxValue - 1, 4)]
        [TestCase((ulong)uint.MaxValue + 1, 8)]
        [TestCase(ulong.MaxValue, 8)]
        [TestCase(ulong.MaxValue - 1, 8)]
        public void UnsignedIntSizeFixed(ulong value, int minSize)
        {
            foreach (int size in new[] { 1, 2, 4, 8 }) {
                if (size >= minSize) {
                    UnsignedIntDltArg unsignedInt = new(value, size);
                    Assert.That(unsignedInt.DataUnsigned, Is.EqualTo(value));
                    Assert.That(unsignedInt.DataBytesLength, Is.EqualTo(size));
                }
            }
        }

        [TestCase((ulong)0, 1)]
        [TestCase(byte.MaxValue, 1)]
        [TestCase((ulong)byte.MaxValue - 1, 1)]
        [TestCase((ulong)byte.MaxValue + 1, 2)]
        [TestCase(ushort.MaxValue, 2)]
        [TestCase((ulong)ushort.MaxValue - 1, 2)]
        [TestCase((ulong)ushort.MaxValue + 1, 4)]
        [TestCase(uint.MaxValue, 4)]
        [TestCase(uint.MaxValue - 1, 4)]
        [TestCase((ulong)uint.MaxValue + 1, 8)]
        [TestCase(ulong.MaxValue, 8)]
        [TestCase(ulong.MaxValue - 1, 8)]
        public void UnsignedIntSizeFixedLong(ulong value, int minSize)
        {
            foreach (int size in new[] { 1, 2, 4, 8 }) {
                if (size >= minSize) {
                    UnsignedIntDltArg unsignedInt = new(unchecked((long)value), size);
                    Assert.That(unsignedInt.DataUnsigned, Is.EqualTo(value));
                    Assert.That(unsignedInt.DataBytesLength, Is.EqualTo(size));
                }
            }
        }

        [TestCase((ulong)0, 1)]
        [TestCase(byte.MaxValue, 1)]
        [TestCase((ulong)byte.MaxValue - 1, 1)]
        [TestCase((ulong)byte.MaxValue + 1, 2)]
        [TestCase(ushort.MaxValue, 2)]
        [TestCase((ulong)ushort.MaxValue - 1, 2)]
        [TestCase((ulong)ushort.MaxValue + 1, 4)]
        [TestCase(uint.MaxValue, 4)]
        [TestCase(uint.MaxValue - 1, 4)]
        [TestCase((ulong)uint.MaxValue + 1, 8)]
        [TestCase(ulong.MaxValue, 8)]
        [TestCase(ulong.MaxValue - 1, 8)]
        public void UnsignedIntEstimateSize(ulong value, int expSize)
        {
            UnsignedIntDltArg unsignedInt = new(value);
            Assert.That(unsignedInt.DataUnsigned, Is.EqualTo(value));
            Assert.That(unsignedInt.DataBytesLength, Is.EqualTo(expSize));
        }
    }
}
