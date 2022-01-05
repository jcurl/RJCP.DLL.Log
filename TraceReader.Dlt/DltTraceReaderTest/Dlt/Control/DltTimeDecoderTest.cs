namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    public class DltTimeMarkerDecoderTest : ControlDecoderTestBase<NoDecoder, NoDecoder>
    {
        public DltTimeMarkerDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, -1, null, typeof(DltTimeMarker))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Array.Empty<byte>();
            Decode(DltType.CONTROL_TIME, payload, "DltTimeMarker", out IControlArg service);

            DltTimeMarker request = (DltTimeMarker)service;
            Assert.That(request.ToString(), Is.EqualTo("[]"));
        }
    }
}
