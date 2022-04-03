namespace RJCP.App.DltDump.Domain
{
    using NUnit.Framework;
    using OutputStream;

    [TestFixture]
    public class OutputStreamFactoryTest
    {
        [Test]
        public void CreateConsoleWithNull()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(OutputFormat.Automatic, null)) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateConsoleWithCon()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(OutputFormat.Automatic, "CON:")) {
                Assert.That(output, Is.TypeOf<ConsoleOutput>());
            }
        }

        [Test]
        public void CreateConsoleWithDev()
        {
            IOutputStreamFactory factory = new OutputStreamFactory();
            using (IOutputStream output = factory.Create(OutputFormat.Automatic, "/dev/stdout")) {
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
            using (IOutputStream output = factory.Create(OutputFormat.Automatic, "file.txt")) {
                Assert.That(output, Is.Null);
            }
        }
    }
}
