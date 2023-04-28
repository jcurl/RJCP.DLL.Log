namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class NonVerboseStaticStringDecoderTest : NonVerboseDecoderTestBase<NonVerboseArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(1, new TestPdu("Static String"));

        public NonVerboseStaticStringDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [Test]
        public void DecodeStaticString()
        {
            byte[] payload = Array.Empty<byte>();

            Decode(1, payload, $"nv_StaticString", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("Static String"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }
    }
}
