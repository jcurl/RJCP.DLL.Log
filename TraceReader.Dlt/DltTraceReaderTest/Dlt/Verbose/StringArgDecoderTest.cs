namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(StringArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class StringArgDecoderTest<T> where T : IVerboseArgDecoder
    {
        [Test]
        public void DecodeUtf8StringLE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x09, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, false, StringEncodingType.Utf8, "München");
        }

        [Test]
        public void DecodeUtf8StringBE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x00, 0x09, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, true, StringEncodingType.Utf8, "München");
        }

        [Test]
        public void DecodeUtf8StringNoNulLE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x08, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E
            }, false, StringEncodingType.Utf8, "München");
        }

        [Test]
        public void DecodeUtf8StringNoNulBE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x00, 0x08, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E
            }, true, StringEncodingType.Utf8, "München");
        }

        [Test]
        public void DecodeAsciiStringLE()
        {
            DecodeString(new byte[] {
                0x00, 0x02, 0x00, 0x00, 0x09, 0x00, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, false, StringEncodingType.Ascii, "Muenchen");
        }

        [Test]
        public void DecodeAsciiStringBE()
        {
            DecodeString(new byte[] {
                0x00, 0x02, 0x00, 0x00, 0x00, 0x09, 0x4D, 0x75, 0x65, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, true, StringEncodingType.Ascii, "Muenchen");
        }

        [Test]
        public void DecodeAsciiStringAsIsoLE()
        {
            DecodeString(new byte[] {
                0x00, 0x02, 0x00, 0x00, 0x08, 0x00, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, false, StringEncodingType.Ascii, "München");
        }

        [Test]
        public void DecodeAsciiStringAsIsoBE()
        {
            DecodeString(new byte[] {
                0x00, 0x02, 0x00, 0x00, 0x00, 0x08, 0x4D, 0xFC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, true, StringEncodingType.Ascii, "München");
        }

        [Test]
        public void DecodeEmptyStringLE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x01, 0x00, 0x00
            }, false, StringEncodingType.Utf8, string.Empty);
        }

        [Test]
        public void DecodeEmptyStringBE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x00, 0x01, 0x00
            }, true, StringEncodingType.Utf8, string.Empty);
        }

        [Test]
        public void DecodeEmptyStringNoNulLE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x00, 0x00
            }, false, StringEncodingType.Utf8, string.Empty);
        }

        [Test]
        public void DecodeEmptyStringNoNulBE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x00, 0x00, 0x00, 0x00
            }, true, StringEncodingType.Utf8, string.Empty);
        }

        [Test]
        public void DecodeUnknownCodingAsUtf8StringLE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x03, 0x00, 0x09, 0x00, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, false, (StringEncodingType)7, "München");
        }

        [Test]
        public void DecodeUnknownCodingAsUtf8StringBE()
        {
            DecodeString(new byte[] {
                0x00, 0x82, 0x03, 0x00, 0x00, 0x09, 0x4D, 0xC3, 0xBC, 0x6E, 0x63, 0x68, 0x65, 0x6E, 0x00
            }, true, (StringEncodingType)7, "München");
        }

        [Test]
        public void DecodeAsciiLargePayloadLE()
        {
            byte[] data = new byte[65000];
            Random r = new Random();

            data[0] = 0x00;
            data[1] = 0x02;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = (byte)((data.Length - 6) & 0xFF);
            data[5] = (byte)((data.Length - 6) >> 8);

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 6; i < data.Length; i++) {
                data[i] = (byte)r.Next(32, 126);
                strBuilder.Append((char)data[i]);
            }

            DecodeString(data, false, StringEncodingType.Ascii, strBuilder.ToString());
        }

        [Test]
        public void DecodeAsciiLargePayloadBE()
        {
            byte[] data = new byte[65000];
            Random r = new Random();

            data[0] = 0x00;
            data[1] = 0x02;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = (byte)((data.Length - 6) >> 8);
            data[5] = (byte)((data.Length - 6) & 0xFF);

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 6; i < data.Length; i++) {
                data[i] = (byte)r.Next(32, 126);
                strBuilder.Append((char)data[i]);
            }

            DecodeString(data, true, StringEncodingType.Ascii, strBuilder.ToString());
        }

        private static void DecodeString(byte[] buffer, bool msbf, StringEncodingType coding, string result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<StringDltArg>());
            Assert.That(((StringDltArg)arg).Data, Is.EqualTo(result));
            Assert.That(((StringDltArg)arg).Coding, Is.EqualTo(coding));
        }
    }
}
