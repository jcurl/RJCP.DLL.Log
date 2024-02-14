namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class StringArgDecoderTest : VerboseDecoderTestBase<StringArgDecoder>
    {
        public StringArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [Test]
        public void DecodeUtf8String()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x82, 0x00, 0x00, 0x09, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x00, 0x82, 0x00, 0x00, 0x09, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(payload, "StringUtf8", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeUtf8StringNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x82, 0x00, 0x00, 0x08, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E } :
                new byte[] { 0x00, 0x00, 0x82, 0x00, 0x00, 0x08, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E };

            Decode(payload, "StringUtf8_NoNul", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeAsciiString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x02, 0x00, 0x00, 0x09, 0x00, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x00, 0x02, 0x00, 0x00, 0x09, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(payload, "StringAscii", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("Muenchen"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiStringNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x02, 0x00, 0x00, 0x08, 0x00, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E } :
                new byte[] { 0x00, 0x00, 0x02, 0x00, 0x00, 0x08, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E };

            Decode(payload, "StringAscii_NoNul", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("Muenchen"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiStringAsIso()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x02, 0x00, 0x00, 0x08, 0x00, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x00, 0x02, 0x00, 0x00, 0x08, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(payload, "StringAsciiAsIso", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiEmptyString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x00 };

            Decode(payload, "StringAsciiEmpty", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeUtf8EmptyString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x82, 0x00, 0x00, 0x01, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x82, 0x00, 0x00, 0x01, 0x00 };

            Decode(payload, "StringUtf8Empty", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeAsciiEmptyStringNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 };

            Decode(payload, "StringAsciiEmptyNoNul", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeUtf8EmptyStringNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x82, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x82, 0x00, 0x00, 0x00 };

            Decode(payload, "StringUtf8EmptyNoNul", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeUnknownCodingString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x82, 0x03, 0x00, 0x09, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x03, 0x82, 0x00, 0x00, 0x09, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(payload, "StringUtf8", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo((StringEncodingType)7));
        }

        [Test]
        public void DecodeAsciiLargePayloadLE()
        {
            byte[] payload = new byte[65000];
            Random r = new();
            if (Endian == Endianness.Little) {
                payload[0] = 0x00;
                payload[1] = 0x02;
                payload[2] = 0x00;
                payload[3] = 0x00;
                payload[4] = (byte)((payload.Length - 6) & 0xFF);
                payload[5] = (byte)((payload.Length - 6) >> 8);
            } else {
                payload[0] = 0x00;
                payload[1] = 0x00;
                payload[2] = 0x02;
                payload[3] = 0x00;
                payload[4] = (byte)((payload.Length - 6) >> 8);
                payload[5] = (byte)((payload.Length - 6) & 0xFF);
            }

            StringBuilder strBuilder = new();
            for (int i = 6; i < payload.Length; i++) {
                payload[i] = (byte)r.Next(32, 126);
                strBuilder.Append((char)payload[i]);
            }

            Decode(payload, "StringAsciiLarge", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<StringDltArg>());
            StringDltArg arg = (StringDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(strBuilder.ToString()));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }
    }
}
