namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(SignedIntArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class SignedIntArgDecoderTest<T> where T : IVerboseArgDecoder
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

        private const int SignedArgType = 0x40;

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSint8bitLE(byte value, int result)
        {
            DecodeSint(new byte[] { 0x21, 0x00, 0x00, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSint8bitBE(byte value, int result)
        {
            DecodeSint(new byte[] { 0x21, 0x00, 0x00, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeSint16bitLE()
        {
            DecodeSint(new byte[] { 0x22, 0x00, 0x00, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeSint16bitBE()
        {
            DecodeSint(new byte[] { 0x22, 0x00, 0x00, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeSint32bitLE()
        {
            DecodeSint(new byte[] { 0x23, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeSint32bitBE()
        {
            DecodeSint(new byte[] { 0x23, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeSint64bitLE()
        {
            DecodeSint(new byte[] { 0x24, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, unchecked((long)0x8967452301EFCDAB));
        }

        [Test]
        public void DecodeSint64bitBE()
        {
            DecodeSint(new byte[] { 0x24, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, unchecked((long)0xABCDEF0123456789));
        }

        [Test]
        public void DecodeSint128bitLE()
        {
            DecodeSint128(new byte[] {
                0x25, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 0);
        }

        [Test]
        public void DecodeSint128bitBE()
        {
            DecodeSint128(new byte[] {
                0x25, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 0);
        }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSintUnknownCoding8bitLE(byte value, int result)
        {
            DecodeSint(new byte[] { 0x21, 0x00, 0x01, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSintUnknownCoding8bitBE(byte value, int result)
        {
            DecodeSint(new byte[] { 0x21, 0x00, 0x01, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeSintUnknownCoding16bitLE()
        {
            DecodeSint(new byte[] { 0x22, 0x00, 0x01, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeSintUnknownCoding16bitBE()
        {
            DecodeSint(new byte[] { 0x22, 0x00, 0x01, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeSintUnknownCoding32bitLE()
        {
            DecodeSint(new byte[] { 0x23, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeSintUnknownCoding32bitBE()
        {
            DecodeSint(new byte[] { 0x23, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeSintUnknownCoding64bitLE()
        {
            DecodeSint(new byte[] { 0x24, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, unchecked((long)0x8967452301EFCDAB));
        }

        [Test]
        public void DecodeSintUnknownCoding64bitBE()
        {
            DecodeSint(new byte[] { 0x24, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, unchecked((long)0xABCDEF0123456789));
        }

        [Test]
        public void DecodeSintUnknownCoding128bitLE()
        {
            DecodeSint128(new byte[] {
                0x25, 0x00, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 2);
        }

        [Test]
        public void DecodeSintUnknownCoding128bitBE()
        {
            DecodeSint128(new byte[] {
                0x25, 0x00, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 2);
        }

        [TestCase(0x20, TestName = "DecodeSintUnknown")]
        [TestCase(0x27, TestName = "DecodeSintUndefined")]
        public void DecodeSint(byte typeInfo)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(new byte[] { typeInfo, 0x00, 0x00, 0x00 }, false, out IDltArg arg);
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(arg, Is.Null);
        }

        private static void DecodeSint(byte[] buffer, bool msbf, long result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<SignedIntDltArg>());
            Assert.That(((SignedIntDltArg)arg).Data, Is.EqualTo(result));
        }

        private static void DecodeSint128(byte[] buffer, bool msbf, int encoding)
        {
            ArgDecoderTest.DecodeUnknown<T>(buffer, msbf, encoding, SignedArgType);
        }
    }
}
