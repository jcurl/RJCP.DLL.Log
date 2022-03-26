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
    public class SetUseTimeStampDecoderTest : ControlDecoderTestBase<SetUseTimeStampRequestDecoder, SetUseTimeStampResponseDecoder>
    {
        public SetUseTimeStampDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x0F, typeof(SetUseTimeStampRequest), typeof(SetUseTimeStampResponse))
        { }

        [TestCase(0x00, "[use_timestamp] off")]
        [TestCase(0x01, "[use_timestamp] on")]
        [TestCase(0xFF, "[use_timestamp] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0F, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0F, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0F_SetUseTimeStampRequest_{status:x2}", out IControlArg service);

            SetUseTimeStampRequest request = (SetUseTimeStampRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_timestamp ok]")]
        [TestCase(0x01, "[use_timestamp not_supported]")]
        [TestCase(0x02, "[use_timestamp error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0F, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0F, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0F_SetUseTimeStampResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x0F));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
