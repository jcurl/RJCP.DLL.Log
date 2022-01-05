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
    public class GetLocalTimeDecoderTest : ControlDecoderTestBase<GetLocalTimeRequestDecoder, GetLocalTimeResponseDecoder>
    {
        public GetLocalTimeDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x0C, typeof(GetLocalTimeRequest), typeof(GetLocalTimeResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0C, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x0C };
            Decode(DltType.CONTROL_REQUEST, payload, "0x0C_GetLocalTimeRequest", out IControlArg service);

            GetLocalTimeRequest request = (GetLocalTimeRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_local_time]"));
        }

        [TestCase(0x00, "[get_local_time ok]")]
        [TestCase(0x01, "[get_local_time not_supported]")]
        [TestCase(0x02, "[get_local_time error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0C, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0C, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0C_GetLocalTimeResponse_{status:x2}", out IControlArg service);

            GetLocalTimeResponse response = (GetLocalTimeResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
