﻿namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class HexIntDltArgTest
    {
        [TestCase(0, 1, "0x00")]
        [TestCase(0, 2, "0x0000")]
        [TestCase(0, 4, "0x00000000")]
        [TestCase(0, 8, "0x0000000000000000")]
        [TestCase(100, 1, "0x64")]
        [TestCase(100, 2, "0x0064")]
        [TestCase(100, 4, "0x00000064")]
        [TestCase(100, 8, "0x0000000000000064")]
        [TestCase(uint.MaxValue, 1, "0xffffffff")]
        [TestCase(uint.MaxValue, 2, "0xffffffff")]
        [TestCase(uint.MaxValue, 4, "0xffffffff")]
        [TestCase(uint.MaxValue, 8, "0x00000000ffffffff")]
        [TestCase(ushort.MaxValue, 1, "0xffff")]
        [TestCase(ushort.MaxValue, 2, "0xffff")]
        [TestCase(ushort.MaxValue, 4, "0x0000ffff")]
        [TestCase(ushort.MaxValue, 8, "0x000000000000ffff")]
        [TestCase(byte.MaxValue, 1, "0xff")]
        [TestCase(byte.MaxValue, 2, "0x00ff")]
        [TestCase(byte.MaxValue, 4, "0x000000ff")]
        [TestCase(byte.MaxValue, 8, "0x00000000000000ff")]
        [TestCase(unchecked((long)ulong.MaxValue), 1, "0xffffffffffffffff")]
        [TestCase(unchecked((long)ulong.MaxValue), 2, "0xffffffffffffffff")]
        [TestCase(unchecked((long)ulong.MaxValue), 4, "0xffffffffffffffff")]
        [TestCase(unchecked((long)ulong.MaxValue), 8, "0xffffffffffffffff")]
        [TestCase(unchecked((long)0x8FFFFFFFFFFFFFFFUL), 1, "0x8fffffffffffffff")]
        [TestCase(unchecked((long)0x8FFFFFFFFFFFFFFFUL), 2, "0x8fffffffffffffff")]
        [TestCase(unchecked((long)0x8FFFFFFFFFFFFFFFUL), 4, "0x8fffffffffffffff")]
        [TestCase(unchecked((long)0x8FFFFFFFFFFFFFFFUL), 8, "0x8fffffffffffffff")]
        [TestCase(0x7FFFFFFFFFFFFFFFL, 1, "0x7fffffffffffffff")]
        [TestCase(0x7FFFFFFFFFFFFFFFL, 2, "0x7fffffffffffffff")]
        [TestCase(0x7FFFFFFFFFFFFFFFL, 4, "0x7fffffffffffffff")]
        [TestCase(0x7FFFFFFFFFFFFFFFL, 8, "0x7fffffffffffffff")]
        [TestCase(0x8FFFFFFFL, 1, "0x8fffffff")]
        [TestCase(0x8FFFFFFFL, 2, "0x8fffffff")]
        [TestCase(0x8FFFFFFFL, 4, "0x8fffffff")]
        [TestCase(0x8FFFFFFFL, 8, "0x000000008fffffff")]
        [TestCase(0x7FFFFFFFL, 1, "0x7fffffff")]
        [TestCase(0x7FFFFFFFL, 2, "0x7fffffff")]
        [TestCase(0x7FFFFFFFL, 4, "0x7fffffff")]
        [TestCase(0x7FFFFFFFL, 8, "0x000000007fffffff")]
        [TestCase(0x8FFFL, 1, "0x8fff")]
        [TestCase(0x8FFFL, 2, "0x8fff")]
        [TestCase(0x8FFFL, 4, "0x00008fff")]
        [TestCase(0x8FFFL, 8, "0x0000000000008fff")]
        [TestCase(0x7FFFL, 1, "0x7fff")]
        [TestCase(0x7FFFL, 2, "0x7fff")]
        [TestCase(0x7FFFL, 4, "0x00007fff")]
        [TestCase(0x7FFFL, 8, "0x0000000000007fff")]
        [TestCase(0x8FL, 1, "0x8f")]
        [TestCase(0x8FL, 2, "0x008f")]
        [TestCase(0x8FL, 4, "0x0000008f")]
        [TestCase(0x8FL, 8, "0x000000000000008f")]
        [TestCase(0x7FL, 1, "0x7f")]
        [TestCase(0x7FL, 2, "0x007f")]
        [TestCase(0x7FL, 4, "0x0000007f")]
        [TestCase(0x7FL, 8, "0x000000000000007f")]
        [TestCase(0xFFL, 1, "0xff")]
        [TestCase(0xFFL, 2, "0x00ff")]
        [TestCase(0xFFL, 4, "0x000000ff")]
        [TestCase(0xFFL, 8, "0x00000000000000ff")]
        public void LongHexValue(long value, int length, string output)
        {
            HexIntDltArg hexArg = new(value, length);
            Assert.That(hexArg.ToString(), Is.EqualTo(output));
            Assert.That(hexArg.Data, Is.EqualTo(value));

            StringBuilder sb = new();
            Assert.That(hexArg.Append(sb).ToString(), Is.EqualTo(output));
        }
    }
}
