namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class NonVerboseArgEncoderTest
    {
        [TestCase(new byte[] { }, true, false, TestName = "Encode_LittleEndian_NVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, false, TestName = "Encode_LittleEndian_NV")]
        [TestCase(new byte[] { }, true, true, TestName = "Encode_BigEndian_NVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, true, TestName = "Encode_BigEndian_NV")]

        [TestCase(new byte[] { }, false, false, TestName = "EncodeNv_LittleEndian_NVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, false, TestName = "EncodeNv_LittleEndian_NV")]
        [TestCase(new byte[] { }, false, true, TestName = "EncodeNv_BigEndian_NVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, true, TestName = "EncodeNv_BigEndian_NV")]
        public void EncodeNvBytes(byte[] value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 6 : 0) + value.Length];
            NonVerboseDltArg arg = new NonVerboseDltArg(value);
            IArgEncoder encoder = new NonVerboseArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));

                Span<byte> payload = buffer.AsSpan(4);
                int len = unchecked((ushort)BitOperations.To16Shift(payload, !msbf));
                Assert.That(len, Is.EqualTo(value.Length));
                Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
            } else {
                Span<byte> payload = buffer.AsSpan();
                Assert.That(payload.ToArray(), Is.EqualTo(value));
            }
        }

        [TestCase(new byte[] { }, true, false, TestName = "Encode_LittleEndian_UNVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, false, TestName = "Encode_LittleEndian_UNV")]
        [TestCase(new byte[] { }, true, true, TestName = "Encode_BigEndian_UNVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, true, true, TestName = "Encode_BigEndian_UNV")]

        [TestCase(new byte[] { }, false, false, TestName = "EncodeNv_LittleEndian_UNVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, false, TestName = "EncodeNv_LittleEndian_UNV")]
        [TestCase(new byte[] { }, false, true, TestName = "EncodeNv_BigEndian_UNVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, false, true, TestName = "EncodeNv_BigEndian_UNV")]
        public void EncodeUNvBytes(byte[] value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 6 : 0) + value.Length];
            UnknownNonVerboseDltArg arg = new UnknownNonVerboseDltArg(value);
            IArgEncoder encoder = new NonVerboseArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));

                Span<byte> payload = buffer.AsSpan(4);
                int len = unchecked((ushort)BitOperations.To16Shift(payload, !msbf));
                Assert.That(len, Is.EqualTo(value.Length));
                Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
            } else {
                Span<byte> payload = buffer.AsSpan();
                Assert.That(payload.ToArray(), Is.EqualTo(value));
            }
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_NVMaxData")]
        [TestCase(true, true, TestName = "Encode_BigEndian_NVMaxData")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_NVMaxData")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_NVMaxData")]
        public void EncodeNvBytesMax(bool verbose, bool msbf)
        {
            byte[] data = new byte[65535 - (verbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeNvBytes(data, verbose, msbf);
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_NVOverSize")]
        [TestCase(true, true, TestName = "Encode_BigEndian_NVOverSize")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_NVOverSize")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_NVOverSize")]
        public void EncodeNvBytesOverSize(bool verbose, bool msbf)
        {
            byte[] data = new byte[65536 - (verbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            NonVerboseDltArg arg = new NonVerboseDltArg(data);
            IArgEncoder encoder = new NonVerboseArgEncoder();
            Assert.That(encoder.Encode(data, verbose, msbf, arg), Is.EqualTo(-1));
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_UNVMaxData")]
        [TestCase(true, true, TestName = "Encode_BigEndian_UNVMaxData")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_UNVMaxData")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_UNVMaxData")]
        public void EncodeUNvBytesMax(bool verbose, bool msbf)
        {
            byte[] data = new byte[65535 - (verbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeUNvBytes(data, verbose, msbf);
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_UNVOverSize")]
        [TestCase(true, true, TestName = "Encode_BigEndian_UNVOverSize")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_UNVOverSize")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_UNVOverSize")]
        public void EncodeUNvBytesOverSize(bool verbose, bool msbf)
        {
            byte[] data = new byte[65536 - (verbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            UnknownNonVerboseDltArg arg = new UnknownNonVerboseDltArg(data);
            IArgEncoder encoder = new NonVerboseArgEncoder();
            Assert.That(encoder.Encode(data, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
