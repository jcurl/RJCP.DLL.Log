namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SyncTimeStampResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SyncTimeStampResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    public class SyncTimeStampResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SyncTimeStampResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeUtc()
        {
            SyncTimeStampResponse response =
                new SyncTimeStampResponse(ControlResponse.StatusOk, new DateTime(2023, 05, 22, 10, 54, 23, 991, DateTimeKind.Utc));

            Span<byte> buffer = ControlEncode(response, 15);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x24));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(BitOperations.To32Shift(buffer[5..9], !IsBigEndian), Is.EqualTo(991000000));
            Assert.That(unchecked((uint)BitOperations.To32Shift(buffer[9..13], !IsBigEndian)), Is.EqualTo(1684752863));
            Assert.That(BitOperations.To16Shift(buffer[13..15], !IsBigEndian), Is.EqualTo(0));
        }

        [Test]
        public void EncodeLocal()
        {
            DateTime time = new DateTime(2023, 05, 22, 10, 54, 23, 991, DateTimeKind.Utc);
            DateTime localtime = time.ToLocalTime();   // The number of ticks is now different.

            SyncTimeStampResponse response =
                new SyncTimeStampResponse(ControlResponse.StatusOk, localtime);

            Span<byte> buffer = ControlEncode(response, 15);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x24));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(BitOperations.To32Shift(buffer[5..9], !IsBigEndian), Is.EqualTo(991000000));
            Assert.That(unchecked((uint)BitOperations.To32Shift(buffer[9..13], !IsBigEndian)), Is.EqualTo(1684752863));
            Assert.That(BitOperations.To16Shift(buffer[13..15], !IsBigEndian), Is.EqualTo(0));
        }

        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void EncodeError(int statusCode)
        {
            SyncTimeStampResponse response =
                new SyncTimeStampResponse(statusCode, DateTime.UtcNow);

            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x24));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 14, 1)] int length)
        {
            SyncTimeStampResponse response =
                new SyncTimeStampResponse(ControlResponse.StatusOk, new DateTime(2023, 05, 22, 10, 54, 23, 991, DateTimeKind.Utc));

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
