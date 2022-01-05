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
    public class GetSoftwareVersionDecoderTest
        : ControlDecoderTestBase<GetSoftwareVersionRequestDecoder, GetSoftwareVersionResponseDecoder>
    {
        public GetSoftwareVersionDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x13, typeof(GetSoftwareVersionRequest), typeof(GetSoftwareVersionResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x13, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x13 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x13_GetSoftwareVersionRequest", out IControlArg service);

            GetSoftwareVersionRequest request = (GetSoftwareVersionRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_software_version]"));
        }

        [Test]
        public void DecodeResponseNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E } :
                new byte[] { 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x07, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E };
            Decode(DltType.CONTROL_RESPONSE, payload, "0x13_GetSoftwareVersionResponse_NoNul", out IControlArg service);

            GetSoftwareVersionResponse response = (GetSoftwareVersionResponse)service;
            Assert.That(response.SwVersion, Is.EqualTo("Version"));
            Assert.That(response.Status, Is.EqualTo(0));
            Assert.That(response.ToString(), Is.EqualTo("[get_software_version ok] Version"));
        }

        [Test]
        public void DecodeResponse()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x08, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x00 };
            Decode(DltType.CONTROL_RESPONSE, payload, "0x13_GetSoftwareVersionResponse", out IControlArg service);

            GetSoftwareVersionResponse response = (GetSoftwareVersionResponse)service;
            Assert.That(response.SwVersion, Is.EqualTo("Version"));
            Assert.That(response.Status, Is.EqualTo(0));
            Assert.That(response.ToString(), Is.EqualTo("[get_software_version ok] Version"));
        }
    }
}
