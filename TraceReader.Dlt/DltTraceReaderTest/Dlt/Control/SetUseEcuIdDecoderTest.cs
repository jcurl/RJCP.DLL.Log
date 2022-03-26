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
    public class SetUseEcuIdDecoderTest : ControlDecoderTestBase<SetUseEcuIdRequestDecoder, SetUseEcuIdResponseDecoder>
    {
        public SetUseEcuIdDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x0D, typeof(SetUseEcuIdRequest), typeof(SetUseEcuIdResponse))
        { }

        [TestCase(0x00, "[use_ecu_id] off")]
        [TestCase(0x01, "[use_ecu_id] on")]
        [TestCase(0xFF, "[use_ecu_id] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0D, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0D, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0D_SetUseEcuIdRequest_{status:x2}", out IControlArg service);

            SetUseEcuIdRequest request = (SetUseEcuIdRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_ecu_id ok]")]
        [TestCase(0x01, "[use_ecu_id not_supported]")]
        [TestCase(0x02, "[use_ecu_id error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0D, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0D, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0D_SetUseEcuIdResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x0D));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
