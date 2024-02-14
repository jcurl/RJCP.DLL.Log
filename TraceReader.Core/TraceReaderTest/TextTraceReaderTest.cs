namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RJCP.CodeQuality.IO;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class TextTraceReaderTest
    {
        [SetUp]
        public void InitFixture()
        {
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        // See TextDecoderBase.BufferLength. This is a private variable in that class, this test case ensures that
        // the boundary condition works as expected, that the line is truncated to this length.
        private const int BufferLength = 4096;

        private static Stream GetStream(byte[] readBuffer, int maxBytes)
        {
            if (maxBytes < BufferLength / 2)
                return new ReadLimitStream(readBuffer, maxBytes);

            return new ReadLimitStream(readBuffer, maxBytes, maxBytes);
        }

        private static async Task TextStreamCheck(ITraceReader<TraceLine> reader, IEnumerable<TraceLine> lines)
        {
            TraceLine line;
            foreach (TraceLine expectedLine in lines) {
                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo(expectedLine.Text));
                Assert.That(line.Line, Is.EqualTo(expectedLine.Line),
                    $"Expected Line {expectedLine.Line}; got {line.Line} for '{expectedLine.Text}'");
                Assert.That(line.Position, Is.EqualTo(expectedLine.Position),
                    $"Expected Pos {expectedLine.Position}; got {line.Position} for '{expectedLine.Text}'");
            }
            line = await reader.GetLineAsync();
            Assert.That(line, Is.Null);
        }

        static readonly int[] ReadChunks = { 0, 1, 2, 3, 5, 10, 100 };

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamLinux(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\nLine2\nLin3\n");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 6),
                    new("Lin3", 2, 12)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamWindows(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\r\nLine2\r\nLin3\r\n");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 7),
                    new("Lin3", 2, 14)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamMac(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\rLine2\rLin3\r");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 6),
                    new("Lin3", 2, 12)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamMixed1(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\rLine2\nLin3\r\n");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 6),
                    new("Lin3", 2, 12)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamMixed2(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\nLine2\r\nLin3\r");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 6),
                    new("Lin3", 2, 13)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamNoEndLine1(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamNoEndLine2(int maxBytes)
        {
            byte[] readBuffer = Encoding.UTF8.GetBytes("Line1\r\nLine2");
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 7)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public async Task TextStreamWindowsIso8859_15(int maxBytes)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-15");
            byte[] readBuffer = {
                0x4c, 0x69, 0x6e, 0x65, 0x31, 0x0d, 0x0a, 0x4c,
                0x69, 0x6e, 0x65, 0x32, 0x0d, 0x0a, 0x4c, 0x69,
                0x6e, 0xa4, 0x33, 0x0d, 0x0a
            };
            using (Stream stream = GetStream(readBuffer, maxBytes)) {
                ITraceReaderFactory<TraceLine> factory = new TextTraceReaderFactory() {
                    Encoding = encoding
                };
                ITraceReader<TraceLine> reader = await factory.CreateAsync(stream);
                TraceLine[] expectedLines = {
                    new("Line1", 0, 0),
                    new("Line2", 1, 7),
                    new("Lin€3", 2, 14)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        public async Task SingleLineBufferLengthOverflow(int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 2];

            string firstLine = new('a', BufferLength);
            for (int i = 0; i < BufferLength; i++) {
                largeBuffer[i] = (byte)'a';
            }

            for (int i = BufferLength; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("bb", 1, BufferLength)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        public async Task SingleLineBufferLength(int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength - 1; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength - 1] = (byte)'b';
            sb.Append('b');
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCase('\n', 0)]
        [TestCase('\r', 0)]
        [TestCase('\n', 30)]
        [TestCase('\r', 30)]
        public async Task BufferLengthNewLine(char newLine, int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength - 1; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength - 1] = (byte)newLine;
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCase('\n', 0)]
        [TestCase('\r', 0)]
        [TestCase('\n', 30)]
        [TestCase('\r', 30)]
        public async Task BufferLengthNewLineOverflow(char newLine, int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 2];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength - 1; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength - 1] = (byte)newLine;

            for (int i = BufferLength; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("bb", 1, BufferLength)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        public async Task BufferLengthNewLineOverflowWindows(int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 2];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength - 2; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength - 2] = (byte)'\r';
            largeBuffer[BufferLength - 1] = (byte)'\n';

            for (int i = BufferLength; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("bb", 1, BufferLength)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        public async Task BufferLengthNewLineOverflowWindowsSplit(int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 2];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength - 1; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength - 1] = (byte)'\r';
            largeBuffer[BufferLength] = (byte)'\n';

            for (int i = BufferLength; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("bb", 1, BufferLength)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCase('\n', 0)]
        [TestCase('\r', 0)]
        [TestCase('\n', 30)]
        [TestCase('\r', 30)]
        public async Task BufferLengthOverflowNewLine(char newLine, int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 5];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength] = (byte)newLine;

            for (int i = BufferLength + 1; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("", 1, BufferLength),
                    new("bbbb", 2, BufferLength + 1)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        public async Task BufferLengthOverflowWindowsNewLine(int maxBytes)
        {
            byte[] largeBuffer = new byte[BufferLength + 6];
            StringBuilder sb = new();
            for (int i = 0; i < BufferLength; i++) {
                largeBuffer[i] = (byte)'a';
                sb.Append('a');
            }
            largeBuffer[BufferLength] = (byte)'\r';
            largeBuffer[BufferLength + 1] = (byte)'\n';

            for (int i = BufferLength + 2; i < largeBuffer.Length; i++) {
                largeBuffer[i] = (byte)'b';
            }
            string firstLine = sb.ToString();

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine[] expectedLines = {
                    new(firstLine, 0, 0),
                    new("", 1, BufferLength),
                    new("bbbb", 2, BufferLength + 2)
                };
                await TextStreamCheck(reader, expectedLines);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        [TestCase(BufferLength - 1)]
        [TestCase(BufferLength)]
        [TestCase(BufferLength + 1)]
        [TestCase(BufferLength + 2)]
        [TestCase(BufferLength + 3)]
        [TestCase(BufferLength + 4)]
        public async Task BufferBoundary4Utf8(int maxBytes)
        {
            // When one tries to decode a 4-byte UTF8 to a 2-char UTF16 so that at the end of the array there is only
            // one char available, the decoder will raise an exception.
            byte[] largeBuffer = new byte[BufferLength + 3];
            for (int i = 0; i < BufferLength - 1; i++) {
                largeBuffer[i] = (byte)'a';
            }
            string firstLine = new('a', BufferLength - 1);

            // Decodes to \uDB40\uDC84
            largeBuffer[BufferLength - 1] = 0xF3;
            largeBuffer[BufferLength] = 0xA0;
            largeBuffer[BufferLength + 1] = 0x82;
            largeBuffer[BufferLength + 2] = 0x84;

            using (Stream stream = GetStream(largeBuffer, maxBytes))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo(firstLine));
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.Position, Is.EqualTo(0));

                // On reading the second line, the actual position where the line starts is approximate. It is not
                // possible to calculate an exact position as the .NET decoder caches bytes as they're read, and the
                // implementation doesn't know the position of the data that is being cached. So in the case that the
                // ITraceDecoder.Decode method is given partial byte sequence, it might return a position that is inside
                // the UTF16 character sequence being returned. That is the case for all sequences, not just this test.
                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("\uDB40\uDC84"));
                Assert.That(line.Line, Is.EqualTo(1));
                Assert.That(line.Position, Is.GreaterThanOrEqualTo(BufferLength - 2).Or.LessThanOrEqualTo(BufferLength));
            }
        }

        [Test]
        public async Task EmptyBuffer()
        {
            byte[] emptyBuffer = Array.Empty<byte>();

            using (Stream stream = new MemoryStream(emptyBuffer))
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(stream)) {
                TraceLine line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
            }
        }

        [Test]
        public async Task ReadFile()
        {
            string path = Path.Combine(Deploy.TestDirectory, "TestResources", "TextFiles", "TextFile.txt");
            using (ITraceReader<TraceLine> reader = await new TextTraceReaderFactory().CreateAsync(path)) {
                TraceLine line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("This is Line 1"));
                Assert.That(line.Line, Is.EqualTo(0));
                Assert.That(line.Position, Is.EqualTo(0));

                // The external file could be Windows or Linux format (depending on the Revision control system, and how
                // it checks the file out), which changes the position.
                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("And now for Line 2."));
                Assert.That(line.Line, Is.EqualTo(1));
                Assert.That(line.Position, Is.EqualTo(15).Or.EqualTo(16));
            }
        }
    }
}
