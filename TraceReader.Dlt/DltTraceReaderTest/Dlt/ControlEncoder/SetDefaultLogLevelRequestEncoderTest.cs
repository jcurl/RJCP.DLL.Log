﻿namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SetDefaultLogLevelRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SetDefaultLogLevelRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class SetDefaultLogLevelRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SetDefaultLogLevelRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [TestCase(LogLevel.Default)]
        [TestCase(LogLevel.Undefined)]
        [TestCase(LogLevel.Info)]
        public void Encode(LogLevel level)
        {
            SetDefaultLogLevelRequest request =
                new(level);

            Span<byte> buffer = ControlEncode(request, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x11));
            Assert.That(buffer[4], Is.EqualTo(unchecked((byte)level)));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void EncodeWithCom()
        {
            SetDefaultLogLevelRequest request =
                new(LogLevel.Info, "COM1");

            Span<byte> buffer = ControlEncode(request, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x11));
            Assert.That(buffer[4], Is.EqualTo((byte)LogLevel.Info));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x4F, 0x4D, 0x31 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 8, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            SetDefaultLogLevelRequest request =
                new(LogLevel.Info);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
