namespace RJCP.Diagnostics.Log.Text
{
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class Iso8859_1Test
    {
        [Test]
        public void ConvertFromBytesControl()
        {
            for (int i = 0; i < 32; i++)
                ConvertFromBytesControl(i);

            for (int i = 127; i < 160; i++)
                ConvertFromBytesControl(i);
        }

        private static void ConvertFromBytesControl(int i)
        {
            byte[] buffer = new byte[1] { (byte)i };
            char[] chars = new char[1];

            int cu = Iso8859_1Accessor.Convert(buffer, chars);
            Assert.That(cu, Is.EqualTo(1), $"Byte {i} wasn't converted, result: {cu}");
            Assert.That(chars[0], Is.EqualTo('.'), $"Byte {i} not converted to '.', result: {(int)chars[0]}");
        }

        [Test]
        public void ConvertFromBytes()
        {
            for (int i = 32; i < 127; i++)
                ConvertFromBytes(i);

            for (int i = 160; i < 256; i++)
                ConvertFromBytes(i);
        }

        [Test]
        public void ConvertFromBytesMoreChars()
        {
            byte[] buffer = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
            char[] chars = new char[1024];

            int cu = Iso8859_1Accessor.Convert(buffer, chars);
            Assert.That(cu, Is.EqualTo(5));
            Assert.That(chars[0..cu], Is.EqualTo(new char[] { 'A', 'B', 'C', 'D', 'E' }));
        }

        [Test]
        public void ConvertFromBytesFewerChars()
        {
            byte[] buffer = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
            char[] chars = new char[2];

            int cu = Iso8859_1Accessor.Convert(buffer, chars);
            Assert.That(cu, Is.EqualTo(2));
            Assert.That(chars[0..cu], Is.EqualTo(new char[] { 'A', 'B' }));
        }

        private static void ConvertFromBytes(int i)
        {
            Decoder decoder = Encoding.GetEncoding("ISO-8859-1").GetDecoder();
            byte[] buffer = new byte[1] { (byte)i };
            char[] chars = new char[1];
            char[] expected = new char[1];

            int conv = decoder.GetChars(buffer, expected, true);
            Assert.That(conv, Is.EqualTo(1));

            int cu = Iso8859_1Accessor.Convert(buffer, chars);
            Assert.That(cu, Is.EqualTo(1), $"Byte {i} wasn't converted, result: {cu}");
            Assert.That(chars[0], Is.EqualTo(expected[0]), $"Byte {i} not converted to '.', result: {(int)chars[0]}");
        }

        [Test]
        public void ConvertToBytesControl()
        {
            for (int i = 1; i < 32; i++)
                ConvertToBytesControl(i);

            for (int i = 127; i < 160; i++)
                ConvertToBytesControl(i);
        }

        private static void ConvertToBytesControl(int i)
        {
            byte[] buffer = new byte[1] { 0 };
            char[] chars = new char[1] { (char)i };

            int bu = Iso8859_1Accessor.Convert(new string(chars), buffer);
            Assert.That(bu, Is.EqualTo(1), $"Character {(int)chars[0]} wasn't converted, result: {bu}");
            Assert.That(buffer[0], Is.EqualTo(i), $"Character {(int)chars[0]} wasn't converted {(int)i}, result: {buffer[0]}");
        }

        [Test]
        public void ConvertToBytes()
        {
            for (int i = 32; i < 127; i++)
                ConvertToBytes(i);

            for (int i = 160; i < 256; i++)
                ConvertToBytes(i);
        }

        private static void ConvertToBytes(int i)
        {
            byte[] buffer = new byte[1] { (byte)i };
            char[] chars = new char[1];

            int cu = Iso8859_1Accessor.Convert(buffer, chars);
            Assert.That(cu, Is.EqualTo(1), $"Byte {i} wasn't converted, result: {cu}");

            buffer[0] = 0;
            int bu = Iso8859_1Accessor.Convert(new string(chars), buffer);
            Assert.That(bu, Is.EqualTo(1), $"Character {(int)chars[0]} wasn't converted, result: {bu}");
            Assert.That(buffer[0], Is.EqualTo(i), $"Character {(int)chars[0]} wasn't converted {i}, result: {buffer[0]}");
        }

        [Test]
        public void ConvertString()
        {
            string value = "Test";
            byte[] buffer = new byte[1024];

            int bu = Iso8859_1Accessor.Convert(value, buffer);
            Assert.That(bu, Is.EqualTo(4));
            Assert.That(buffer[0..4], Is.EqualTo(new byte[] { 0x54, 0x65, 0x73, 0x74 }));
        }

        [Test]
        public void ConvertNullTerminated()
        {
            string value = "Test\0Test2";
            byte[] buffer = new byte[1024];

            int bu = Iso8859_1Accessor.Convert(value, buffer);
            Assert.That(bu, Is.EqualTo(4));
            Assert.That(buffer[0..4], Is.EqualTo(new byte[] { 0x54, 0x65, 0x73, 0x74 }));
        }

        [Test]
        public void ConvertNullTerminatedZero()
        {
            string value = "\0";
            byte[] buffer = new byte[1024];

            int bu = Iso8859_1Accessor.Convert(value, buffer);
            Assert.That(bu, Is.EqualTo(0));
        }

        [Test]
        public void ConvertNullString()
        {
            byte[] buffer = new byte[1024];
            int bu = Iso8859_1Accessor.Convert(null, buffer);
            Assert.That(bu, Is.EqualTo(0));
        }

        [Test]
        public void ConvertEmptyString()
        {
            byte[] buffer = new byte[1024];
            int bu = Iso8859_1Accessor.Convert(string.Empty, buffer);
            Assert.That(bu, Is.EqualTo(0));
        }

        [Test]
        public void ConvertOutOfRange()
        {
            byte[] buffer = new byte[1024];

            // The € symbol is not in ISO-8859-1, so should be ignored.
            int bu = Iso8859_1Accessor.Convert("ABCD €135", buffer);
            Assert.That(bu, Is.EqualTo(8));
            Assert.That(buffer[0..bu], Is.EqualTo(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x20, 0x31, 0x33, 0x35 }));
        }
    }
}
