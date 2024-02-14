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
    public class NonVerboseRawArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseRawArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(1, new TestPdu("S_RAW", 0))
            .Add(2, new TestPdu("S_RAWD", 0));

        public NonVerboseRawArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [TestCase(1)]
        [TestCase(2)]
        public void RawArg(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 } :
                new byte[] { 0x00, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            Decode(messageId, payload, $"nv_RawArg_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<RawDltArg>());

            RawDltArg arg = (RawDltArg)dltArg;
            Assert.That(arg.Data, Is.EqualTo(payload[2..]));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void RawArgEmpty(int messageId)
        {
            byte[] payload = new byte[] { 0x00, 0x00 };

            Decode(messageId, payload, $"nv_RawArgEmpty_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<RawDltArg>());

            RawDltArg arg = (RawDltArg)dltArg;
            Assert.That(arg.Data, Is.Empty);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void RawArgLarge(int messageId)
        {
            byte[] payload = new byte[65000];
            Random r = new();
            if (Endian == Endianness.Little) {
                payload[0] = (byte)((payload.Length - 2) & 0xFF);
                payload[1] = (byte)((payload.Length - 2) >> 8);
            } else {
                payload[0] = (byte)((payload.Length - 2) >> 8);
                payload[1] = (byte)((payload.Length - 2) & 0xFF);
            }

            for (int i = 2; i < payload.Length; i++) {
                payload[i] = (byte)r.Next(0, 255);
            }

            Decode(messageId, payload, $"nv_RawArgLarge_{messageId}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<RawDltArg>());

            RawDltArg arg = (RawDltArg)dltArg;
            Assert.That(arg.Data, Is.EqualTo(payload[2..]));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeRawEmptyBuffer(int messageId)
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(messageId, payload, $"nv_Raw_EmptyBuffer_{messageId}");
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeRawOnlyLengthBuffer(int messageId)
        {
            byte[] payload = new byte[] { 0x01, 0x01 };

            DecodeIsInvalid(messageId, payload, $"nv_RawOnlyLengthBuffer_{messageId}");
        }

        [TestCase(1)]
        [TestCase(2)]
        public void DecodeRawBufferTooShort(int messageId)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x08, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 } :
                new byte[] { 0x00, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };

            DecodeIsInvalid(messageId, payload, $"nv_RawBufferTooShort_{messageId}");
        }
    }
}
