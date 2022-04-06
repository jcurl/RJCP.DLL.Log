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

        [TestCase(OutputFormat.Automatic)]
        [TestCase(OutputFormat.Console)]
        public void CreateConsoleWithNull(OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, null)) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [TestCase(OutputFormat.Automatic)]
        [TestCase(OutputFormat.Console)]
        public void CreateConsoleWithCon(OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "CON:")) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [TestCase(OutputFormat.Automatic)]
        [TestCase(OutputFormat.Console)]
        public void CreateConsoleWithDev(OutputFormat outputFormat)
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
        public void CreateUnkonwn()
        {
            // This test case will become obsolete when files are supported for writing.

            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(OutputFormat.Automatic, "file.dlt")) {
                Assert.That(output, Is.Null);
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

        [TestCase(OutputFormat.Automatic)]
        [TestCase(OutputFormat.Text)]
        public void CreateTextOutputWithFile(OutputFormat outputFormat)
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(outputFormat, "file.txt")) {
                Assert.That(output, Is.TypeOf<TextOutput>());
            }
        }
    }
}
