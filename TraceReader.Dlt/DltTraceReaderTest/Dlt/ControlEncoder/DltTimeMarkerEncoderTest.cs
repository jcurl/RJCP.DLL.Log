namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    public class DltTimeMarkerEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public DltTimeMarkerEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeTimeMarker([Values(0, 64)] int len)
        {
            DltTimeMarker response = new DltTimeMarker();

            byte[] buffer = new byte[len];
            Span<byte> payload = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(0));
            Assert.That(payload.Length, Is.EqualTo(0));
        }
    }
}
