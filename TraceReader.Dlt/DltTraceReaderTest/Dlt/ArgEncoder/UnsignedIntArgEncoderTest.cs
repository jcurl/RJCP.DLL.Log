namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class UnsignedIntArgEncoderTest
    {
        [TestCase(0, true, false, 0x41, 1, TestName = "Encode_LittleEndian_8bitZero")]
        [TestCase(1, true, false, 0x41, 1, TestName = "Encode_LittleEndian_8bitOne")]
        [TestCase(255, true, false, 0x41, 1, TestName = "Encode_LittleEndian_8bitMax")]
        [TestCase(256, true, false, 0x42, 2, TestName = "Encode_LittleEndian_16bit")]
        [TestCase(65535, true, false, 0x42, 2, TestName = "Encode_LittleEndian_16bitMax")]
        [TestCase(65536, true, false, 0x43, 4, TestName = "Encode_LittleEndian_32bit")]
        [TestCase(0xFFFFFFFF, true, false, 0x43, 4, TestName = "Encode_LittleEndian_32bitMax")]
        [TestCase(0x1_00000000, true, false, 0x44, 8, TestName = "Encode_LittleEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), true, false, 0x44, 8, TestName = "Encode_LittleEndian_64bitMax")]
        [TestCase(0, true, true, 0x41, 1, TestName = "Encode_BigEndian_8bitZero")]
        [TestCase(1, true, true, 0x41, 1, TestName = "Encode_BigEndian_8bitOne")]
        [TestCase(255, true, true, 0x41, 1, TestName = "Encode_BigEndian_8bitMax")]
        [TestCase(256, true, true, 0x42, 2, TestName = "Encode_BigEndian_16bit")]
        [TestCase(65535, true, true, 0x42, 2, TestName = "Encode_BigEndian_16bitMax")]
        [TestCase(65536, true, true, 0x43, 4, TestName = "Encode_BigEndian_32bit")]
        [TestCase(0xFFFFFFFF, true, true, 0x43, 4, TestName = "Encode_BigEndian_32bitMax")]
        [TestCase(0x1_00000000, true, true, 0x44, 8, TestName = "Encode_BigEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), true, true, 0x44, 8, TestName = "Encode_BigEndian_64bitMax")]

        [TestCase(0, false, false, 0x41, 1, TestName = "EncodeNv_LittleEndian_8bitZero")]
        [TestCase(1, false, false, 0x41, 1, TestName = "EncodeNv_LittleEndian_8bitOne")]
        [TestCase(255, false, false, 0x41, 1, TestName = "EncodeNv_LittleEndian_8bitMax")]
        [TestCase(256, false, false, 0x42, 2, TestName = "EncodeNv_LittleEndian_16bit")]
        [TestCase(65535, false, false, 0x42, 2, TestName = "EncodeNv_LittleEndian_16bitMax")]
        [TestCase(65536, false, false, 0x43, 4, TestName = "EncodeNv_LittleEndian_32bit")]
        [TestCase(0xFFFFFFFF, false, false, 0x43, 4, TestName = "EncodeNv_LittleEndian_32bitMax")]
        [TestCase(0x1_00000000, false, false, 0x44, 8, TestName = "EncodeNv_LittleEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, false, 0x44, 8, TestName = "EncodeNv_LittleEndian_64bitMax")]
        [TestCase(0, false, true, 0x41, 1, TestName = "EncodeNv_BigEndian_8bitZero")]
        [TestCase(1, false, true, 0x41, 1, TestName = "EncodeNv_BigEndian_8bitOne")]
        [TestCase(255, false, true, 0x41, 1, TestName = "EncodeNv_BigEndian_8bitMax")]
        [TestCase(256, false, true, 0x42, 2, TestName = "EncodeNv_BigEndian_16bit")]
        [TestCase(65535, false, true, 0x42, 2, TestName = "EncodeNv_BigEndian_16bitMax")]
        [TestCase(65536, false, true, 0x43, 4, TestName = "EncodeNv_BigEndian_32bit")]
        [TestCase(0xFFFFFFFF, false, true, 0x43, 4, TestName = "EncodeNv_BigEndian_32bitMax")]
        [TestCase(0x1_00000000, false, true, 0x44, 8, TestName = "EncodeNv_BigEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, true, 0x44, 8, TestName = "EncodeNv_BigEndian_64bitMax")]
        public void EncodeUnsignedInt(long value, bool verbose, bool msbf, byte expTypeInfo, int expLen)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + expLen];
            UnsignedIntDltArg arg = new UnsignedIntDltArg(value);
            IArgEncoder encoder = new UnsignedIntArgEncoder();
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
                result = payload[0];
                break;
            case 2:
                result = unchecked((ushort)BitOperations.To16Shift(payload[0..2], !msbf));
                break;
            case 4:
                result = unchecked((uint)BitOperations.To32Shift(payload[0..4], !msbf));
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
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), true, true, TestName = "EncodeInsufficientBuffer_BigEndian_64bitMax")]

        [TestCase(0, false, false, TestName = "InsufficientBufferNv_LittleEndian_8bitZero")]
        [TestCase(256, false, false, TestName = "InsufficientBufferNv_LittleEndian_16bit")]
        [TestCase(65536, false, false, TestName = "InsufficientBufferNv_LittleEndian_32bit")]
        [TestCase(0x1_00000000, false, false, TestName = "InsufficientBufferNv_LittleEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, false, TestName = "InsufficientBufferNv_LittleEndian_64bitMax")]
        [TestCase(0, false, true, TestName = "InsufficientBufferNv_BigEndian_8bitZero")]
        [TestCase(256, false, true, TestName = "InsufficientBufferNv_BigEndian_16bit")]
        [TestCase(65536, false, true, TestName = "InsufficientBufferNv_BigEndian_32bit")]
        [TestCase(0x1_00000000, false, true, TestName = "InsufficientBufferNv_BigEndian_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), false, true, TestName = "EncodeInsufficientBufferNv_BigEndian_64bitMax")]
        public void InsufficientBuffer(long value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0)];
            UnsignedIntDltArg arg = new UnsignedIntDltArg(value);
            IArgEncoder encoder = new UnsignedIntArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
