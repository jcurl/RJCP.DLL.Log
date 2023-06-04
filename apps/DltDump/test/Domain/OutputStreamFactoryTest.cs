namespace RJCP.App.DltDump.Domain
{
    using System;
    using NUnit.Framework;
    using OutputStream;

    [TestFixture]
    public class OutputStreamFactoryTest
    {
        [Test]
        public void OutputStreamFactoryDefaults()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            Assert.That(factory.Force, Is.False);
            Assert.That(factory.Split, Is.EqualTo(0));
        }

        [Test]
        public void CreateConsoleWithNull(
            [Values(OutputFormat.Automatic, OutputFormat.Console)] OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, null)) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateConsoleWithCon(
            [Values(OutputFormat.Automatic, OutputFormat.Console)] OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "CON:")) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateConsoleWithDev(
            [Values(OutputFormat.Automatic, OutputFormat.Console)] OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "/dev/stdout")) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateConsoleOutputFormat()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(OutputFormat.Console, "file.txt")) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateTextOutputWithNull()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            Assert.That(() => {
                _ = factory.Create(OutputFormat.Text, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CreateTextOutputWithFile(
            [Values(OutputFormat.Automatic, OutputFormat.Text)] OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "file.txt")) {
                Assert.That(output, Is.TypeOf<TextOutput>());
            }
        }

        [Test]
        public void CreateDltOutputWithNull()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            Assert.That(() => {
                _ = factory.Create(OutputFormat.Dlt, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CreateDltOutputWithFile(
            [Values(OutputFormat.Automatic, OutputFormat.Dlt)] OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "file.dlt")) {
                Assert.That(output, Is.TypeOf<DltOutput>());
            }
        }

        [Test]
        public void CreateDltOutputWithConvert(
            [Values(OutputFormat.Automatic, OutputFormat.Dlt)] OutputFormat outputFormat,
            [Values(false, true)] bool nvConvert)
        {
            IOutputStreamFactory factory = new OutputStreamFactory() {
                ConvertNonVerbose = nvConvert
            };
            using (IOutputStream output = factory.Create(outputFormat, "file.dlt")) {
                Assert.That(output, Is.TypeOf<DltOutput>());
                Assert.That(((DltOutput)output).ConvertNonVerbose, Is.EqualTo(nvConvert));
            }
        }
    }
}
