namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(StringArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(StringArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(StringArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(StringArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Little, LineType.Verbose)]
    public class StringArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public StringArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase("München", 8, TestName = "Encode_Utf8")]
        [TestCase("", 0, TestName = "Encode_Utf8Empty")]
        public void EncodeUtf8(string value, int expLen)
        {
            Span<byte> buffer = ArgEncode(new StringDltArg(value, StringEncodingType.Utf8), expLen + 3);
            Assert.That(buffer.Length, Is.EqualTo((IsVerbose ? 4 : 0) + expLen + 3));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x82, 0x00 } :
                    new byte[] { 0x00, 0x82, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer[(IsVerbose ? 4 : 0)..];
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
            string result = Encoding.UTF8.GetString(payload[2..(2 + len - 1)]);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(TestName = "Encode_Utf8Full")]
        public void EncodeUtf8Max()
        {
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65535 - (IsVerbose ? 4 : 0) - 3 - HeaderLen);

            EncodeUtf8(sb.ToString(), sb.Length);
        }

        [TestCase("Muenchen", 8, TestName = "Encode_ASCII")]
        [TestCase("", 0, TestName = "Encode_ASCIIEmpty")]
        public void EncodeAscii(string value, int expLen)
        {
            Span<byte> buffer = ArgEncode(new StringDltArg(value, StringEncodingType.Ascii), expLen + 3);
            Assert.That(buffer.Length, Is.EqualTo((IsVerbose ? 4 : 0) + expLen + 3));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x02, 0x00 } :
                    new byte[] { 0x00, 0x02, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer[(IsVerbose ? 4 : 0)..];
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
            string result = Encoding.ASCII.GetString(payload[2..(2 + len - 1)]);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(TestName = "Encode_AsciiFull")]
        public void EncodeAsciiMax()
        {
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65535 - (IsVerbose ? 4 : 0) - 3 - HeaderLen);

            EncodeAscii(sb.ToString(), sb.Length);
        }

        [TestCase(StringEncodingType.Utf8, TestName = "Encode_Utf8OverSize")]
        [TestCase(StringEncodingType.Ascii, TestName = "Encode_AsciiOverSize")]
        public void EncodeStringOverSize(StringEncodingType strType)
        {
            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen + 2 + 65535];
            StringBuilder sb = new StringBuilder(65536);
            sb.Append('x', 65535 - (IsVerbose ? 4 : 0));

            ArgEncode(buffer, new StringDltArg(sb.ToString(), strType), out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
