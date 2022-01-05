namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SyncTimeStampDecoderTest : ControlDecoderTestBase<SyncTimeStampRequestDecoder, SyncTimeStampResponseDecoder>
    {
        public SyncTimeStampDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x24, typeof(SyncTimeStampRequest), typeof(SyncTimeStampResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x24, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x24_SyncTimeStampRequest", out IControlArg service);

            SyncTimeStampRequest request = (SyncTimeStampRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[sync_timestamp]"));
        }

        [TestCase(0x00, "[sync_timestamp ok] 2022-01-05 09:48:46.55685Z")]
        [TestCase(0x01, "[sync_timestamp not_supported] 2022-01-05 09:48:46.55685Z")]
        [TestCase(0x02, "[sync_timestamp error] 2022-01-05 09:48:46.55685Z")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x24, 0x00, 0x00, 0x00, status,
                0x0D, 0xEC, 0x30, 0x21,           // 556,854,285 ns
                0x7E, 0x69, 0xD5, 0x61,           // Number of seconds in UTC
                0x00, 0x00

            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x24_SyncTimeStampResponse_{status:x2}", out IControlArg service);

            SyncTimeStampResponse response = (SyncTimeStampResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeStamp, Is.EqualTo(new DateTime(2022, 1, 5, 9, 48, 46, DateTimeKind.Utc).AddTicks(5568542)));
        }
    }
}
