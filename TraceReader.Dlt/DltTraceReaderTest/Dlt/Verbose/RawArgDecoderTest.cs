namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(RawArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class RawArgDecoderTest<T> where T : IVerboseArgDecoder
    {
        [Test]
        public void RawArgLE()
        {
            DecodeRaw(new byte[] {
                0x00, 0x04, 0x00, 0x00,
                0x08, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88
            }, false);
        }

        [Test]
        public void RawArgBE()
        {
            DecodeRaw(new byte[] {
                0x00, 0x04, 0x00, 0x00,
                0x00, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88
            }, true);
        }

        [Test]
        public void RawArgEmptyLE()
        {
            DecodeRaw(new byte[] { 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void RawArgEmptyBE()
        {
            DecodeRaw(new byte[] { 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 }, true);
        }

        [Test]
        public void RawArgLargePayloadLE()
        {
            byte[] data = new byte[65000];
            new Random().NextBytes(data);
            data[0] = 0x00;
            data[1] = 0x04;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = (byte)((data.Length - 6) & 0xFF);
            data[5] = (byte)((data.Length - 6) >> 8);

            DecodeRaw(data, false);
        }

        [Test]
        public void RawArgLargePayloadBE()
        {
            byte[] data = new byte[65000];
            new Random().NextBytes(data);
            data[0] = 0x00;
            data[1] = 0x04;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = (byte)((data.Length - 6) >> 8);
            data[5] = (byte)((data.Length - 6) & 0xFF);

            DecodeRaw(data, true);
        }

        private static void DecodeRaw(byte[] buffer, bool msbf)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<RawDltArg>());
            Assert.That(((RawDltArg)arg).Data, Is.EqualTo(buffer[6..]));
        }
    }
}
