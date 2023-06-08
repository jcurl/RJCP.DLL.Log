namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Diagnostics.Log.Dlt;
    using Domain;
    using Domain.Dlt;
    using Domain.InputStream;
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

        private static readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");
        private static readonly string EmptyFile2 = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile2.dlt");
        private static readonly string EmptyPcap = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.pcap");
        private static readonly string FibexDir = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "valid");
        private static readonly string FibexFile = Path.Combine(FibexDir, "fibex-tcb.xml");

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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(2));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(2));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(new Uri(EmptyFile).AbsoluteUri));
            }
        }
        #endregion

        #region Input Packet Service
        [Test]
        public void PacketInput()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestPacketReaderFactory testFactory = new TestPacketReaderFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("pkt", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    "pkt://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
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
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
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
        [TestCase("pcap", InputFormat.Pcap, InputFormat.Pcap)]
        [TestCase("pcapng", InputFormat.Pcap, InputFormat.Pcap)]
        public void SetInputFormatDlt(string option, InputFormat result, InputFormat decodeFormat)
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

        [TestCase("file", InputFormat.File, InputFormat.File)]
        [TestCase("File", InputFormat.File, InputFormat.File)]
        [TestCase("FILE", InputFormat.File, InputFormat.File)]
        [TestCase("serial", InputFormat.Serial, InputFormat.Serial)]
        [TestCase("ser", InputFormat.Serial, InputFormat.Serial)]
        [TestCase("network", InputFormat.Network, InputFormat.Network)]
        [TestCase("net", InputFormat.Network, InputFormat.Network)]
        [TestCase("automatic", InputFormat.Automatic, InputFormat.Pcap)]
        [TestCase("auto", InputFormat.Automatic, InputFormat.Pcap)]
        [TestCase("pcap", InputFormat.Pcap, InputFormat.Pcap)]
        [TestCase("pcapng", InputFormat.Pcap, InputFormat.Pcap)]
        public void SetInputFormatPcap(string option, InputFormat result, InputFormat decodeFormat)
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("format", option), EmptyPcap
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
                testFactory.OpenEvent += (s, e) => {
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchString, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchString, Has.Count.EqualTo(2));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex, Has.Count.EqualTo(2));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchString, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.SearchRegex, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchString, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SearchRegex, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.EcuId, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.EcuId, Has.Count.EqualTo(2));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.EcuId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchEcuIdDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ecuid"), "AAAA,ECU1,AAAA", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.EcuId, Has.Count.EqualTo(2).Or.Count.EqualTo(3));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.AppId, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.AppId, Has.Count.EqualTo(2));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.AppId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchAppIdDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("appid"), "AAAA,APP1,AAAA", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.AppId, Has.Count.EqualTo(2).Or.Count.EqualTo(3));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.CtxId, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.CtxId, Has.Count.EqualTo(2));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.CtxId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchCtsIdDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("ctxid"), "AAAA,CTX1,AAAA", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.CtxId, Has.Count.EqualTo(2).Or.Count.EqualTo(3));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.SessionId, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.SessionId, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void SearchSessionIdDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("sessionid", "127,127,128"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.SessionId, Has.Count.EqualTo(2).Or.Count.EqualTo(3));
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
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
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
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
                Assert.That(global.StdOut.Lines, Is.Empty);
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
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.None, Is.True);
            }
        }

        [TestCase("fatal", DltType.LOG_FATAL)]
        [TestCase("error", DltType.LOG_ERROR)]
        [TestCase("warn", DltType.LOG_WARN)]
        [TestCase("info", DltType.LOG_INFO)]
        [TestCase("debug", DltType.LOG_DEBUG)]
        [TestCase("verbose", DltType.LOG_VERBOSE)]
        [TestCase("ipc", DltType.NW_TRACE_IPC)]
        [TestCase("can", DltType.NW_TRACE_CAN)]
        [TestCase("flexray", DltType.NW_TRACE_FLEXRAY)]
        [TestCase("most", DltType.NW_TRACE_MOST)]
        [TestCase("ethernet", DltType.NW_TRACE_ETHERNET)]
        [TestCase("someip", DltType.NW_TRACE_SOMEIP)]
        [TestCase("user1", DltType.NW_TRACE_USER_DEFINED_0)]
        [TestCase("user2", DltType.NW_TRACE_USER_DEFINED_1)]
        [TestCase("user3", DltType.NW_TRACE_USER_DEFINED_2)]
        [TestCase("user4", DltType.NW_TRACE_USER_DEFINED_3)]
        [TestCase("user5", DltType.NW_TRACE_USER_DEFINED_4)]
        [TestCase("user6", DltType.NW_TRACE_USER_DEFINED_5)]
        [TestCase("user7", DltType.NW_TRACE_USER_DEFINED_6)]
        [TestCase("user8", DltType.NW_TRACE_USER_DEFINED_7)]
        [TestCase("user9", DltType.NW_TRACE_USER_DEFINED_8)]
        [TestCase("request", DltType.CONTROL_REQUEST)]
        [TestCase("response", DltType.CONTROL_RESPONSE)]
        [TestCase("time", DltType.CONTROL_TIME)]
        [TestCase("variable", DltType.APP_TRACE_VARIABLE)]
        [TestCase("functionin", DltType.APP_TRACE_FUNCTION_IN)]
        [TestCase("functionout", DltType.APP_TRACE_FUNCTION_OUT)]
        [TestCase("state", DltType.APP_TRACE_STATE)]
        [TestCase("vfb", DltType.APP_TRACE_VFB)]
        [TestCase("48", DltType.LOG_WARN)]
        public void SearchDltType(string dltFilterType, DltType result)
        {
            using (TestApplication global = new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("type", dltFilterType), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                Assert.That(cmdOptions.DltTypeFilters, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.DltTypeFilters[0], Is.EqualTo(result));
            }
        }

        [Test]
        public void SearchDltTypeInfo()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("type", "info"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void SearchDltTypeWarn()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("type", "warn"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Is.Empty);
            }
        }

        [Test]
        public void DltTypeDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("type", "info,warn,warn,error"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.DltTypeFilters, Has.Count.EqualTo(3).Or.Count.EqualTo(4));
            }
        }

        [TestCase("foobar")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("-")]
        public void DltTypeInvalid(string dltType)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("type", dltType), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
                global.WriteStd();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SearchNotBeforeUtc(bool utc)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notBefore = TestLines.Verbose.TimeStamp - TimeSpan.FromSeconds(2);
                string notBeforeOpt;
                if (utc) {
                    notBeforeOpt = notBefore.ToString(@"yyyy-MM-dd\ZHH:mm:ss");   // Input is UTC, string is UTC
                } else {
                    notBeforeOpt = notBefore.ToLocalTime().ToString(@"yyyy-MM-dd\THH:mm:ss");
                }

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-before"), notBeforeOpt, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SearchNotBeforeNoMatchUtc(bool utc)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notBefore = TestLines.Verbose.TimeStamp + TimeSpan.FromSeconds(2);
                string notBeforeOpt;
                if (utc) {
                    notBeforeOpt = notBefore.ToString(@"yyyy-MM-dd\ZHH:mm:ss");   // Input is UTC, string is UTC
                } else {
                    notBeforeOpt = notBefore.ToLocalTime().ToString(@"yyyy-MM-dd\THH:mm:ss");
                }

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-before"), notBeforeOpt, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                global.WriteStd();
                Assert.That(global.StdOut.Lines, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SearchNotAfterUtc(bool utc)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notAfter = TestLines.Verbose.TimeStamp + TimeSpan.FromSeconds(2);
                string notAfterOpt;
                if (utc) {
                    notAfterOpt = notAfter.ToString(@"yyyy-MM-dd\ZHH:mm:ss");   // Input is UTC, string is UTC
                } else {
                    notAfterOpt = notAfter.ToLocalTime().ToString(@"yyyy-MM-dd\THH:mm:ss");
                }

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-after"), notAfterOpt, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SearchNotAfterNoMatchUtc(bool utc)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notAfter = TestLines.Verbose.TimeStamp - TimeSpan.FromSeconds(2);
                string notAfterOpt;
                if (utc) {
                    notAfterOpt = notAfter.ToString(@"yyyy-MM-dd\ZHH:mm:ss");   // Input is UTC, string is UTC
                } else {
                    notAfterOpt = notAfter.ToLocalTime().ToString(@"yyyy-MM-dd\THH:mm:ss");
                }

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-after"), notAfterOpt, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                global.WriteStd();
                Assert.That(global.StdOut.Lines, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SearchDateTimeRange(bool utc)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notBefore = TestLines.Verbose.TimeStamp - TimeSpan.FromSeconds(2);
                DateTime notAfter = TestLines.Verbose.TimeStamp + TimeSpan.FromSeconds(2);
                string notBeforeOpt;
                string notAfterOpt;
                if (utc) {
                    notBeforeOpt = notBefore.ToString(@"yyyy-MM-dd\ZHH:mm:ss");   // Input is UTC, string is UTC
                    notAfterOpt = notAfter.ToString(@"yyyy-MM-dd\ZHH:mm:ss");
                } else {
                    notBeforeOpt = notBefore.ToLocalTime().ToString(@"yyyy-MM-dd HH:mm:ss"); // An alternative for local time
                    notAfterOpt = notAfter.ToLocalTime().ToString(@"yyyy-MM-dd HH:mm:ss");
                }

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-after", notAfterOpt), LongOpt("not-before", notBeforeOpt), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));

                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
            }
        }

        [TestCase("not-before")]
        [TestCase("not-after")]
        public void InvalidDate(string opt)
        {
            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt(opt, "xxxx"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void InvalidDateOrder()
        {
            using (new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                DateTime notBefore = TestLines.Verbose.TimeStamp + TimeSpan.FromSeconds(2);
                DateTime notAfter = TestLines.Verbose.TimeStamp - TimeSpan.FromSeconds(2);
                string notBeforeOpt = notBefore.ToString(@"yyyy-MM-dd\ZHH:mm:ss");
                string notAfterOpt = notAfter.ToString(@"yyyy-MM-dd\ZHH:mm:ss");

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("not-before", notBeforeOpt), LongOpt("not-after", notAfterOpt), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [TestCase("42", 1)]
        [TestCase("43", 0)]
        public void SearchMessageId(string messageId, int count)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NonVerbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid"), messageId, "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(count));
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void SearchMessageIds()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NonVerbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid"), "42", LongOpt("messageid"), "43", "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchMessageIdsList()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NonVerbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid", "42,43"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchMessageIdsListDuplicate()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.NonVerbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid", "42,42"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(1).Or.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchMessageIdVerboseLine()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Verbose);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid", "42,43"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void SearchMessageIdControlLine()
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Control);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid", "42,43"), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.Success));
                global.WriteStd();
                Assert.That(global.StdOut.Lines, Is.Empty);
                Assert.That(cmdOptions.MessageId, Has.Count.EqualTo(2));
            }
        }

        [TestCase("foobar")]
        [TestCase("-100")]
        [TestCase("0xFF")]
        [TestCase("  ")]
        [TestCase("")]
        [TestCase("<empty>")]
        public void MessageIdInvalid(string messageId)
        {
            using (TestApplication global = new TestApplication()) {
                ((TestDltTraceReaderFactory)Global.Instance.DltReaderFactory).Lines.Add(TestLines.Control);
                TestNetworkStreamFactory testFactory = new TestNetworkStreamFactory();
                ((TestInputStreamFactory)Global.Instance.InputStreamFactory).SetFactory("net", testFactory);

                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("messageid", messageId), "net://127.0.0.1"
                }), Is.EqualTo(ExitCode.OptionsError));
                global.WriteStd();
            }
        }
        #endregion

        #region Context
        [Test]
        public void BeforeContextLongOptNo()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s', "string"), LongOpt("before-context", "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.BeforeContext, Is.EqualTo(1));
                Assert.That(cmdOptions.AfterContext, Is.EqualTo(0));
            }
        }

        [Test]
        public void BeforeContextShortOpt()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s', "string"), ShortOpt('B', "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.BeforeContext, Is.EqualTo(1));
                Assert.That(cmdOptions.AfterContext, Is.EqualTo(0));
            }
        }

        [Test]
        public void BeforeContextNoFilter()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('B', "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public void BeforeContextNegative()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('B', "-1"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void BeforeContextInvalid()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('B', "xx"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void AfterContextLongOptNo()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s', "string"), LongOpt("after-context", "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.BeforeContext, Is.EqualTo(0));
                Assert.That(cmdOptions.AfterContext, Is.EqualTo(1));
            }
        }

        [Test]
        public void AfterContextShortOpt()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('s', "string"), ShortOpt('A', "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.BeforeContext, Is.EqualTo(0));
                Assert.That(cmdOptions.AfterContext, Is.EqualTo(1));
            }
        }

        [Test]
        public void AfterContextNoFilter()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('A', "1"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
            }
        }

        [Test]
        public void AfterContextNegative()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('A', "-1"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }

        [Test]
        public void AfterContextInvalid()
        {
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('A', "xx"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }
        #endregion

        #region Output Files
        [Test]
        public void OutputFileEmpty()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("output", ""), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.Empty);
            }
        }

        [Test]
        public void OutputFileConsole()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "CON:"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("CON:"));
            }
        }

        [Test]
        public void OutputFileConsoleStdOut()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "/dev/stdout"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("/dev/stdout"));
            }
        }

        [Test]
        public void OutputTextFileName()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("output.txt"));
            }
        }

        [Test]
        public void OutputDltFileName()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.dlt"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("output.dlt"));
            }
        }

        [Test]
        public void OutputTextFileNameForce()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("force"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("output.txt"));
                Assert.That(cmdOptions.Force, Is.True);
            }
        }
        #endregion

        #region Split
        [Test]
        public void SplitInteger()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", "102400"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("output.txt"));
                Assert.That(cmdOptions.Split, Is.EqualTo(102400));
            }
        }

        [TestCase("65536", 65536)]
        [TestCase("65536b", 65536)]
        [TestCase("65536B", 65536)]
        [TestCase("100k", 100 << 10)]
        [TestCase("100K", 100 << 10)]
        [TestCase("100kB", 100 << 10)]
        [TestCase("100kb", 100 << 10)]
        [TestCase("100KB", 100 << 10)]
        [TestCase("100Kb", 100 << 10)]
        [TestCase("10m", 10 << 20)]
        [TestCase("10M", 10 << 20)]
        [TestCase("10mB", 10 << 20)]
        [TestCase("10mb", 10 << 20)]
        [TestCase("10MB", 10 << 20)]
        [TestCase("10Mb", 10 << 20)]
        [TestCase("2g", (long)2 << 30)]
        [TestCase("2G", (long)2 << 30)]
        [TestCase("2gB", (long)2 << 30)]
        [TestCase("2gb", (long)2 << 30)]
        [TestCase("2GB", (long)2 << 30)]
        [TestCase("2Gb", (long)2 << 30)]
        public void SplitIntegerModifier(string split, long value)
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", split), EmptyFile
                }), Is.EqualTo(ExitCode.Success));

                // The command options don't really matter for FilterCommand, but useful to see that the options were
                // properly generated.
                Assert.That(cmdOptions.OutputFileName, Is.EqualTo("output.txt"));
                Assert.That(cmdOptions.Split, Is.EqualTo(value));

                // When the `FilterApp` runs, it gets the split, and gives it to the factory. Here we see that the value
                // was converted from the command line and given to the `FilterApp`.
                Assert.That(Global.Instance.OutputStreamFactory.Split, Is.EqualTo(value));
            }
        }

        [Test]
        public void SplitNegativeInteger()
        {
            using (TestApplication global = new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", "-102400"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));

                global.WriteStd();
            }
        }

        [Test]
        public void SplitUnknownModifier()
        {
            using (TestApplication global = new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", "10E"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));

                global.WriteStd();
            }
        }

        [TestCase("0")]
        [TestCase("10")]
        [TestCase("65535")]
        public void SplitTooSmall(string value)
        {
            using (TestApplication global = new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", value), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));

                global.WriteStd();
            }
        }

        [Test]
        public void SplitOverflow()
        {
            using (TestApplication global = new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('o', "output.txt"), LongOpt("split", "99999999999999999999999999999999999999999"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));

                global.WriteStd();
            }
        }
        #endregion

        #region Input File with NonVerbose
        [Test]
        public void ConvertNonVerbose()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('F', FibexFile), LongOpt("nv-verbose"), ShortOpt('o', "out.dlt"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments, Is.EqualTo(new[] { EmptyFile }));
                Assert.That(cmdOptions.Fibex, Has.Count.EqualTo(1));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
                Assert.That(cmdOptions.NonVerboseWriteVerbose, Is.True);
            }
        }
        #endregion
    }
}
