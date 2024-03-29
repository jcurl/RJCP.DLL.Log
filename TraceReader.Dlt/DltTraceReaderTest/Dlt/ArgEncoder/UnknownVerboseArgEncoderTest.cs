﻿namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(UnknownVerboseArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(UnknownVerboseArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceWriter, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceWriter, Endianness.Little, LineType.Verbose)]
    public class UnknownVerboseArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public UnknownVerboseArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x01 }, TestName = "Encode_UnknownVerbose")]
        public void EncodeUnknownVerbose(byte[] value)
        {
            Span<byte> buffer = ArgEncode(new UnknownVerboseDltArg(value, IsBigEndian), value.Length);
            Assert.That(buffer.Length, Is.EqualTo(value.Length));

            Assert.That(buffer.ToArray(), Is.EqualTo(value));
        }

        [TestCase(TestName = "Encode_UnknownVerboseMax")]
        public void EncodeUnknownVerboseMax()
        {
            byte[] value = new byte[65535 - HeaderLen];
            if (IsBigEndian) {
                value[0] = 0x00;
                value[1] = 0x00;
                value[2] = 0x04;
                value[3] = 0x00;
            } else {
                value[0] = 0x00;
                value[1] = 0x04;
                value[2] = 0x00;
                value[3] = 0x00;
            }
            Random rnd = new();
            rnd.NextBytes(value.AsSpan(4));
            EncodeUnknownVerbose(value);
        }
    }

    [TestFixture(typeof(UnknownVerboseArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(UnknownVerboseArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    public class UnknownVerboseArgEncoderTestNv<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public UnknownVerboseArgEncoderTestNv(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x01 }, TestName = "Encode_NonVerboseError")]
        public void EncodeNonVerboseError(byte[] value)
        {
            byte[] buffer = new byte[HeaderLen + 4 + value.Length];
            ArgEncode(buffer, new UnknownVerboseDltArg(value, IsBigEndian), out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
