namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class SyncTimeStampDecoderTest : ControlDecoderTestBase<SyncTimeStampRequestDecoder, SyncTimeStampResponseDecoder>
    {
        public SyncTimeStampDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x24, typeof(SyncTimeStampRequest), typeof(SyncTimeStampResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x24, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x24 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x24_SyncTimeStampRequest", out IControlArg service);

            SyncTimeStampRequest request = (SyncTimeStampRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[sync_timestamp]"));
        }

        [TestCase(0x00, "[sync_timestamp ok] 2022-01-05 09:48:46.55685Z")]
        [TestCase(0x01, "[sync_timestamp not_supported] 2022-01-05 09:48:46.55685Z")]
        [TestCase(0x02, "[sync_timestamp error] 2022-01-05 09:48:46.55685Z")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x24, 0x00, 0x00, 0x00, status,
                    0x0D, 0xEC, 0x30, 0x21,           // 556,854,285 ns
                    0x7E, 0x69, 0xD5, 0x61,           // Number of seconds in UTC
                    0x00, 0x00
                } :
                new byte[] { 0x00, 0x00, 0x00, 0x24, status,
                    0x21, 0x30, 0xEC, 0x0D,           // 556,854,285 ns
                    0x61, 0xD5, 0x69, 0x7E,           // Number of seconds in UTC
                    0x00, 0x00
                };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x24_SyncTimeStampResponse_{status:x2}", out IControlArg service);

            SyncTimeStampResponse response = (SyncTimeStampResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeStamp, Is.EqualTo(new DateTime(2022, 1, 5, 9, 48, 46, DateTimeKind.Utc).AddTicks(5568542)));
        }

        [TestCase(0x00, "[sync_timestamp ok] 2039-01-10 04:37:18.00143Z")]
        [TestCase(0x01, "[sync_timestamp not_supported] 2039-01-10 04:37:18.00143Z")]
        [TestCase(0x02, "[sync_timestamp error] 2039-01-10 04:37:18.00143Z")]
        public void DecodeResponseLargeDate(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x24, 0x00, 0x00, 0x00, status,
                    0xF8, 0xF2, 0x15, 0x00,           // 1,438,456 ns
                    0x7E, 0x69, 0xD5, 0x81,           // Number of seconds in UTC
                    0x00, 0x00
                } :
                new byte[] { 0x00, 0x00, 0x00, 0x24, status,
                    0x00, 0x15, 0xF2, 0xF8,           // 1,438,456 ns
                    0x81, 0xD5, 0x69, 0x7E,           // Number of seconds in UTC
                    0x00, 0x00
                };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x24_SyncTimeStampResponse_{status:x2}", out IControlArg service);

            SyncTimeStampResponse response = (SyncTimeStampResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeStamp, Is.EqualTo(new DateTime(2039, 1, 10, 4, 37, 18, DateTimeKind.Utc).AddTicks(14384)));
        }
    }
}
