namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Domain;
    using Domain.InputStream;
    using Infrastructure.Dlt;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using TestResources;
    using static Infrastructure.OptionsGen;

    [TestFixture]
    public class FilterCommandTest
    {
        // The FilterCommand class needs a CmdOptions object as a constructor. We can't create this in a test case, so
        // must use the CommandLine functionality to do this for us, via CommandLine.Run(). Therefore, this test harness
        // tests both the command line conversion and the functionality of the FilterCommand.

        // Even though FilterApp.Run() is called, it won't open or read the files as the dependencies are mocked via
        // TestApplication.

        private static void CommandFactorySetup(Action<CmdOptions> action)
        {
            var factoryMock = new Mock<ICommandFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<CmdOptions>()))
                .Returns((CmdOptions opt) => {
                    var f = new CommandFactory();
                    ICommand command = f.Create(opt);

                    Assert.That(command, Is.TypeOf<FilterCommand>());
                    action(opt);
                    return command;
                });

            // The CommandLine.Run() will use this factory.
            Global.Instance.CommandFactory = factoryMock.Object;
        }

        [Test]
        public void NoOptions()
        {
            Assert.That(() => {
                _ = new FilterCommand(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");
        private readonly string EmptyFile2 = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile2.dlt");

        #region Input File
        [Test]
        public void InputFileMissing()
        {
            string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "NonExistent.dlt");
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    file
                }), Is.EqualTo(ExitCode.NoFilesProcessed));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(file));
            }
        }

        [Test]
        public void InputFileRelative()
        {
            using (ScratchPad scratch = Deploy.ScratchPad())
            using (new TestApplication()) {
                // FilterApp checks that the file actually exists
                scratch.DeployItem(Path.Combine("TestResources", "Input", "EmptyFile.dlt"));

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    "EmptyFile.dlt"
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo("EmptyFile.dlt"));
            }
        }

        [Test]
        public void InputFileAbsolute()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
            }
        }

        [Test]
        public void InputFilesRelative()
        {
            using (ScratchPad scratch = Deploy.ScratchPad())
            using (new TestApplication()) {
                // FilterApp checks that the file actually exists
                scratch.DeployItem(Path.Combine("TestResources", "Input", "EmptyFile.dlt"));
                scratch.DeployItem(Path.Combine("TestResources", "Input", "EmptyFile2.dlt"));

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    "EmptyFile.dlt", "EmptyFile2.dlt"
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo("EmptyFile.dlt"));
                Assert.That(cmdOptions.Arguments[1], Is.EqualTo("EmptyFile2.dlt"));
            }
        }

        [Test]
        public void InputFilesAbsolute()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    EmptyFile, EmptyFile2
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.Arguments[1], Is.EqualTo(EmptyFile2));
            }
        }

        [Test]
        public void InputFileUri()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    new Uri(EmptyFile).AbsoluteUri
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(new Uri(EmptyFile).AbsoluteUri));
            }
        }
        #endregion

        #region Position option
        [Test]
        public void ShowPosition()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("position"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.Position, Is.True);
            }
        }
        #endregion

        #region Input Format
        [Test]
        public void DefaultSetInputFormatFile()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                Assert.That(cmdOptions.InputFormat, Is.EqualTo(InputFormat.Automatic));

                // When automatic, the FilterApp uses the InputStream.SuggestedFormat which is created
                // by the InputStreamFactory.
                Assert.That(Global.Instance.DltReaderFactory.InputFormat, Is.EqualTo(InputFormat.File));
            }
        }

        [TestCase("file", InputFormat.File, InputFormat.File)]
        [TestCase("File", InputFormat.File, InputFormat.File)]
        [TestCase("FILE", InputFormat.File, InputFormat.File)]
        [TestCase("serial", InputFormat.Serial, InputFormat.Serial)]
        [TestCase("ser", InputFormat.Serial, InputFormat.Serial)]
        [TestCase("network", InputFormat.Network, InputFormat.Network)]
        [TestCase("net", InputFormat.Network, InputFormat.Network)]
        [TestCase("automatic", InputFormat.Automatic, InputFormat.File)]
        [TestCase("auto", InputFormat.Automatic, InputFormat.File)]
        public void SetInputFormat(string option, InputFormat result, InputFormat decodeFormat)
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("format", option), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                Assert.That(cmdOptions.InputFormat, Is.EqualTo(result));

                // This shows that the DltReaderFactory got the command line option when parsing (it only shows the last
                // instance that was instantiated).
                Assert.That(Global.Instance.DltReaderFactory.InputFormat, Is.EqualTo(decodeFormat));
            }
        }

        [Test]
        public void InvalidInputFormat()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("format", "foo"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }
        #endregion

        #region Retry option
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(10)]
        public void RetryCount(int count)
        {
            using (new TestApplication()) {
                int actualConnects = 0;

                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                testFactory.ConnectEvent += (s, e) => {
                    actualConnects++;
                    e.Succeed = false;
                };

                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("retries", count.ToString()), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.NoFilesProcessed));

                // 5 retries produces 6 connect attempts before ending.
                Assert.That(actualConnects, Is.EqualTo(count + 1));
            }
        }

        [Test]
        public void RetryCountInfinite()
        {
            using (new TestApplication()) {
                int actualCreate = 0;

                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                testFactory.CreateEvent += (s, e) => {
                    e.Succeed = actualCreate < 20;
                    actualCreate++;
                };
                testFactory.ConnectEvent += (s, e) => {
                    e.Succeed = true;
                };

                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                // Tests that a retries of -1 results in infinite retries, even after successful connects.
                Assert.That(CommandLine.Run(new[] {
                    LongOpt("retries", "-1"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                // The reconnects is infinite, until we get the a CreateEvent failing.
                Assert.That(actualCreate, Is.EqualTo(21));
            }
        }
        #endregion

        #region Filter Options
        [TestCase("Message", 1)]
        [TestCase("xxx", 0)]
        public void SearchString(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s'), search, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchString.Count, Is.EqualTo(1));
            }
        }

        [TestCase("Message", 1)]
        [TestCase("xxx", 0)]
        public void SearchMultiString(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s'), search, LongOpt("string"), "foo", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchString.Count, Is.EqualTo(2));
            }
        }

        [TestCase(@"s+age\s+\d+", 1)]
        [TestCase(@"\S+\s+$", 0)]
        public void SearchRegEx(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('r'), search, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex.Count, Is.EqualTo(1));
            }
        }

        [TestCase(@"s+age\s+\d+", 1)]
        [TestCase(@"\S+\s+$", 0)]
        public void SearchMultiRegEx(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('r'), search, LongOpt("regex"), "^foo$", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex.Count, Is.EqualTo(2));
            }
        }

        [TestCase("xxx", @"s+age\s+\d+", 1)]
        [TestCase("xxx", @"\S+\s+$", 0)]
        [TestCase("Message", @"s+age\s+\d+", 1)]
        [TestCase("Message", @"\S+\s+$", 1)]
        public void SearchRegExString(string search, string regex, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s'), search, LongOpt("regex"), regex, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchString.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.SearchRegex.Count, Is.EqualTo(1));
            }
        }

        [TestCase("message", 1)]
        [TestCase("xxx", 0)]
        public void SearchStringIgnoreCase(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s'), search, ShortOpt('i'), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchString.Count, Is.EqualTo(1));
            }
        }

        [TestCase(@"s+age\s+\d+", 1)]
        [TestCase(@"\S+\s+$", 0)]
        public void SearchRegExIgnoreCase(string search, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('r'), search, LongOpt("ignorecase"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex.Count, Is.EqualTo(1));
            }
        }

        [TestCase("ECU1", 1)]
        [TestCase("ecu1", 0)]
        [TestCase("xxx", 0)]
        public void SearchEcuId(string ecuId, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ecuid"), ecuId, LongOpt("ignorecase"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.EcuId.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void SearchEcuIdMultiNoMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ecuid"), "AAAA,BBBB", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.EcuId.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void SearchEcuIdMultiMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ecuid"), "AAAA,ECU1", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.EcuId.Count, Is.EqualTo(2));
            }
        }

        [TestCase("APP1", 1)]
        [TestCase("app1", 0)]
        [TestCase("xxx", 0)]
        public void SearchAppId(string appId, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("appid"), appId, LongOpt("ignorecase"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.AppId.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void SearchAppIdMultiNoMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("appid"), "AAAA,BBBB", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.AppId.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void SearchAppIdMultiMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("appid"), "AAAA,APP1", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.AppId.Count, Is.EqualTo(2));
            }
        }

        [TestCase("CTX1", 1)]
        [TestCase("ctx1", 0)]
        [TestCase("xxx", 0)]
        public void SearchCtxId(string ctxId, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ctxid"), ctxId, LongOpt("ignorecase"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.CtxId.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void SearchCtxIdMultiNoMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ctxid"), "AAAA,BBBB", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.CtxId.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void SearchCtxIdMultiMatch()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ctxid"), "AAAA,CTX1", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.CtxId.Count, Is.EqualTo(2));
            }
        }

        [TestCase("127", 1)]
        [TestCase("128", 0)]
        public void SearchSessionId(string session, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("sessionid"), session, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(count));
                Assert.That(cmdOptions.SessionId.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void SearchNegativeSessionId()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("sessionid", "-1"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.SessionId.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void SearchVerbose()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("verbose"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.VerboseMessage, Is.True);
            }
        }

        [Test]
        public void SearchVerboseNotPresent()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NoExtHdr);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("verbose"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.VerboseMessage, Is.True);
            }
        }

        [Test]
        public void SearchNoneVerbose()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NoExtHdr);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("nonverbose"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.NonVerboseMessage, Is.True);
            }
        }

        [Test]
        public void SearchNonVerboseNotPresent()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("nonverbose"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.NonVerboseMessage, Is.True);
            }
        }

        [Test]
        public void SearchControl()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Control);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("control"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.ControlMessage, Is.True);
            }
        }

        [Test]
        public void SearchControlNotPresent()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NoExtHdr);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("control"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.ControlMessage, Is.True);
            }
        }

        [Test]
        public void InvalidSessionIdString()
        {
            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("sessionid"), "abc", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void InvalidSessionIdLarge()
        {
            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("sessionid"), "6000000000", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void FilterNone()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("none"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.None, Is.True);
            }
        }
        #endregion
    }
}
