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
    public class SetTimingPacketsDecoderTest : ControlDecoderTestBase<SetTimingPacketsRequestDecoder, SetTimingPacketsResponseDecoder>
    {
        public SetTimingPacketsDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x0B, typeof(SetTimingPacketsRequest), typeof(SetTimingPacketsResponse))
        { }

        [TestCase(0x00, "[set_timing_packets] off")]
        [TestCase(0x01, "[set_timing_packets] on")]
        [TestCase(0xFF, "[set_timing_packets] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0B, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0B, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0B_SetTimingPacketsRequest_{status:x2}", out IControlArg service);

            SetTimingPacketsRequest request = (SetTimingPacketsRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_timing_packets ok]")]
        [TestCase(0x01, "[set_timing_packets not_supported]")]
        [TestCase(0x02, "[set_timing_packets error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x0B, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x0B, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0B_SetTimingPacketsResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x0B));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
