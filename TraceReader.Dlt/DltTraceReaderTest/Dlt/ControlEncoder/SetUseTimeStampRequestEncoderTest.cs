namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SetUseTimeStampRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SetUseTimeStampRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class SetUseTimeStampRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SetUseTimeStampRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode([Values(false, true)] bool enabled)
        {
            SetUseTimeStampRequest request = new(enabled);

            Span<byte> buffer = ControlEncode(request, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x0F));
            Assert.That(buffer[4], Is.EqualTo(enabled ? 1 : 0));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 4, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            SetUseTimeStampRequest request = new(true);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
