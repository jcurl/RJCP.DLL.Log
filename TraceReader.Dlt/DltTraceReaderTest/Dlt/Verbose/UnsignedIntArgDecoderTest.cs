namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(UnsignedIntArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class UnsignedIntArgDecoderTest<T> where T : IVerboseArgDecoder
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

        private const int UnsignedArgType = 0x80;

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUint8bitLE(byte value, uint result)
        {
            DecodeUint(new byte[] { 0x41, 0x00, 0x00, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUint8bitBE(byte value, uint result)
        {
            DecodeUint(new byte[] { 0x41, 0x00, 0x00, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeUint16bitLE()
        {
            DecodeUint(new byte[] { 0x42, 0x00, 0x00, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeUint16bitBE()
        {
            DecodeUint(new byte[] { 0x42, 0x00, 0x00, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeUint32bitLE()
        {
            DecodeUint(new byte[] { 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeUint32bitBE()
        {
            DecodeUint(new byte[] { 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeUint64bitLE()
        {
            DecodeUint(new byte[] { 0x44, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, 0x8967452301EFCDAB);
        }

        [Test]
        public void DecodeUint64bitBE()
        {
            DecodeUint(new byte[] { 0x44, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, 0xABCDEF0123456789);
        }

        [Test]
        public void DecodeUint128bitLE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 0);
        }

        [Test]
        public void DecodeUint128bitBE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 0);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUintUnknownCoding8bitLE(byte value, uint result)
        {
            DecodeUint(new byte[] { 0x41, 0x80, 0x03, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUintUnknownCoding8bitBE(byte value, uint result)
        {
            DecodeUint(new byte[] { 0x41, 0x80, 0x03, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeUintUnknownCoding16bitLE()
        {
            DecodeUint(new byte[] { 0x42, 0x80, 0x03, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeUintUnknownCoding16bitBE()
        {
            DecodeUint(new byte[] { 0x42, 0x80, 0x03, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeUintUnknownCoding32bitLE()
        {
            DecodeUint(new byte[] { 0x43, 0x80, 0x03, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeUintUnknownCoding32bitBE()
        {
            DecodeUint(new byte[] { 0x43, 0x80, 0x03, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeUintUnknownCoding64bitLE()
        {
            DecodeUint(new byte[] { 0x44, 0x80, 0x03, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, 0x8967452301EFCDAB);
        }

        [Test]
        public void DecodeUintUnknownCoding64bitBE()
        {
            DecodeUint(new byte[] { 0x44, 0x80, 0x03, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, 0xABCDEF0123456789);
        }

        [Test]
        public void DecodeUintUnknownCoding128bitLE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x80, 0x03, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 7);
        }

        [Test]
        public void DecodeUintUnknownCoding128bitBE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x80, 0x03, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 7);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeHexInt8bitLE(byte value, uint result)
        {
            DecodeHexUint(new byte[] { 0x41, 0x00, 0x01, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeHexInt8bitBE(byte value, uint result)
        {
            DecodeHexUint(new byte[] { 0x41, 0x00, 0x01, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeHexInt16bitLE()
        {
            DecodeHexUint(new byte[] { 0x42, 0x00, 0x01, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeHexInt16bitBE()
        {
            DecodeHexUint(new byte[] { 0x42, 0x00, 0x01, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeHexInt32bitLE()
        {
            DecodeHexUint(new byte[] { 0x43, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeHexInt32bitBE()
        {
            DecodeHexUint(new byte[] { 0x43, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeHexInt64bitLE()
        {
            DecodeHexUint(new byte[] { 0x44, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, 0x8967452301EFCDAB);
        }

        [Test]
        public void DecodeHexInt64bitBE()
        {
            DecodeHexUint(new byte[] { 0x44, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, 0xABCDEF0123456789);
        }

        [Test]
        public void DecodeHexInt128bitLE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x00, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 2);
        }

        [Test]
        public void DecodeHexInt128bitBE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x00, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 2);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeBinInt8bitLE(byte value, uint result)
        {
            DecodeBinUint(new byte[] { 0x41, 0x80, 0x01, 0x00, value }, false, result);
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeBinInt8bitBE(byte value, uint result)
        {
            DecodeBinUint(new byte[] { 0x41, 0x80, 0x01, 0x00, value }, true, result);
        }

        [Test]
        public void DecodeBinInt16bitLE()
        {
            DecodeBinUint(new byte[] { 0x42, 0x80, 0x01, 0x00, 0x12, 0x34 }, false, 0x3412);
        }

        [Test]
        public void DecodeBinInt16bitBE()
        {
            DecodeBinUint(new byte[] { 0x42, 0x80, 0x01, 0x00, 0x12, 0x34 }, true, 0x1234);
        }

        [Test]
        public void DecodeBinInt32bitLE()
        {
            DecodeBinUint(new byte[] { 0x43, 0x80, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, false, 0x78563412);
        }

        [Test]
        public void DecodeBinInt32bitBE()
        {
            DecodeBinUint(new byte[] { 0x43, 0x80, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 }, true, 0x12345678);
        }

        [Test]
        public void DecodeBinInt64bitLE()
        {
            DecodeBinUint(new byte[] { 0x44, 0x80, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, false, 0x8967452301EFCDAB);
        }

        [Test]
        public void DecodeBinInt64bitBE()
        {
            DecodeBinUint(new byte[] { 0x44, 0x80, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }, true, 0xABCDEF0123456789);
        }

        [Test]
        public void DecodeBinInt128bitLE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x80, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, false, 3);
        }

        [Test]
        public void DecodeBinInt128bitBE()
        {
            DecodeUint128(new byte[] {
                0x45, 0x80, 0x01, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
            }, true, 3);
        }

        [TestCase(0x40, 0x00, 0x00, TestName = "DecodeUintUnknown")]
        [TestCase(0x47, 0x00, 0x00, TestName = "DecodeUintUndefined")]
        [TestCase(0x40, 0x80, 0x01, TestName = "DecodeBinIntUnknown")]
        [TestCase(0x47, 0x80, 0x01, TestName = "DecodeBinIntUndefined")]
        [TestCase(0x40, 0x00, 0x01, TestName = "DecodeHexIntUnknown")]
        [TestCase(0x47, 0x00, 0x01, TestName = "DecodeHexIntUndefined")]
        public void DecodeUint(byte typeInfo, byte cod1, byte cod2)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(new byte[] { typeInfo, cod1, cod2, 0x00 }, false, out IDltArg arg);
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(arg, Is.Null);
        }

        private static void DecodeUint(byte[] buffer, bool msbf, ulong result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<UnsignedIntDltArg>());
            Assert.That(arg, Is.InstanceOf<IntDltArg>());
            Assert.That(((UnsignedIntDltArg)arg).Data, Is.EqualTo(unchecked((long)result)));
            Assert.That(((UnsignedIntDltArg)arg).DataUnsigned, Is.EqualTo(result));
        }

        private static void DecodeHexUint(byte[] buffer, bool msbf, ulong result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<HexIntDltArg>());
            Assert.That(arg, Is.InstanceOf<IntDltArg>());
            Assert.That(((HexIntDltArg)arg).Data, Is.EqualTo(unchecked((long)result)));
        }

        private static void DecodeBinUint(byte[] buffer, bool msbf, ulong result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<BinaryIntDltArg>());
            Assert.That(arg, Is.InstanceOf<IntDltArg>());
            Assert.That(((BinaryIntDltArg)arg).Data, Is.EqualTo(unchecked((long)result)));
        }

        private static void DecodeUint128(byte[] buffer, bool msbf, int encoding)
        {
            ArgDecoderTest.DecodeUnknown<T>(buffer, msbf, encoding, UnsignedArgType);
        }
    }
}
