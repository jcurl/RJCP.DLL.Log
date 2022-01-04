namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    public class DltTimeMarkerDecoderTest : ControlDecoderTestBase<IControlArgDecoder, IControlArgDecoder>
    {
        public DltTimeMarkerDecoderTest(DecoderType decoderType)
            : base(decoderType, -1, null, typeof(DltTimeMarker))
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
