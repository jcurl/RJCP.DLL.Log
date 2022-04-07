namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Domain;
    using Domain.InputStream;
    using Infrastructure.Dlt;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using TestResources;

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
            Assert.That(result, Is.EqualTo(ExitCode.InputError));
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
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
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
            const int RetryOption = 7;      // Option given to FilterApp
            const int CreateCount = 2;      // Our test case fails to connect after this count
            const int RetryCount = 4;       // Our test case connects after this many retries

            ExitCode expectedResult;
            int retries = RetryCount;
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
                    int creates = CreateCount + 1;  // First create is to check
                    // For loop 1, it fails the first two times and then connects.
                    // For loop 2, it fails the first two times and then connects.
                    // For loop 3, the connection has an error.
                    testFactory.CreateEvent += (s, e) => {
                        --creates;
                        retries = RetryCount;
                        e.Succeed = creates >= 0;
                    };
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0;
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = CreateCount * (RetryCount + 1);
                    break;
                case ConnectTestMode.RetryThenFail:
                    int loop = CreateCount;
                    // For loop 1, it fails the first two times and then connects.
                    // For loop 2, it fails the first two times and then connects.
                    // For loop 3, it will never connect, thus ending the connect infinite loop
                    testFactory.ConnectEvent += (s, e) => {
                        Console.WriteLine("ConnectEvent");
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0 && loop > 0;  // Here is 1, because first create has no connect.
                        if (e.Succeed) {
                            retries = RetryCount;
                            --loop;
                        }
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = CreateCount * (RetryCount + 1) + RetryOption + 1;
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

        [Test]
        public async Task OpenMultipleMix1()
        {
            using (TestApplication global = new TestApplication()) {
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile, "net://127.0.0.1" });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenMultipleMix2()
        {
            using (TestApplication global = new TestApplication()) {
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { "net://127.0.0.1", EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenMultipleFile()
        {
            using (TestApplication global = new TestApplication()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile, EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ShowSingleLine()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task ShowSingleLinePosition()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ShowPosition = true
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));

                string expectedLine = string.Format("00000003: {0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
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

        [Test]
        public async Task BeforeContext()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    BeforeContext = 1,
                };
                config.AddAppId("APP2");

                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(2));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task BeforeContextNone()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    BeforeContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task AfterContext()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    AfterContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(3));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task AfterContextNone()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    AfterContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task OutputToConsole()
        {
            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "/dev/stdout"
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));

                string expectedLine = string.Format("00000003: {0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task OutputToTextFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file.txt"
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));

                Assert.That(File.Exists("file.txt"));

                FileInfo fileInfo = new FileInfo("file.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(10 + TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public async Task OutputToTextFileExists()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                using (Stream file = new FileStream("file.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                    /* Do nothing, just create the file */
                }

                FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file.txt"
                };
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));

                Assert.That(File.Exists("file.txt"));

                FileInfo fileInfo = new FileInfo("file.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task OutputToTextFileInUse()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                using (Stream file = new FileStream("file.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                    FilterConfig config = new FilterConfig(new[] { EmptyFile }) {
                        ShowPosition = true,
                        OutputFileName = "file.txt"
                    };
                    FilterApp app = new FilterApp(config);
                    ExitCode result = await app.Run();

                    Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                }

                Assert.That(File.Exists("file.txt"));

                FileInfo fileInfo = new FileInfo("file.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task OutputError()
        {
            var factoryMock = new Mock<IOutputStreamFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<OutputFormat>(), It.IsAny<string>()))
                .Returns((OutputFormat fmt, string name) => {
                    return null;
                });

            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = factoryMock.Object;

                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                ExitCode result = await app.Run();

                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.OutputError));
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void OutputUnhandledException()
        {
            var factoryMock = new Mock<IOutputStreamFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<OutputFormat>(), It.IsAny<string>()))
                .Returns((OutputFormat fmt, string name) => {
                    throw new NotSupportedException();
                });

            using (TestApplication global = new TestApplication()) {
                Global.Instance.OutputStreamFactory = factoryMock.Object;

                FilterConfig config = new FilterConfig(new[] { EmptyFile });
                FilterApp app = new FilterApp(config);
                Assert.That(async () => {
                    _ = await app.Run();
                }, Throws.TypeOf<NotSupportedException>());

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }
    }
}
