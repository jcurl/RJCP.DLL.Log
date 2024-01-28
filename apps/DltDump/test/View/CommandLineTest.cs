namespace RJCP.App.DltDump.View
{
    using System;
    using Moq;
    using NUnit.Framework;
    using RJCP.Core.CommandLine;
    using static Infrastructure.OptionsGen;

    /// <summary>
    /// Tests the class CommandLine. The CommandFactory should be mocked so that it is not tested.
    /// </summary>
    [TestFixture]
    public class CommandLineTest
    {
        [Test]
        public void NoOptions()
        {
            // If no options are given, the application doesn't know what to do.
            Assert.That(CommandLine.Run(Array.Empty<string>()), Is.EqualTo(ExitCode.OptionsError));
        }

        private static void CommandFactorySetup(ExitCode result, Action<Options, CmdOptions> action)
        {
            // The command does nothing other than return the exit code requested.
            var commandMock = new Mock<ICommand>();
            commandMock.Setup(m => m.Run())
                .Returns(result);

            var factoryMock = new Mock<ICommandFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<Options>(), It.IsAny<CmdOptions>()))
                .Callback((Options cmdLine, CmdOptions opt) => { action(cmdLine, opt); })
                .Returns(commandMock.Object);

            // The CommandLine.Run() will use this factory.
            Global.Instance.CommandFactory = factoryMock.Object;
        }

        [Test]
        public void InvalidOption()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                // Ensures that CommandLine.Run calls the factory given the options. It only returns the error code,
                // because it is not allowed to call the command factory.
                Assert.That(CommandLine.Run(new[] {
                    LongOpt("invalidoption")
                }), Is.EqualTo(ExitCode.OptionsError));
                Assert.That(cmdOptions, Is.Null);
            }
        }

        [TestCase(ExitCode.Success)]
        [TestCase(ExitCode.OptionsError)]
        public void CommandLineRunExitCode(ExitCode result)
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(result, (cmdLine, opt) => cmdOptions = opt);

                // Ensures that CommandLine.Run calls the factory given the options.
                Assert.That(CommandLine.Run(Array.Empty<string>()), Is.EqualTo(result));
                Assert.That(cmdOptions, Is.Not.Null);
                Assert.That(cmdOptions.Help, Is.False);
                Assert.That(cmdOptions.Version, Is.False);
            }
        }

        [Test]
        public void HelpLong()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("help")
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Help, Is.True);
                Assert.That(cmdOptions.Version, Is.False);
            }
        }

        [Test]
        public void HelpShort()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    ShortOpt('?')
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Help, Is.True);
                Assert.That(cmdOptions.Version, Is.False);
            }
        }

        [Test]
        public void VersionLong()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("version")
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Help, Is.False);
                Assert.That(cmdOptions.Version, Is.True);
            }
        }

        [Test]
        public void HelpVersion()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("version"), ShortOpt('?')
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Help, Is.True);
                Assert.That(cmdOptions.Version, Is.True);
            }
        }

        [Test]
        public void Log()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(ExitCode.Success, (cmdLine, opt) => cmdOptions = opt);

                Assert.That(CommandLine.Run(new[] {
                    LongOpt("log")
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Log, Is.True);
            }
        }
    }
}
