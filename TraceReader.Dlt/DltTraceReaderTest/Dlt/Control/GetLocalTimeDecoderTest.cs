namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetLocalTimeDecoderTest : ControlDecoderTestBase<GetLocalTimeRequestDecoder, GetLocalTimeResponseDecoder>
    {
        public GetLocalTimeDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0C, typeof(GetLocalTimeRequest), typeof(GetLocalTimeResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x0C, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x0C_GetLocalTimeRequest", out IControlArg service);

            GetLocalTimeRequest request = (GetLocalTimeRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_local_time]"));
        }

        [TestCase(0x00, "[get_local_time ok]")]
        [TestCase(0x01, "[get_local_time not_supported]")]
        [TestCase(0x02, "[get_local_time error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0C, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0C_GetLocalTimeResponse_{status:x2}", out IControlArg service);

            GetLocalTimeResponse response = (GetLocalTimeResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
