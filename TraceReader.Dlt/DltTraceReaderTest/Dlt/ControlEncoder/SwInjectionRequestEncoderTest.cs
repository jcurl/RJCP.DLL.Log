namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SwInjectionRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SwInjectionRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class SwInjectionRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SwInjectionRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode()
        {
            SwInjectionRequest request = new SwInjectionRequest(0x1000, new byte[] { 0x41, 0x42, 0x43, 0x44 });

            Span<byte> buffer = ControlEncode(request, 12);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x1000));
            Assert.That(BitOperations.To32Shift(buffer[4..8], !IsBigEndian), Is.EqualTo(4));
            Assert.That(buffer[8..12].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x42, 0x43, 0x44 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 11, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            SwInjectionRequest request = new SwInjectionRequest(0x1000, new byte[] { 0x41, 0x42, 0x43, 0x44 });

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
