namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture]
    public class BoolArgEncoderTest
    {
        [TestCase(false, true, false, TestName = "Encode_LittleEndian_False")]
        [TestCase(false, true, true, TestName = "Encode_BigEndian_False")]
        [TestCase(true, true, false, TestName = "Encode_LittleEndian_True")]
        [TestCase(true, true, true, TestName = "Encode_BigEndian_True")]

        [TestCase(false, false, false, TestName = "EncodeNv_LittleEndian_False")]
        [TestCase(false, false, true, TestName = "EncodeNv_BigEndian_False")]
        [TestCase(true, false, false, TestName = "EncodeNv_LittleEndian_True")]
        [TestCase(true, false, true, TestName = "EncodeNv_BigEndian_True")]
        public void EncodeBool(bool value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 1];
            BoolDltArg arg = new BoolDltArg(value);
            IArgEncoder encoder = new BoolArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x00, 0x11 } :
                    new byte[] { 0x11, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0, 1);
            Assert.That(payload[0], Is.EqualTo(value ? 1 : 0));
        }

        [TestCase(false, true, false, TestName = "InsufficientBuffer_LittleEndian_False")]
        [TestCase(false, true, true, TestName = "InsufficientBuffer_BigEndian_False")]
        [TestCase(true, true, false, TestName = "InsufficientBuffer_LittleEndian_True")]
        [TestCase(true, true, true, TestName = "InsufficientBuffer_BigEndian_True")]

        [TestCase(false, false, false, TestName = "InsufficientBufferNv_LittleEndian_False")]
        [TestCase(false, false, true, TestName = "InsufficientBufferNv_BigEndian_False")]
        [TestCase(true, false, false, TestName = "InsufficientBufferNv_LittleEndian_True")]
        [TestCase(true, false, true, TestName = "InsufficientBufferNv_BigEndian_True")]
        public void InsufficientBuffer(bool value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0)];
            BoolDltArg arg = new BoolDltArg(value);
            IArgEncoder encoder = new BoolArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
