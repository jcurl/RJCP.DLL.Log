namespace RJCP.App.DltDump.Application
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class FilterAppTest
    {
        private readonly string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        // Most of the functionality of FilterApp is tested through the FilterCommandTest. This is because it takes a
        // CmdOptions as input which requires the CommandLine.Run() method to be called. Anything that can't be tested
        // via an integration test can be tested additionally here.

        [Test]
        public void NullConfig()
        {
            Assert.That(() => { _ = new FilterApp(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task NoFiles()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            FilterApp app = new FilterApp(config);
            ExitCode result = await app.Run();
            Assert.That(result, Is.EqualTo(ExitCode.OptionsError));
        }

        [Test]
        public void NullFileList()
        {
            Assert.That(() => { _ = new FilterConfig(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task OpenNoError()
        {
            using (new TestApplication()) {
                FilterConfig config = new FilterConfig(new[] { file });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [TestCase(TestDltTraceReaderFactory.FileOpenError.ArgumentNullException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.ArgumentException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.NotSupportedException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.FileNotFoundException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.IOException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.SecurityException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.DirectoryNotFoundException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.UnauthorizedAccessException)]
        [TestCase(TestDltTraceReaderFactory.FileOpenError.PathTooLongException)]
        public async Task OpenErrorArgumentNullException(TestDltTraceReaderFactory.FileOpenError openError)
        {
            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).OpenError = openError;

                FilterConfig config = new FilterConfig(new[] { file });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
            }
        }

        [Test]
        public void OpenErrorUnhandledException()
        {
            // An exception occurring here won't be caught, and will bubble up to the top level,
            // so that a core dump can be captured. This exception is not documented by .NET.

            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).OpenError =
                    TestDltTraceReaderFactory.FileOpenError.InvalidOperationException;

                FilterConfig config = new FilterConfig(new[] { file });
                FilterApp app = new FilterApp(config);
                Assert.That(async () => {
                    await app.Run();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }
    }
}
