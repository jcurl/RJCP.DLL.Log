namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using Domain.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using TestResources;

    [TestFixture]
    public class TextOutputTest
    {
        [Test]
        public void NullFileName()
        {
            Assert.That(() => {
                _ = new TextOutput(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyFileName()
        {
            Assert.That(() => {
                _ = new TextOutput(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void DefaultProperties()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);
                Assert.That(output.SupportsBinary, Is.False);
                Assert.That(output.Force, Is.False);
            }
        }

        [Test]
        public void DefaultPropertiesWithForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt", true)) {
                Assert.That(File.Exists("File.txt"), Is.False);
                Assert.That(output.SupportsBinary, Is.False);
                Assert.That(output.Force, Is.True);
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteTextLine(InputFormat inputFormat)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                output.SetInput("input.dlt", inputFormat);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteTextLineWithPosition(InputFormat inputFormat)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                output.ShowPosition = true;

                Assert.That(File.Exists("File.txt"), Is.False);

                output.SetInput("input.dlt", inputFormat);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(10 + TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(InputFormat.File)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Pcap)]
        public void WriteTextLineAsPacket(InputFormat inputFormat)
        {
            byte[] data = new byte[] { 0x00 };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                output.SetInput("input.dlt", inputFormat);
                // If this writes binary data, it would just write 1 byte, not the length.
                output.Write(TestLines.Verbose, data.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringInitNoFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringInitFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File_%FILE%.txt")) {
                Assert.That(File.Exists("File_.txt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File_.txt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File_.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File_%FILE%.txt")) {
                Assert.That(File.Exists("File_.txt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File_.txt"), Is.True);

                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File_.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(2 * (TestLines.Verbose.ToString().Length + Environment.NewLine.Length)));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputFromValidToNullStringFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File_%FILE%.txt")) {
                Assert.That(File.Exists("File_.txt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File_input.txt"), Is.True);
                FileInfo fileInfo = new("File_input.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));

                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                // The data is written exactly as the packet says. There is no interpretation of the data.
                Assert.That(File.Exists("File_.txt"), Is.True);
                FileInfo fileInfoEmpty = new("File_.txt");
                Assert.That(fileInfoEmpty.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void SetInputUnknownFormat()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", (InputFormat)255);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputUnsupportedFormatAutomatic()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", InputFormat.Automatic);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputValid()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TextOutput output = new("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);

                // The file is still only created when written.
                Assert.That(File.Exists("File.txt"), Is.False);
            }
        }
    }
}
