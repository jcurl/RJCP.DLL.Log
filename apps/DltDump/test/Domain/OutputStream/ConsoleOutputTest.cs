namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System.Globalization;
    using Domain.Dlt;
    using NUnit.Framework;
    using TestResources;

    [TestFixture]
    public class ConsoleOutputTest
    {
        [Test]
        public void Initialize()
        {
            using (IOutputStream output = new ConsoleOutput()) {
                Assert.That(output.SupportsBinary, Is.False);
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteLine(InputFormat inputFormat)
        {
            using (TestApplication global = new TestApplication())
            using (IOutputStream output = new ConsoleOutput()) {
                output.SetInput("input.dlt", inputFormat);
                output.Write(TestLines.Verbose);
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteLinePacket(InputFormat inputFormat)
        {
            using (TestApplication global = new TestApplication())
            using (IOutputStream output = new ConsoleOutput()) {
                output.SetInput("input.dlt", inputFormat);
                output.Write(TestLines.Verbose, new byte[] { 0x01 });
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteLineWithNoPosition(InputFormat inputFormat)
        {
            using (TestApplication global = new TestApplication())
            using (IOutputStream output = new ConsoleOutput(false)) {
                output.SetInput("input.dlt", inputFormat);
                output.Write(TestLines.Verbose);
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public void WriteLineWithPosition()
        {
            using (TestApplication global = new TestApplication())
            using (IOutputStream output = new ConsoleOutput(true)) {
                output.Write(TestLines.Verbose);
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));

                string expectedLine = string.Format("00000003: {0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }
    }
}
