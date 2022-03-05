namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Domain;
    using Domain.InputStream;
    using Infrastructure.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;

    [TestFixture]
    public class FilterAppTest
    {
        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

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
                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [TestCase(TestDltFileStreamFactory.FileOpenError.ArgumentNullException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.ArgumentException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.NotSupportedException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.FileNotFoundException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.IOException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.SecurityException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.DirectoryNotFoundException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.UnauthorizedAccessException)]
        [TestCase(TestDltFileStreamFactory.FileOpenError.PathTooLongException)]
        public async Task OpenErrorArgumentNullException(TestDltFileStreamFactory.FileOpenError openError)
        {
            // Tests that the FilterApp catches the InputStreamException. Is also a partial integration test for
            // InputStreamFactory that sees a file.

            using (new TestApplication()) {
                TestDltFileStreamFactory fileFactory = new TestDltFileStreamFactory {
                    OpenError = openError
                };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("file", fileFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile });
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
                TestDltFileStreamFactory fileFactory = new TestDltFileStreamFactory {
                    OpenError = TestDltFileStreamFactory.FileOpenError.InvalidOperationException
                };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("file", fileFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                Assert.That(async () => {
                    await app.Run();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public async Task OpenUnknownUri()
        {
            using (TestApplication global = new TestApplication()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { "unknown://" });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenConnectError()
        {
            using (TestApplication global = new TestApplication()) {
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory() {
                    ConnectResult = false
                };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { "net://127.0.0.1" });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        private readonly DltTraceLine line1 = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.VerboseFeature + DltLineFeatures.MessageTypeFeature +
                DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        [Test]
        public async Task ShowSingleLine()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(line1);

                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 0 log info verbose 1 Message 1",
                    line1.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task ShowSingleLinePosition()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(line1);

                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ShowPosition = true
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));

                string expectedLine = string.Format("00000003: {0} 80.5440 127 ECU1 APP1 CTX1 0 log info verbose 1 Message 1",
                    line1.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }
    }
}
