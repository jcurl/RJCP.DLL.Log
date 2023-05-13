namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class StringArgEncoderTest
    {
        [TestCase("München", true, false, 8, TestName = "Encode_LittleEndian_Utf8")]
        [TestCase("", true, false, 0, TestName = "Encode_LittleEndian_Utf8Empty")]
        [TestCase("München", true, true, 8, TestName = "Encode_BigEndian_Utf8")]
        [TestCase("", true, true, 0, TestName = "Encode_BigEndian_Utf8Empty")]

        [TestCase("München", false, false, 8, TestName = "EncodeNv_LittleEndian_Utf8")]
        [TestCase("", false, false, 0, TestName = "EncodeNv_LittleEndian_Utf8Empty")]
        [TestCase("München", false, true, 8, TestName = "EncodeNv_BigEndian_Utf8")]
        [TestCase("", false, true, 0, TestName = "EncodeNv_BigEndian_Utf8Empty")]
        public void EncodeUtf8(string value, bool verbose, bool msbf, int expLen)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 2 + expLen];
            StringDltArg arg = new StringDltArg(value, StringEncodingType.Utf8);
            IArgEncoder encoder = new StringArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x82, 0x00 } :
                    new byte[] { 0x00, 0x82, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0);
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !msbf));
            string result = Encoding.UTF8.GetString(payload[2..(2 + len)]);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_Utf8Full")]
        [TestCase(true, true, TestName = "Encode_BigEndian_Utf8Full")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_Utf8Full")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_Utf8Full")]
        public void EncodeUtf8Max(bool verbose, bool msbf)
        {
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65533 - (verbose ? 4 : 0));

            EncodeUtf8(sb.ToString(), verbose, msbf, sb.Length);
        }

        [TestCase("Muenchen", true, false, 8, TestName = "Encode_LittleEndian_ASCII")]
        [TestCase("", true, false, 0, TestName = "Encode_LittleEndian_ASCIIEmpty")]
        [TestCase("Muenchen", true, true, 8, TestName = "Encode_BigEndian_ASCII")]
        [TestCase("", true, true, 0, TestName = "Encode_BigEndian_ASCIIEmpty")]

        [TestCase("Muenchen", false, false, 8, TestName = "EncodeNv_LittleEndian_ASCII")]
        [TestCase("", false, false, 0, TestName = "EncodeNv_LittleEndian_ASCIIEmpty")]
        [TestCase("Muenchen", false, true, 8, TestName = "EncodeNv_BigEndian_ASCII")]
        [TestCase("", false, true, 0, TestName = "EncodeNv_BigEndian_ASCIIEmpty")]
        public void EncodeAscii(string value, bool verbose, bool msbf, int expLen)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 2 + expLen];
            StringDltArg arg = new StringDltArg(value, StringEncodingType.Ascii);
            IArgEncoder encoder = new StringArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x02, 0x00 } :
                    new byte[] { 0x00, 0x02, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0);
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !msbf));
            string result = Encoding.ASCII.GetString(payload[2..(2 + len)]);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(true, false, TestName = "Encode_LittleEndian_AsciiFull")]
        [TestCase(true, true, TestName = "Encode_BigEndian_AsciiFull")]

        [TestCase(false, false, TestName = "EncodeNv_LittleEndian_AsciiFull")]
        [TestCase(false, true, TestName = "EncodeNv_BigEndian_AsciiFull")]
        public void EncodeAsciiMax(bool verbose, bool msbf)
        {
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65534 - (verbose ? 4 : 0));

            EncodeAscii(sb.ToString(), verbose, msbf, sb.Length);
        }

        [TestCase(true, false, StringEncodingType.Utf8, TestName = "Encode_LittleEndian_Utf8OverSize")]
        [TestCase(true, true, StringEncodingType.Utf8, TestName = "Encode_BigEndian_Utf8OverSize")]
        [TestCase(false, false, StringEncodingType.Utf8, TestName = "EncodeNv_LittleEndian_Utf8OverSize")]
        [TestCase(false, true, StringEncodingType.Utf8, TestName = "EncodeNv_BigEndian_Utf8OverSize")]

        [TestCase(true, false, StringEncodingType.Ascii, TestName = "Encode_LittleEndian_AsciiOverSize")]
        [TestCase(true, true, StringEncodingType.Ascii, TestName = "Encode_BigEndian_Ascii8OverSize")]
        [TestCase(false, false, StringEncodingType.Ascii, TestName = "EncodeNv_LittleEndian_AsciiOverSize")]
        [TestCase(false, true, StringEncodingType.Ascii, TestName = "EncodeNv_BigEndian_AsciiOverSize")]
        public void EncodeStringOverSize(bool verbose, bool msbf, StringEncodingType strType)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 2 + 65535];
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65535 - (verbose ? 4 : 0));

            StringDltArg arg = new StringDltArg(sb.ToString(), strType);
            IArgEncoder encoder = new StringArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
