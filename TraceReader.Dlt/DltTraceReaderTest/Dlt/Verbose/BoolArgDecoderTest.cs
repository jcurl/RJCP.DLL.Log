namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(BoolArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class BoolArgDecoderTest<T> where T : IVerboseArgDecoder
    {
        [SetUp]
        public void InitializeTestCase()
        {
            if (typeof(T).Equals(typeof(VerboseArgDecoder))) {
                // Required to decode ISO-8859-15 when encoded as ASCII.
                var instance = CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(instance);
            }
        }

        [Test]
        public void DecodeBoolFalse8bit()
        {
            DecodeBool(new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void DecodeBoolFalse16bit()
        {
            DecodeBool(new byte[] { 0x12, 0x00, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void DecodeBoolFalse32bit()
        {
            DecodeBool(new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void DecodeBoolFalse64bit()
        {
            DecodeBool(new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void DecodeBoolFalse128bit()
        {
            DecodeBool(new byte[] { 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, false);
        }

        [Test]
        public void DecodeBoolTrue8bit()
        {
            DecodeBool(new byte[] { 0x11, 0x00, 0x00, 0x00, 0xFF }, true);
        }

        [Test]
        public void DecodeBoolTrue16bit()
        {
            DecodeBool(new byte[] { 0x12, 0x00, 0x00, 0x00, 0xFF, 0xFF }, true);
        }

        [Test]
        public void DecodeBoolTrue32bit()
        {
            DecodeBool(new byte[] { 0x13, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, true);
        }

        [Test]
        public void DecodeBoolTrue64bit()
        {
            DecodeBool(new byte[] { 0x14, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, true);
        }

        [Test]
        public void DecodeBoolTrue128bit()
        {
            DecodeBool(new byte[] { 0x15, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, true);
        }

        [TestCase(0x10, TestName = "DecodeBoolUnknown")]
        [TestCase(0x17, TestName = "DecodeBoolUndefined")]
        public void DecodeBool(byte typeInfo)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(new byte[] { typeInfo, 0x00, 0x00, 0x00 }, false, out IDltArg arg);
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(arg, Is.Null);
        }

        private static void DecodeBool(byte[] buffer, bool result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, false, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<BoolDltArg>());
            Assert.That(((BoolDltArg)arg).Data, Is.EqualTo(result));
        }
    }
}
