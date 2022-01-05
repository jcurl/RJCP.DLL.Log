namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class CustomTimeZoneDecoderTest : ControlDecoderTestBase<NoDecoder, CustomTimeZoneResponseDecoder>
    {
        public CustomTimeZoneDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x0F03, null, typeof(CustomTimeZoneResponse))
        { }

        [TestCase(0x00, 0x00, "[timezone ok] +10:00")]
        [TestCase(0x01, 0x00, "[timezone not_supported] +10:00")]
        [TestCase(0x02, 0x00, "[timezone error] +10:00")]
        [TestCase(0x00, 0x01, "[timezone ok] +10:00 DST")]
        [TestCase(0x01, 0x01, "[timezone not_supported] +10:00 DST")]
        [TestCase(0x02, 0x01, "[timezone error] +10:00 DST")]
        [TestCase(0x00, 0xFF, "[timezone ok] +10:00 DST")]
        [TestCase(0x01, 0xFF, "[timezone not_supported] +10:00 DST")]
        [TestCase(0x02, 0xFF, "[timezone error] +10:00 DST")]
        public void DecodeResponsePositiveTz(byte status, byte isDst, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x03, 0x0F, 0x00, 0x00, status, 0xA0, 0x8C, 0x00, 0x00, isDst } :
                new byte[] { 0x00, 0x00, 0x0F, 0x03, status, 0x00, 0x00, 0x8C, 0xA0, isDst };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF03_CustomTimeZoneResponseResponse_P_{status:x2}_{isDst:x2}", out IControlArg service);

            CustomTimeZoneResponse response = (CustomTimeZoneResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeZone.TotalSeconds, Is.EqualTo(36000));
            Assert.That(response.IsDst, Is.EqualTo(isDst != 0));
        }

        [TestCase(0x00, 0x00, "[timezone ok] +09:45")]
        [TestCase(0x01, 0x00, "[timezone not_supported] +09:45")]
        [TestCase(0x02, 0x00, "[timezone error] +09:45")]
        [TestCase(0x00, 0x01, "[timezone ok] +09:45 DST")]
        [TestCase(0x01, 0x01, "[timezone not_supported] +09:45 DST")]
        [TestCase(0x02, 0x01, "[timezone error] +09:45 DST")]
        [TestCase(0x00, 0xFF, "[timezone ok] +09:45 DST")]
        [TestCase(0x01, 0xFF, "[timezone not_supported] +09:45 DST")]
        [TestCase(0x02, 0xFF, "[timezone error] +09:45 DST")]
        public void DecodeResponsePositiveTz2(byte status, byte isDst, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x03, 0x0F, 0x00, 0x00, status, 0x1C, 0x89, 0x00, 0x00, isDst } :
                new byte[] { 0x00, 0x00, 0x0F, 0x03, status, 0x00, 0x00, 0x89, 0x1C, isDst };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF03_CustomTimeZoneResponseResponse_P2_{status:x2}_{isDst:x2}", out IControlArg service);

            CustomTimeZoneResponse response = (CustomTimeZoneResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeZone.TotalSeconds, Is.EqualTo(35100));
            Assert.That(response.IsDst, Is.EqualTo(isDst != 0));
        }

        [TestCase(0x00, 0x00, "[timezone ok] -05:00")]
        [TestCase(0x01, 0x00, "[timezone not_supported] -05:00")]
        [TestCase(0x02, 0x00, "[timezone error] -05:00")]
        [TestCase(0x00, 0x01, "[timezone ok] -05:00 DST")]
        [TestCase(0x01, 0x01, "[timezone not_supported] -05:00 DST")]
        [TestCase(0x02, 0x01, "[timezone error] -05:00 DST")]
        [TestCase(0x00, 0xFF, "[timezone ok] -05:00 DST")]
        [TestCase(0x01, 0xFF, "[timezone not_supported] -05:00 DST")]
        [TestCase(0x02, 0xFF, "[timezone error] -05:00 DST")]
        public void DecodeResponseNegativeTz(byte status, byte isDst, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x03, 0x0F, 0x00, 0x00, status, 0xB0, 0xB9, 0xFF, 0xFF, isDst } :
                new byte[] { 0x00, 0x00, 0x0F, 0x03, status, 0xFF, 0xFF, 0xB9, 0xB0, isDst };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF03_CustomTimeZoneResponseResponse_N_{status:x2}_{isDst:x2}", out IControlArg service);

            CustomTimeZoneResponse response = (CustomTimeZoneResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeZone.TotalSeconds, Is.EqualTo(-18000));
            Assert.That(response.IsDst, Is.EqualTo(isDst != 0));
        }

        [TestCase(0x00, 0x00, "[timezone ok] -06:30")]
        [TestCase(0x01, 0x00, "[timezone not_supported] -06:30")]
        [TestCase(0x02, 0x00, "[timezone error] -06:30")]
        [TestCase(0x00, 0x01, "[timezone ok] -06:30 DST")]
        [TestCase(0x01, 0x01, "[timezone not_supported] -06:30 DST")]
        [TestCase(0x02, 0x01, "[timezone error] -06:30 DST")]
        [TestCase(0x00, 0xFF, "[timezone ok] -06:30 DST")]
        [TestCase(0x01, 0xFF, "[timezone not_supported] -06:30 DST")]
        [TestCase(0x02, 0xFF, "[timezone error] -06:30 DST")]
        public void DecodeResponseNegativeTz2(byte status, byte isDst, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x03, 0x0F, 0x00, 0x00, status, 0x98, 0xA4, 0xFF, 0xFF, isDst } :
                new byte[] { 0x00, 0x00, 0x0F, 0x03, status, 0xFF, 0xFF, 0xA4, 0x98, isDst };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF03_CustomTimeZoneResponseResponse_N2_{status:x2}_{isDst:x2}", out IControlArg service);

            CustomTimeZoneResponse response = (CustomTimeZoneResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeZone.TotalSeconds, Is.EqualTo(-23400));
            Assert.That(response.IsDst, Is.EqualTo(isDst != 0));
        }

        [TestCase(0x00, 0x00, "[timezone ok] +00:00")]
        [TestCase(0x01, 0x00, "[timezone not_supported] +00:00")]
        [TestCase(0x02, 0x00, "[timezone error] +00:00")]
        [TestCase(0x00, 0x01, "[timezone ok] +00:00 DST")]
        [TestCase(0x01, 0x01, "[timezone not_supported] +00:00 DST")]
        [TestCase(0x02, 0x01, "[timezone error] +00:00 DST")]
        [TestCase(0x00, 0xFF, "[timezone ok] +00:00 DST")]
        [TestCase(0x01, 0xFF, "[timezone not_supported] +00:00 DST")]
        [TestCase(0x02, 0xFF, "[timezone error] +00:00 DST")]
        public void DecodeResponseUtc(byte status, byte isDst, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x03, 0x0F, 0x00, 0x00, status, 0x00, 0x00, 0x00, 0x00, isDst } :
                new byte[] { 0x00, 0x00, 0x0F, 0x03, status, 0x00, 0x00, 0x00, 0x00, isDst };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF03_CustomTimeZoneResponseResponse_U_{status:x2}_{isDst:x2}", out IControlArg service);

            CustomTimeZoneResponse response = (CustomTimeZoneResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.TimeZone.Ticks, Is.EqualTo(0));
            Assert.That(response.IsDst, Is.EqualTo(isDst != 0));
        }
    }
}
