﻿namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Dlt;
    using Domain.InputStream;
    using Domain.OutputStream;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using TestResources;

    [TestFixture]
    public class FilterAppTest
    {
        private static readonly string InputDir = Path.Combine(Deploy.TestDirectory, "TestResources", "Input");
        private static readonly string EmptyFile = Path.Combine(InputDir, "EmptyFile.dlt");
        private static readonly string SimpleNvFile = Path.Combine(InputDir, "SimpleNonVerbose.dlt");
        private static readonly string FibexDir = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "valid");
        private static readonly string FibexFile = Path.Combine(FibexDir, "fibex-tcb.xml");

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
            FilterConfig config = new(Array.Empty<string>());
            FilterApp app = new(config);
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
                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
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
                TestDltFileStreamFactory fileFactory = new() {
                    OpenError = openError
                };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("file", fileFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
            }
        }

        [Test]
        public async Task OpenErrorUnhandledException()
        {
            // An exception occurring here won't be caught, and will bubble up to the top level,
            // so that a core dump can be captured. This exception is not documented by .NET.

            using (new TestApplication()) {
                TestDltFileStreamFactory fileFactory = new() {
                    OpenError = FileOpenError.InvalidOperationException
                };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("file", fileFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                await Assert.ThatAsync(async () => {
                    await app.Run();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public async Task OpenUnknownUri()
        {
            using (TestApplication global = new()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { "unknown://" });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenConnectError()
        {
            using (TestApplication global = new()) {
                TestNetworkStreamFactory testFactory = new();
                testFactory.ConnectEvent += (s, e) => { e.Succeed = false; };
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { "net://127.0.0.1" });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
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
            const int RetryOption = 7; // Option given to FilterApp
            const int OpenCount = 2;   // Our test case fails to connect after this count
            const int RetryCount = 4;  // Our test case connects after this many retries

            ExitCode expectedResult;
            int retries = RetryCount;
            int expectedConnects;
            int actualConnects = 0;

            using (TestApplication global = new()) {
                TestNetworkStreamFactory testFactory = new();
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
                    int opens = OpenCount;
                    // For loop 1, it fails the first four times and then connects.
                    // For loop 2, it fails the first four times and then connects.
                    // For loop 3, the connection has an error.
                    testFactory.OpenEvent += (s, e) => {
                        --opens;
                        retries = RetryCount;
                        e.Succeed = opens >= 0;
                    };
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0;
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = OpenCount * (RetryCount + 1);
                    break;
                case ConnectTestMode.RetryThenFail:
                    int loop = OpenCount;
                    // For loop 1, it fails the first four times and then connects.
                    // For loop 2, it fails the first four times and then connects.
                    // For loop 3, it will never connect, thus ending the connect infinite loop
                    testFactory.ConnectEvent += (s, e) => {
                        actualConnects++;
                        --retries;
                        e.Succeed = retries < 0 && loop > 0;
                        if (e.Succeed) {
                            retries = RetryCount;
                            --loop;
                        }
                    };
                    expectedResult = ExitCode.Success;
                    expectedConnects = OpenCount * (RetryCount + 1) + RetryOption + 1;
                    break;
                default:
                    Assert.Fail("Unknown test case");
                    return;
                }

                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { "net://127.0.0.1" }) {
                    ConnectRetries = RetryOption
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(expectedResult));

                // 3 lines to indicate connecting, the 4th line that it failed.
                Assert.That(actualConnects, Is.EqualTo(expectedConnects));
                Assert.That(global.Terminal.StdOutLines, Is.Not.Empty);
            }
        }

        [Test]
        public async Task OpenRetriesFile()
        {
            using (TestApplication global = new()) {
                // The number of retries should be ignored for streams that aren't live.
                FilterConfig config = new(new[] { EmptyFile }) {
                    ConnectRetries = -1
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.Terminal.StdOutLines, Is.Empty);
            }
        }

        [Test]
        public async Task OpenMultipleMix1()
        {
            using (TestApplication global = new()) {
                TestNetworkStreamFactory testFactory = new();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile, "net://127.0.0.1" });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenMultipleMix2()
        {
            using (TestApplication global = new()) {
                TestNetworkStreamFactory testFactory = new();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { "net://127.0.0.1", EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.InputError));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task OpenMultipleFile()
        {
            using (TestApplication global = new()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile, EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                // The user will be told also on the command line.
                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.Terminal.StdOutLines, Is.Empty);
            }
        }

        [Test]
        public async Task ShowSingleLine()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime()
                        .ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task ShowSingleLinePosition()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));

                string expectedLine = string.Format(
                    "00000003: {0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime()
                        .ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(expectedLine));
            }
        }

        [TestCase(InputFormat.Automatic, true)]
        [TestCase(InputFormat.Network, true)]
        [TestCase(InputFormat.Serial, true)]
        [TestCase(InputFormat.File, false)]
        public async Task OnlineModeForNetwork(InputFormat inputFormat, bool onlineMode)
        {
            using (TestApplication global = new()) {
                TestNetworkStreamFactory testFactory = new();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { "net://127.0.0.1" }) {
                    InputFormat = inputFormat
                };
                FilterApp app = new(config);
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
            using (TestApplication global = new()) {
                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile }) {
                    InputFormat = inputFormat
                };
                FilterApp app = new(config);
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
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    BeforeContext = 1,
                };
                config.AddAppId("APP2");

                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(2));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task BeforeContextNone()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    BeforeContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task AfterContext()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    AfterContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(3));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task AfterContextNone()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                // The file won't be accessed, as the InputStreamFactory will handle this and is mocked.
                FilterConfig config = new(new[] { EmptyFile }) {
                    InputFormat = InputFormat.Automatic,
                    AfterContext = 2,
                };
                config.AddAppId("APP2");

                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();

                // We're in offline mode, because the input is a file. This is regardless of the InputFormat.
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public async Task OutputToConsole()
        {
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "/dev/stdout"
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));

                string expectedLine = string.Format(
                    "00000003: {0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime()
                        .ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task OutputToTextFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file.txt"
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));

                Assert.That(File.Exists("file.txt"), Is.True);

                FileInfo fileInfo = new("file.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(10 + TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task OutputToTextFileExists(bool force)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                pad.DeployEmptyFile("file.txt");
                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    Force = force,
                    OutputFileName = "file.txt"
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                FileInfo fileInfo = new("file.txt");
                if (!force) {
                    Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                    Assert.That(fileInfo.Length, Is.EqualTo(0));
                } else {
                    Assert.That(result, Is.EqualTo(ExitCode.Success));
                    Assert.That(fileInfo.Length,
                        Is.EqualTo(10 + TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
                }

                Assert.That(File.Exists("file.txt"), Is.True);
            }
        }

        [Test]
        public async Task OutputToTextFileInUse()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                using (Stream file =
                       new FileStream("file.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                    FilterConfig config = new(new[] { EmptyFile }) {
                        ShowPosition = true,
                        OutputFileName = "file.txt"
                    };
                    FilterApp app = new(config);
                    ExitCode result = await app.Run();

                    Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));
                }

                Assert.That(File.Exists("file.txt"), Is.True);

                FileInfo fileInfo = new("file.txt");
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

            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = factoryMock.Object;

                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.OutputError));
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task OutputUnhandledException()
        {
            var factoryMock = new Mock<IOutputStreamFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<OutputFormat>(), It.IsAny<string>()))
                .Returns((OutputFormat fmt, string name) => {
                    throw new NotSupportedException();
                });

            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = factoryMock.Object;

                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                await Assert.ThatAsync(async () => {
                    _ = await app.Run();
                }, Throws.TypeOf<NotSupportedException>());

                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task OutputSplitNone()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file.txt",
                    Split = 1
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(File.Exists("file.txt"), Is.True);
            }
        }

        [Test]
        public async Task OutputSplitCtr()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file_%CTR%.txt",
                    Split = 1
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(File.Exists("file_001.txt"), Is.True);
                Assert.That(File.Exists("file_002.txt"), Is.True);
            }
        }

        [Test]
        public async Task OutputSplitDateTime()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file_%CDATETIME%.txt",
                    Split = 1
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                string line1 = TestLines.Verbose.TimeStamp.ToLocalTime().ToString(@"yyyyMMdd\THHmmss");
                string line2 = TestLines.Verbose2.TimeStamp.ToLocalTime().ToString(@"yyyyMMdd\THHmmss");
                Assert.That(result, Is.EqualTo(ExitCode.Success));
                Assert.That(File.Exists($"file_{line1}.txt"), Is.True);
                Assert.That(File.Exists($"file_{line2}.txt"), Is.True);
            }
        }

        [Test]
        public async Task OutputProtectedFiles()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                pad.DeployEmptyFile("input.txt");
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                FilterConfig config = new(new[] { "input.txt" }) {
                    OutputFileName = "%FILE%.txt",
                    Force = true
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));

                FileInfo info = new("input.txt");
                Assert.That(info.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task OutputProtectedFilesPartial()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                pad.DeployEmptyFile("input_002.txt");
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose2);

                // Try to overwrite `input_002.txt`
                FilterConfig config = new(new[] { "input_002.txt" }) {
                    OutputFileName = "input_%CTR%.txt",
                    Force = true,
                    Split = 1
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                global.WriteStd();
                Assert.That(result, Is.EqualTo(ExitCode.NoFilesProcessed));

                FileInfo info = new("input_002.txt");
                Assert.That(info.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task OutputToDltFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();

                FilterConfig config = new(new[] { EmptyFile }) {
                    ShowPosition = true,
                    OutputFileName = "file.dlt"
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));

                // The FilterApp sets this output. so that the factory knows to create the correct TraceReader
                // that writes DLT.
                Assert.That(Global.Instance.DltReaderFactory.OutputStream, Is.TypeOf<DltOutput>());

                // The TraceReader would write this file. As the TestDltTraceReaderFactory instantiates a
                // BinaryTraceReader and never generates output, this file is never created
                Assert.That(File.Exists("file.dlt"), Is.False);
            }
        }

        [Test]
        public async Task OutputToDltFileConvertNonVerbose()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                // This is a full integrated test.

                FibexFile map = new(FibexOptions.None);
                map.LoadFile(FibexFile);

                FilterConfig config = new(new[] { SimpleNvFile }) {
                    OutputFileName = "file.dlt",
                    ConvertNonVerbose = true,
                    FrameMap = map
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));

                // The FilterApp sets this output. so that the factory knows to create the correct TraceReader
                // that writes DLT.
                Assert.That(Global.Instance.DltReaderFactory.OutputStream, Is.TypeOf<DltOutput>());

                // The TraceReader would write this file.
                Assert.That(File.Exists("file.dlt"), Is.True);

                byte[] encoded;
                using (MemoryStream mem = new())
                using (FileStream file = new("file.dlt", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    await file.CopyToAsync(mem);
                    encoded = mem.ToArray();
                }

                byte[] expected = new byte[] {
                    0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x43, 0x42, 0x00,  // Storage Header. ECU1 => TCB.
                    0x25, 0x00, 0x00, 0x36, 0x54, 0x43, 0x42, 0x00,                                                  // StdHeader
                    0x41, 0x01, 0x54, 0x45, 0x53, 0x54, 0x43, 0x4F, 0x4E, 0x31,                                      // ExtHeader, 1 arg
                    0x00, 0x82, 0x00, 0x00, 0x1E, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x6E, 0x6F, 0x6E, 0x20, 0x76, 0x65,  // Arg 1 (Verbose)
                    0x72, 0x62, 0x6F, 0x73, 0x65, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x6D, 0x65, 0x73, 0x73, 0x61,
                    0x67, 0x65, 0x2E, 0x00,
                    0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x43, 0x42, 0x00,  // Storage Header
                    0x25, 0x01, 0x00, 0x3C, 0x54, 0x43, 0x42, 0x00,                                                  // StdHeader
                    0x31, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x4F, 0x4E, 0x31,                                      // ExtHeader, 2 args
                    0x00, 0x82, 0x00, 0x00, 0x1E, 0x00, 0x42, 0x75, 0x66, 0x66, 0x65, 0x72, 0x20, 0x6E, 0x65, 0x61,  // Arg 1 (Verbose)
                    0x72, 0x20, 0x6C, 0x69, 0x6D, 0x69, 0x74, 0x2E, 0x20, 0x46, 0x72, 0x65, 0x65, 0x20, 0x73, 0x69,
                    0x7A, 0x65, 0x3A, 0x00,
                    0x42, 0x00, 0x00, 0x00, 0xFF, 0x7F                                                               // Arg 2 (Verbose)
                };
                Assert.That(encoded, Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task OutputToDltFileConvertNonVerboseFiltered()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                // This is a full integrated test.

                FibexFile map = new(FibexOptions.None);
                map.LoadFile(FibexFile);

                FilterConfig config = new(new[] { SimpleNvFile }) {
                    OutputFileName = "file.dlt",
                    ConvertNonVerbose = true,
                    FrameMap = map
                };
                config.AddAppId("APP1");
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.Success));

                // The FilterApp sets this output. so that the factory knows to create the correct TraceReader
                // that writes DLT.
                Assert.That(Global.Instance.DltReaderFactory.OutputStream, Is.TypeOf<FilterOutput>());

                // The TraceReader would write this file.
                Assert.That(File.Exists("file.dlt"), Is.True);

                byte[] encoded;
                using (MemoryStream mem = new())
                using (FileStream file = new("file.dlt", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    await file.CopyToAsync(mem);
                    encoded = mem.ToArray();
                }

                byte[] expected = new byte[] {
                    0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x43, 0x42, 0x00,  // Storage Header. ECU1 => TCB.
                    0x25, 0x01, 0x00, 0x3C, 0x54, 0x43, 0x42, 0x00,                                                  // StdHeader
                    0x31, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x4F, 0x4E, 0x31,                                      // ExtHeader, 2 args
                    0x00, 0x82, 0x00, 0x00, 0x1E, 0x00, 0x42, 0x75, 0x66, 0x66, 0x65, 0x72, 0x20, 0x6E, 0x65, 0x61,  // Arg 1 (Verbose)
                    0x72, 0x20, 0x6C, 0x69, 0x6D, 0x69, 0x74, 0x2E, 0x20, 0x46, 0x72, 0x65, 0x65, 0x20, 0x73, 0x69,
                    0x7A, 0x65, 0x3A, 0x00,
                    0x42, 0x00, 0x00, 0x00, 0xFF, 0x7F                                                               // Arg 2 (Verbose)
                };
                Assert.That(encoded, Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task ExceptionWhileDecoding()
        {
            using (TestApplication global = new()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).TriggerExceptionOnEof = true;
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);

                FilterConfig config = new(new[] { EmptyFile });
                FilterApp app = new(config);
                ExitCode result = await app.Run();

                Assert.That(result, Is.EqualTo(ExitCode.PartialFilesProcessed));
                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(2));

                string expectedLine = string.Format("{0} 80.5440 127 ECU1 APP1 CTX1 127 log info verbose 1 Message 1",
                    TestLines.Verbose.TimeStamp.ToLocalTime()
                        .ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(expectedLine));
            }
        }

        [Test]
        public async Task OpenLiveStreamWithAutoFlush()
        {
            // This is difficult to test, as we don't mock the stream that is being written to. This would require
            // changing OutputBase that we write to our own stream, which is not how OutputBase works, and the test
            // requires the type OutputBase else the AutoFlushPeriod won't be set. Thus, the test should pass, but only
            // through the code coverage can we see that it is being tested.

            // Instead, we rely on it being called, and test cases in OutputWriterTest check the functionality that
            // flushing is periodically called.

            using (Deploy.ScratchPad())
            using (TestApplication global = new()) {
                Global.Instance.OutputStreamFactory = new OutputStreamFactory();   // TextOutput allows flushing

                FilterConfig config = new(new[] { "net://127.0.0.1" }) {
                    OutputFileName = "file.txt"
                };
                FilterApp app = new(config);
                ExitCode result = await app.Run();
                Assert.That(result, Is.EqualTo(ExitCode.Success));
            }
        }
    }
}
