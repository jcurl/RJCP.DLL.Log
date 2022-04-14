namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using Infrastructure.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using TestResources;

    [TestFixture]
    public class DltOutputTest
    {
        [Test]
        public void NullFileName()
        {
            Assert.That(() => {
                _ = new DltOutput(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyFileName()
        {
            Assert.That(() => {
                _ = new DltOutput(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void DefaultProperties()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.False);
            }
        }

        [Test]
        public void DefaultPropertiesWithForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt", true)) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.True);
            }
        }

        [Test]
        public void WriteLineAsPacket()
        {
            byte[] data = new byte[] { 0x35 };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                // If this writes binary data, it would just write 1 byte, not the length.
                output.Write(TestLines.Verbose, data.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void WriteLinesAsPacket()
        {
            byte[] data = new byte[] { 0x35, 0x00 };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                // If this writes binary data, it would just write 1 byte, not the length.
                output.Write(TestLines.Verbose, data.AsSpan());
                output.Write(TestLines.Verbose2, data.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(4));
            }
        }

        [Test]
        public void SetInputNullString()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput(null, InputFormat.File);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void SetInputUnknownFormat()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", (InputFormat)255);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputUnsupportedFormatAutomatic()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", InputFormat.Automatic);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputValid()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);

                // The file is still only created when written.
                Assert.That(File.Exists("File.dlt"), Is.False);
            }
        }
    }
}
