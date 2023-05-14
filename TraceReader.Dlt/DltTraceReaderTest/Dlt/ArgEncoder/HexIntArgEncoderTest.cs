namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(HexIntArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(HexIntArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(HexIntArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(HexIntArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Little, LineType.Verbose)]
    public class HexIntArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public HexIntArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(0, 0x41, 1, TestName = "Encode_8bitZero")]
        [TestCase(1, 0x41, 1, TestName = "Encode_8bitOne")]
        [TestCase(255, 0x41, 1, TestName = "Encode_8bitMax")]
        [TestCase(256, 0x42, 2, TestName = "Encode_16bit")]
        [TestCase(65535, 0x42, 2, TestName = "Encode_16bitMax")]
        [TestCase(65536, 0x43, 4, TestName = "Encode_32bit")]
        [TestCase(0xFFFFFFFF, 0x43, 4, TestName = "Encode_32bitMax")]
        [TestCase(0x1_00000000, 0x44, 8, TestName = "Encode_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), 0x44, 8, TestName = "Encode_64bitMax")]
        public void EncodeHexInt(long value, byte expTypeInfo, int expLen)
        {
            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + expLen];
            HexIntDltArg arg = new HexIntDltArg(value, expLen);
            Assert.That(ArgEncode(buffer, arg), Is.EqualTo(buffer.Length));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x01, 0x80, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x80, 0x01, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            long result;
            Span<byte> payload = buffer.AsSpan(IsVerbose ? 4 : 0, expLen);
            switch (expLen) {
            case 1:
                result = payload[0];
                break;
            case 2:
                result = unchecked((ushort)BitOperations.To16Shift(payload[0..2], !IsBigEndian));
                break;
            case 4:
                result = unchecked((uint)BitOperations.To32Shift(payload[0..4], !IsBigEndian));
                break;
            case 8:
                result = BitOperations.To64Shift(payload[0..8], !IsBigEndian);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expLen), "Unsupported length");
            }
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(0, 1, TestName = "InsufficientBuffer_8bitZero")]
        [TestCase(256, 2, TestName = "InsufficientBuffer_16bit")]
        [TestCase(65536, 4, TestName = "InsufficientBuffer_32bit")]
        [TestCase(0x1_00000000, 8, TestName = "InsufficientBuffer_64bit")]
        [TestCase(unchecked((long)0xFFFFFFFF_FFFFFFFF), 8, TestName = "InsufficientBuffer_64bitMax")]
        public void InsufficientBuffer(long value, int len)
        {
            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + len - 1];
            HexIntDltArg arg = new HexIntDltArg(value, len);
            Assert.That(ArgEncode(buffer, arg), Is.EqualTo(-1));
        }
    }
}
