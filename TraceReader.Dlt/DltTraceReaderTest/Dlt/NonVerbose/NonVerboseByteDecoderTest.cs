namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    public class NonVerboseByteDecoderTest : NonVerboseByteDecoderTestBase
    {
        public NonVerboseByteDecoderTest(DecoderType decoderType, Endianness endian) : base(decoderType, endian) { }

        [Test]
        public void DecodeNonVerboseBytes()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x12 } :
                new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x12 };

            Decode(payload, "NonVerbose", out IDltArg nonVerboseArg);
            Assert.That(nonVerboseArg, Is.TypeOf<NonVerboseDltArg>());
            NonVerboseDltArg arg = (NonVerboseDltArg)nonVerboseArg;
            Assert.That(arg.MessageId, Is.EqualTo(1));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x00, 0x12 }));
        }
    }
}
