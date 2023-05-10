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
    public class NonVerboseUnknownArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseUnknownArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_FOO", 0))
            .Add(1, new TestPdu("S_FOO8", 1))
            .Add(2, new TestPdu("S_FOO16", 2))
            .Add(3, new TestPdu("S_FOO64", 10));

        public NonVerboseUnknownArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [Test]
        public void DecodeUnknownVariableLength()
        {
            // Because the PDU defines the length as zero, assume the length is in the payload.
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x10, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 } :
                new byte[] { 0x00, 0x10, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };

            Decode(0, payload, $"nv_Unknown_VarLength", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.Data, Has.Length.EqualTo(16));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 }));
            Assert.That(arg.ToString(), Is.EqualTo("(10) 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f 10"));
        }

        [Test]
        public void DecodeUnknownVariableLength_Zero()
        {
            // Because the PDU defines the length as zero, assume the length is in the payload.
            byte[] payload = new byte[] { 0x00, 0x00 };

            Decode(0, payload, $"nv_Unknown_VarLengthZeroBuffer", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.Data, Is.Empty);
            Assert.That(arg.Data, Is.EqualTo(Array.Empty<byte>()));
            Assert.That(arg.ToString(), Is.EqualTo("(00)"));
        }

        [Test]
        public void DecodeUnknown8bit()
        {
            byte[] payload = new byte[] { 0x10 };

            Decode(1, payload, $"nv_Unknown8bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.Data, Has.Length.EqualTo(1));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x10 }));
            Assert.That(arg.ToString(), Is.EqualTo("(01) 10"));
        }

        [Test]
        public void DecodeUnknown16bit()
        {
            byte[] payload = new byte[] { 0x10, 0xFF };

            Decode(2, payload, $"nv_Unknown16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.Data, Has.Length.EqualTo(2));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x10, 0xFF }));
            Assert.That(arg.ToString(), Is.EqualTo("(02) 10 ff"));
        }

        [Test]
        public void DecodeUnknown64bit()
        {
            byte[] payload = new byte[] { 0xFF, 0x80, 0x55, 0x00, 0x01, 0xAA, 0xFF, 0x80, 0x7F, 0x43 };

            Decode(3, payload, $"nv_Unknown10byte", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.Data, Has.Length.EqualTo(10));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0xFF, 0x80, 0x55, 0x00, 0x01, 0xAA, 0xFF, 0x80, 0x7F, 0x43 }));
            Assert.That(arg.ToString(), Is.EqualTo("(0a) ff 80 55 00 01 aa ff 80 7f 43"));
        }

        [Test]
        public void DecodeUnknownNoBuffer_VarLength()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_UnknownVarLength_Invalid");
        }

        [Test]
        public void DecodeUnknownNoBuffer_FixedLength()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_UnknownFixedLength_Invalid");
        }

        [Test]
        public void DecodeUnknownNoBufferLen_FixedLength()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(0, payload, "nv_UnknownVarLength_InvalidBufferLen");
        }

        [Test]
        public void DecodeUnknownInsufficientBufferLen_FixedLength()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x01, 0x00 } :
                new byte[] { 0x00, 0x01 };

            DecodeIsInvalid(0, payload, "nv_UnknownVarLength_InsufficientBufferLen");
        }
    }
}
