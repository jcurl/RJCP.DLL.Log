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

        [TestCase(FileOpenError.ArgumentNullException)]
        [TestCase(FileOpenError.ArgumentException)]
        [TestCase(FileOpenError.NotSupportedException)]
        [TestCase(FileOpenError.FileNotFoundException)]
        [TestCase(FileOpenError.IOException)]
        [TestCase(FileOpenError.SecurityException)]
        [TestCase(FileOpenError.DirectoryNotFoundException)]
        [TestCase(FileOpenError.UnauthorizedAccessException)]
        [TestCase(FileOpenError.PathTooLongException)]
        public async Task OpenErrorArgumentNullException(FileOpenError openError)
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
                    OpenError = FileOpenError.InvalidOperationException
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
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                testFactory.ConnectEvent += (s, e) => { e.Succeed = false; };
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

        public enum ConnectTestMode
        {
            ConnectFail,
            RetryThenCreateFail,
            RetryThenFail
        }

        [TestCase(ConnectTestMode.ConnectFail)]
        [TestCase(ConnectTestMode.RetryThenCreateFail)]
        [TestCase(ConnectTestMode.RetryThenFail)]
        public async Task OpenConnectErrorRetries(ConnectTestMode mode)
        {
            const int RetryOption = 7;   // Option given to FilterApp
            const int ConnectCount = 2;  // Our test case fails to connect after this count
            const int RetryCount = 4;    // Our test case connects after this many retries

            int connects = ConnectCount;
            int retries = RetryCount;

            ExitCode expectedResult;
            int expectedConnects;
            int actualConnects = 0;

            using (TestApplication global = new TestApplication()) {
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                switch (mode) {
                case ConnectTestMode.ConnectFail:
                    // Connection will never succeed.
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        e.Succeed = false;
                    };
                    expectedResult = ExitCode.NoFilesProcessed;
                    expectedConnects = RetryOption + 1;
                    break;
                case ConnectTestMode.RetryThenCreateFail:
                    // For loop 1, it fails the first two times and then connects.
                    // For loop 2, it fails the first two times and then connects.
                    // For loop 3, the connection has an error.
                    testFactory.CreateEvent += (s, e) => {
                        --connects;
                        retries = RetryCount;
                        e.Succeed = connects >= 0;
                    };
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0;
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = 2 * (RetryCount + 1);
                    break;
                case ConnectTestMode.RetryThenFail:
                    // For loop 1, it fails the first two times and then connects.
                    // For loop 2, it fails the first two times and then connects.
                    // For loop 3, it will never connect, thus ending the connect infinite loop
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0 && connects > 0;
                        if (e.Succeed) {
                            retries = RetryCount;
                            --connects;
                        }
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = 2 * (RetryCount + 1) + RetryOption + 1;
                    break;
                default:
                    Assert.Fail("Unknown test case");
                    return;
                }
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { "net://127.0.0.1" }) {
                    ConnectRetries = RetryOption
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(expectedResult));

                // 3 lines to indicate connecting, the 4th line that it failed.
                Assert.That(actualConnects, Is.EqualTo(expectedConnects));
                Assert.That(global.StdOut.Lines.Count, Is.Not.EqualTo(0));
            }
        }

        [Test]
        public async Task OpenRetriesFile()
        {
            using (TestApplication global = new TestApplication()) {
                // The number of retries should be ignored for streams that aren't live.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ConnectRetries = -1
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
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

        [TestCase(InputFormat.Automatic, true)]
        [TestCase(InputFormat.Network, true)]
        [TestCase(InputFormat.Serial, true)]
        [TestCase(InputFormat.File, false)]
        public async Task OnlineModeForNetwork(InputFormat inputFormat, bool onlineMode)
        {
            using (TestApplication global = new TestApplication()) {
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { "net://127.0.0.1" }) {
                    InputFormat = inputFormat
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();
                // We're in online mode, as per the InputFormat
                Assert.That(Global.Instance.DltReaderFactory.OnlineMode, Is.EqualTo(onlineMode));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [TestCase(InputFormat.Automatic)]
        [TestCase(InputFormat.Network)]
        [TestCase(InputFormat.Serial)]
        [TestCase(InputFormat.File)]
        public async Task OfflineModeForNetwork(InputFormat inputFormat)
        {
            using (TestApplication global = new TestApplication()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    InputFormat = inputFormat
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(Global.Instance.DltReaderFactory.OnlineMode, Is.False);
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }
    }
}
