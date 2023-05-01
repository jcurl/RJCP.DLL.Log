namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using static Infrastructure.OptionsGen;

    [TestFixture]
    public class NonVerboseCommandTest
    {
        // The NonVerboseCommand class needs a CmdOptions object as a constructor. We can't create this in a test case,
        // so must use the CommandLine functionality to do this for us, via CommandLine.Run(). Therefore, this test
        // harness tests both the command line conversion and the functionality of the NonVerboseCommand.

        // Even though NonVerboseCommand.Run() is called, it won't open or read the files as the dependencies are mocked
        // via TestApplication.

        private static void CommandFactorySetup(Action<CmdOptions> action)
        {
            var factoryMock = new Mock<ICommandFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<CmdOptions>()))
                .Returns((CmdOptions opt) => {
                    var f = new CommandFactory();
                    ICommand command = f.Create(opt);

                    Assert.That(command, Is.TypeOf<NonVerboseCommand>());
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
                _ = new NonVerboseCommand(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private static readonly string FibexDir = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "valid");
        private static readonly string FibexFile = Path.Combine(FibexDir, "fibex-tcb.xml");
        private static readonly string FibexFile2 = Path.Combine(FibexDir, "fibex-tcb2.xml");
        private static readonly string FibexFileInv = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "invalid", "fibex-tcb-error-pduid.xml");

        [Test]
        public void FibexFileSingle()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", FibexFile)
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
            }
        }

        [Test]
        public void FibexFileMulti1()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", FibexFile), LongOpt("fibex", FibexFile2), LongOpt("nv-multiecu")
                }), Is.EqualTo(ExitCode.Success)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
                Assert.That(cmdOptions.Fibex[1], Is.EqualTo(FibexFile2));
            }
        }

        [Test]
        public void FibexFileMulti2()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", $"{FibexFile},{FibexFile2}"), LongOpt("nv-multiecu")
                }), Is.EqualTo(ExitCode.Success)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
                Assert.That(cmdOptions.Fibex[1], Is.EqualTo(FibexFile2));
            }
        }

        [Test]
        public void FibexFileMultiNoExtHdr()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", $"{FibexFile},{FibexFile2}"), LongOpt("nv-multiecu"), LongOpt("nv-noexthdr")
                }), Is.EqualTo(ExitCode.Success)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
                Assert.That(cmdOptions.Fibex[1], Is.EqualTo(FibexFile2));
            }
        }

        [Test]
        public void FibexFileMultiWithError()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", $"{FibexFile},{FibexFile2}"), LongOpt("nv-noexthdr")
                }), Is.EqualTo(ExitCode.FibexError)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFile));
                Assert.That(cmdOptions.Fibex[1], Is.EqualTo(FibexFile2));
            }
        }

        [Test]
        public void FibexFileWithError()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", FibexFileInv)
                }), Is.EqualTo(ExitCode.FibexError)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexFileInv));
            }
        }

        [Test]
        public void FibexFileNotFound()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", "foobar.xml")
                }), Is.EqualTo(ExitCode.FibexError)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo("foobar.xml"));
            }
        }

        [Test]
        public void FibexDirMulti()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("fibex", FibexDir), LongOpt("nv-multiecu")
                }), Is.EqualTo(ExitCode.Success)); ;
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(0));
                Assert.That(cmdOptions.Fibex.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Fibex[0], Is.EqualTo(FibexDir));
            }
        }
    }
}
