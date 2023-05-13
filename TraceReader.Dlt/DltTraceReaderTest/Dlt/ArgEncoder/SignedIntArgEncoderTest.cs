namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class SignedIntArgEncoderTest
    {
        [TestCase(0, true, false, 0x21, 1, TestName = "Encode_LittleEndian_8bitZero")]
        [TestCase(1, true, false, 0x21, 1, TestName = "Encode_LittleEndian_8bitOne")]
        [TestCase(-1, true, false, 0x21, 1, TestName = "Encode_LittleEndian_8bitMinusOne")]
        [TestCase(sbyte.MinValue, true, false, 0x21, 1, TestName = "Encode_LittleEndian_8bitMin")]
        [TestCase(sbyte.MaxValue, true, false, 0x21, 1, TestName = "Encode_LittleEndian_8bitMax")]
        [TestCase(sbyte.MaxValue + 1, true, false, 0x22, 2, TestName = "Encode_LittleEndian_16bit")]
        [TestCase(short.MaxValue, true, false, 0x22, 2, TestName = "Encode_LittleEndian_16bitMax")]
        [TestCase(short.MinValue, true, false, 0x22, 2, TestName = "Encode_LittleEndian_16bitMin")]
        [TestCase(short.MaxValue + 1, true, false, 0x23, 4, TestName = "Encode_LittleEndian_32bit")]
        [TestCase(int.MaxValue, true, false, 0x23, 4, TestName = "Encode_LittleEndian_32bitMax")]
        [TestCase(int.MinValue, true, false, 0x23, 4, TestName = "Encode_LittleEndian_32bitMax")]
        [TestCase((long)int.MaxValue + 1, true, false, 0x24, 8, TestName = "Encode_LittleEndian_64bit")]
        [TestCase(long.MaxValue, true, false, 0x24, 8, TestName = "Encode_LittleEndian_64bitMax")]
        [TestCase(long.MinValue, true, false, 0x24, 8, TestName = "Encode_LittleEndian_64bitMin")]
        [TestCase(0, true, true, 0x21, 1, TestName = "Encode_BigEndian_8bitZero")]
        [TestCase(1, true, true, 0x21, 1, TestName = "Encode_BigEndian_8bitOne")]
        [TestCase(-1, true, true, 0x21, 1, TestName = "Encode_BigEndian_8bitMinusOne")]
        [TestCase(sbyte.MinValue, true, true, 0x21, 1, TestName = "Encode_BigEndian_8bitMin")]
        [TestCase(sbyte.MaxValue, true, true, 0x21, 1, TestName = "Encode_BigEndian_8bitMax")]
        [TestCase(sbyte.MaxValue + 1, true, true, 0x22, 2, TestName = "Encode_BigEndian_16bit")]
        [TestCase(short.MaxValue, true, true, 0x22, 2, TestName = "Encode_BigEndian_16bitMax")]
        [TestCase(short.MinValue, true, true, 0x22, 2, TestName = "Encode_BigEndian_16bitMin")]
        [TestCase(short.MaxValue + 1, true, true, 0x23, 4, TestName = "Encode_BigEndian_32bit")]
        [TestCase(int.MaxValue, true, true, 0x23, 4, TestName = "Encode_BigEndian_32bitMax")]
        [TestCase(int.MinValue, true, true, 0x23, 4, TestName = "Encode_BigEndian_32bitMax")]
        [TestCase((long)int.MaxValue + 1, true, true, 0x24, 8, TestName = "Encode_BigEndian_64bit")]
        [TestCase(long.MaxValue, true, true, 0x24, 8, TestName = "Encode_BigEndian_64bitMax")]
        [TestCase(long.MinValue, true, true, 0x24, 8, TestName = "Encode_BigEndian_64bitMin")]

        [TestCase(0, false, false, 0x21, 1, TestName = "EncodeNv_LittleEndian_8bitZero")]
        [TestCase(1, false, false, 0x21, 1, TestName = "EncodeNv_LittleEndian_8bitOne")]
        [TestCase(-1, false, false, 0x21, 1, TestName = "EncodeNv_LittleEndian_8bitMinusOne")]
        [TestCase(sbyte.MinValue, false, false, 0x21, 1, TestName = "EncodeNv_LittleEndian_8bitMin")]
        [TestCase(sbyte.MaxValue, false, false, 0x21, 1, TestName = "EncodeNv_LittleEndian_8bitMax")]
        [TestCase(sbyte.MaxValue + 1, false, false, 0x22, 2, TestName = "EncodeNv_LittleEndian_16bit")]
        [TestCase(short.MaxValue, false, false, 0x22, 2, TestName = "EncodeNv_LittleEndian_16bitMax")]
        [TestCase(short.MinValue, false, false, 0x22, 2, TestName = "EncodeNv_LittleEndian_16bitMin")]
        [TestCase(short.MaxValue + 1, false, false, 0x23, 4, TestName = "EncodeNv_LittleEndian_32bit")]
        [TestCase(int.MaxValue, false, false, 0x23, 4, TestName = "EncodeNv_LittleEndian_32bitMax")]
        [TestCase(int.MinValue, false, false, 0x23, 4, TestName = "EncodeNv_LittleEndian_32bitMax")]
        [TestCase((long)int.MaxValue + 1, false, false, 0x24, 8, TestName = "EncodeNv_LittleEndian_64bit")]
        [TestCase(long.MaxValue, false, false, 0x24, 8, TestName = "EncodeNv_LittleEndian_64bitMax")]
        [TestCase(long.MinValue, false, false, 0x24, 8, TestName = "EncodeNv_LittleEndian_64bitMin")]
        [TestCase(0, false, true, 0x21, 1, TestName = "EncodeNv_BigEndian_8bitZero")]
        [TestCase(1, false, true, 0x21, 1, TestName = "EncodeNv_BigEndian_8bitOne")]
        [TestCase(-1, false, true, 0x21, 1, TestName = "EncodeNv_BigEndian_8bitMinusOne")]
        [TestCase(sbyte.MinValue, false, true, 0x21, 1, TestName = "EncodeNv_BigEndian_8bitMin")]
        [TestCase(sbyte.MaxValue, false, true, 0x21, 1, TestName = "EncodeNv_BigEndian_8bitMax")]
        [TestCase(sbyte.MaxValue + 1, false, true, 0x22, 2, TestName = "EncodeNv_BigEndian_16bit")]
        [TestCase(short.MaxValue, false, true, 0x22, 2, TestName = "EncodeNv_BigEndian_16bitMax")]
        [TestCase(short.MinValue, false, true, 0x22, 2, TestName = "EncodeNv_BigEndian_16bitMin")]
        [TestCase(short.MaxValue + 1, false, true, 0x23, 4, TestName = "EncodeNv_BigEndian_32bit")]
        [TestCase(int.MaxValue, false, true, 0x23, 4, TestName = "EncodeNv_BigEndian_32bitMax")]
        [TestCase(int.MinValue, false, true, 0x23, 4, TestName = "EncodeNv_BigEndian_32bitMax")]
        [TestCase((long)int.MaxValue + 1, false, true, 0x24, 8, TestName = "EncodeNv_BigEndian_64bit")]
        [TestCase(long.MaxValue, false, true, 0x24, 8, TestName = "EncodeNv_BigEndian_64bitMax")]
        [TestCase(long.MinValue, false, true, 0x24, 8, TestName = "EncodeNv_BigEndian_64bitMin")]
        public void EncodeSignedInt(long value, bool verbose, bool msbf, byte expTypeInfo, int expLen)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + expLen];
            SignedIntDltArg arg = new SignedIntDltArg(value);
            IArgEncoder encoder = new SignedIntArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x00, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            long result;
            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0, expLen);
            switch (expLen) {
            case 1:
                result = unchecked((sbyte)payload[0]);
                break;
            case 2:
                result = BitOperations.To16Shift(payload[0..2], !msbf);
                break;
            case 4:
                result = BitOperations.To32Shift(payload[0..4], !msbf);
                break;
            case 8:
                result = BitOperations.To64Shift(payload[0..8], !msbf);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expLen), "Unsupported length");
            }
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(0, true, false, TestName = "InsufficientBuffer_LittleEndian_8bitZero")]
        [TestCase(256, true, false, TestName = "InsufficientBuffer_LittleEndian_16bit")]
        [TestCase(65536, true, false, TestName = "InsufficientBuffer_LittleEndian_32bit")]
        [TestCase(0x1_00000000, true, false, TestName = "InsufficientBuffer_LittleEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), true, false, TestName = "InsufficientBuffer_LittleEndian_64bitMax")]
        [TestCase(0, true, true, TestName = "InsufficientBuffer_BigEndian_8bitZero")]
        [TestCase(256, true, true, TestName = "InsufficientBuffer_BigEndian_16bit")]
        [TestCase(65536, true, true, TestName = "InsufficientBuffer_BigEndian_32bit")]
        [TestCase(0x1_00000000, true, true, TestName = "InsufficientBuffer_BigEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), true, true, TestName = "InsufficientBuffer_BigEndian_64bitMax")]

        [TestCase(0, false, false, TestName = "InsufficientBufferNv_LittleEndian_8bitZero")]
        [TestCase(256, false, false, TestName = "InsufficientBufferNv_LittleEndian_16bit")]
        [TestCase(65536, false, false, TestName = "InsufficientBufferNv_LittleEndian_32bit")]
        [TestCase(0x1_00000000, false, false, TestName = "InsufficientBufferNv_LittleEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, false, TestName = "InsufficientBufferNv_LittleEndian_64bitMax")]
        [TestCase(0, false, true, TestName = "InsufficientBufferNv_BigEndian_8bitZero")]
        [TestCase(256, false, true, TestName = "InsufficientBufferNv_BigEndian_16bit")]
        [TestCase(65536, false, true, TestName = "InsufficientBufferNv_BigEndian_32bit")]
        [TestCase(0x1_00000000, false, true, TestName = "InsufficientBufferNv_BigEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, true, TestName = "InsufficientBufferNv_BigEndian_64bitMax")]
        public void InsufficientBuffer(long value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0)];
            SignedIntDltArg arg = new SignedIntDltArg(value);
            IArgEncoder encoder = new SignedIntArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
