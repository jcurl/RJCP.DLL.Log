﻿namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(GetSoftwareVersionResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(GetSoftwareVersionResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    public class GetSoftwareVersionResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public GetSoftwareVersionResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeSuccess()
        {
            GetSoftwareVersionResponse response =
                new GetSoftwareVersionResponse(ControlResponse.StatusOk, "version1");

            byte[] expected = IsBigEndian ?
                new byte[] { 0x00, 0x00, 0x00, 0x09, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x31, 0x00 } :
                new byte[] { 0x09, 0x00, 0x00, 0x00, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x31, 0x00 };

            Span<byte> buffer = ControlEncode(response, 18);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x13));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(buffer[5..].ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void EncodeEmpty([Values("", null)] string empty)
        {
            GetSoftwareVersionResponse response =
                new GetSoftwareVersionResponse(ControlResponse.StatusOk, empty);

            byte[] expected = IsBigEndian ?
                new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00 } :
                new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00 };

            Span<byte> buffer = ControlEncode(response, 10);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x13));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(buffer[5..].ToArray(), Is.EqualTo(expected));
        }

        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void EncodeError(int statusCode)
        {
            GetSoftwareVersionResponse response =
                new GetSoftwareVersionResponse(statusCode, string.Empty);

            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x13));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
        }

        [Test]
        public void InsufficientBuffer([Values(3, 4, 5, 6, 7, 8, 9, 10, 16, 17)] int length)
        {
            GetSoftwareVersionResponse response =
                new GetSoftwareVersionResponse(ControlResponse.StatusOk, "version1");

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
