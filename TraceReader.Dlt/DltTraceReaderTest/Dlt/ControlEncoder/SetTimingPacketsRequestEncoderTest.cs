namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SetTimingPacketsRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SetTimingPacketsRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    public class SetTimingPacketsRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SetTimingPacketsRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode([Values(false, true)] bool enabled)
        {
            SetTimingPacketsRequest request = new SetTimingPacketsRequest(enabled);

            Span<byte> buffer = ControlEncode(request, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x0B));
            Assert.That(buffer[4], Is.EqualTo(enabled ? 1 : 0));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 4, 1)] int length)
        {
            SetTimingPacketsRequest request = new SetTimingPacketsRequest(true);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
