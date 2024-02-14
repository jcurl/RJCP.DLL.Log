namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SetLogLevelRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SetLogLevelRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class SetLogLevelRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SetLogLevelRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [TestCase(LogLevel.Default)]
        [TestCase(LogLevel.Undefined)]
        [TestCase(LogLevel.Info)]
        public void Encode(LogLevel level)
        {
            SetLogLevelRequest request =
                new("APP1", "CTX1", level);

            Span<byte> buffer = ControlEncode(request, 17);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x01));
            Assert.That(buffer[4..8].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[8..12].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
            Assert.That(buffer[12], Is.EqualTo(unchecked((byte)level)));
            Assert.That(buffer[13..17].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void EncodeWithCom()
        {
            SetLogLevelRequest request =
                new("APP1", "CTX1", LogLevel.Block, "COM1");

            Span<byte> buffer = ControlEncode(request, 17);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x01));
            Assert.That(buffer[4..8].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[8..12].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
            Assert.That(buffer[12], Is.EqualTo(unchecked((byte)LogLevel.Block)));
            Assert.That(buffer[13..17].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x4F, 0x4D, 0x31 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 16, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            SetLogLevelRequest request =
                new("APP1", "CTX1", LogLevel.Info);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
