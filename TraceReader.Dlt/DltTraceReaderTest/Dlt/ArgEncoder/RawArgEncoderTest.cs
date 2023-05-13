namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class RawArgEncoderTest
    {
        [TestCase(new byte[] { }, true, false, TestName = "Encode_LittleEndian_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, false, TestName = "Encode_LittleEndian_Raw")]
        [TestCase(new byte[] { }, true, true, TestName = "Encode_BigEndian_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, true, TestName = "Encode_BigEndian_Raw")]

        [TestCase(new byte[] { }, false, false, TestName = "EncodeNv_LittleEndian_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, false, TestName = "EncodeNv_LittleEndian_Raw")]
        [TestCase(new byte[] { }, false, true, TestName = "EncodeNv_BigEndian_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, true, TestName = "EncodeNv_BigEndian_Raw")]
        public void EncodeRaw(byte[] value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 2 + value.Length];
            RawDltArg arg = new RawDltArg(value);
            IArgEncoder encoder = new RawArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0);
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !msbf));
            Assert.That(len, Is.EqualTo(value.Length));
            Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_RawMaxData")]
        [TestCase(true, true, TestName = "Encode_BigEndian_RawMaxData")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_RawMaxData")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_RawMaxData")]
        public void EncodeRawMax(bool verbose, bool msbf)
        {
            byte[] data = new byte[65533 - (verbose ? 4 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeRaw(data, verbose, msbf);
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_RawOverSize")]
        [TestCase(true, true, TestName = "Encode_BigEndian_RawOverSize")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_RawOverSize")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_RawOverSize")]
        public void EncodeRawOverSize(bool verbose, bool msbf)
        {
            byte[] data = new byte[65534 - (verbose ? 4 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            RawDltArg arg = new RawDltArg(data);
            IArgEncoder encoder = new RawArgEncoder();
            Assert.That(encoder.Encode(data, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
