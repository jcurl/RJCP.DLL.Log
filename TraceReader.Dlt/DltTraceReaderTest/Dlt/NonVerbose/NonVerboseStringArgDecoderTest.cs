namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
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
    public class NonVerboseStringArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseStringArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(1, new TestPdu("S_STRG_ASCII", 0))
            .Add(2, new TestPdu("S_STRG_UTF8", 0))
            .Add(3, new TestPdu("S_UTF8", 0));

        public NonVerboseStringArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override NonVerboseStringArgDecoder CreateArgDecoder(int messageId)
        {
            string decoderType = Map.GetFrame(messageId, null, null, null).Arguments[0].PduType;
            switch (decoderType) {
            case "S_STRG_ASCII": return new NonVerboseStringArgDecoder(StringEncodingType.Ascii);
            case "S_STRG_UTF8": return new NonVerboseStringArgDecoder(StringEncodingType.Utf8);
            case "S_UTF8": return new NonVerboseStringArgDecoder(StringEncodingType.Utf8);
            default: throw new NotImplementedException();
            }
        }

        [TestCase(2)]
        [TestCase(3)]
        public void DecodeUtf8String(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x09, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x09, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(messageId, payload, $"nv_StringUtf8_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [TestCase(2)]
        [TestCase(3)]
        public void DecodeUtf8StringNoNul(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E } :
                new byte[] { 0x00, 0x08, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E };

            Decode(messageId, payload, $"nv_StringUtf8_NoNul_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeAsciiString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x09, 0x00, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x09, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(1, payload, "nv_StringAscii", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("Muenchen"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiStringNoNul()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E } :
                new byte[] { 0x00, 0x08, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E };

            Decode(1, payload, "nv_StringAscii_NoNul", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("Muenchen"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiStringAsIso()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 } :
                new byte[] { 0x00, 0x08, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00 };

            Decode(1, payload, "nv_StringAsciiAsIso", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("München"));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [Test]
        public void DecodeAsciiEmptyString()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {0x01, 0x00, 0x00 } :
                new byte[] {0x00, 0x01, 0x00 };

            Decode(1, payload, "nv_StringAsciiEmpty", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [TestCase(2)]
        [TestCase(3)]
        public void DecodeUtf8EmptyString(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x01, 0x00, 0x00 } :
            new byte[] { 0x00, 0x01, 0x00 };

            Decode(messageId, payload, $"nv_StringUtf8Empty_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeAsciiEmptyStringNoNul()
        {
            byte[] payload = new byte[] { 0x00, 0x00 };

            Decode(1, payload, "nv_StringAsciiEmptyNoNul", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [TestCase(2)]
        [TestCase(3)]
        public void DecodeUtf8EmptyStringNoNul(int messageId)
        {
            byte[] payload = new byte[] { 0x00, 0x00 };

            Decode(messageId, payload, $"nv_StringUtf8EmptyNoNul_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Utf8));
        }

        [Test]
        public void DecodeAsciiLargePayloadLE()
        {
            byte[] payload = new byte[65000];
            Random r = new Random();
            if (Endian == Endianness.Little) {
                payload[0] = (byte)((payload.Length - 2) & 0xFF);
                payload[1] = (byte)((payload.Length - 2) >> 8);
            } else {
                payload[0] = (byte)((payload.Length - 2) >> 8);
                payload[1] = (byte)((payload.Length - 2) & 0xFF);
            }

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 2; i < payload.Length; i++) {
                payload[i] = (byte)r.Next(32, 126);
                strBuilder.Append((char)payload[i]);
            }

            Decode(1, payload, "nv_StringAsciiLarge", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<StringDltArg>());

            StringDltArg arg = (StringDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(strBuilder.ToString()));
            Assert.That(arg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeStringEmptyBuffer(int messageId)
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(messageId, payload, $"nv_String_EmptyBuffer_{messageId}");
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeStringOnlyLengthBuffer(int messageId)
        {
            byte[] payload = new byte[] { 0x01, 0x01 };

            DecodeIsInvalid(messageId, payload, $"nv_StringOnlyLengthBuffer_{messageId}");
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeStringBufferTooShort(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 } :
                new byte[] { 0x00, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };

            DecodeIsInvalid(messageId, payload, $"nv_StringBufferTooShort_{messageId}");
        }
    }
}
