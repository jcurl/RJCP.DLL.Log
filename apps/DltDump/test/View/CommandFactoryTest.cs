namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using static Infrastructure.OptionsGen;

    [TestFixture]
    public class CommandFactoryTest
    {
        private static void CommandFactorySetup(Action<ICommand> action)
        {
            // The command does nothing other than return the exit code success.
            var commandMock = new Mock<ICommand>();
            commandMock.Setup(m => m.Run())
                .Returns(ExitCode.Success);

            // Gets the command that was actually instantiated. Because the CmdOptions contains private setters, we have
            // to use the real CommandLine which will pares the data, to create our options object, which we can then
            // see that the correct Command object is returned. We won't ever execute that command here, that's for a
            // different test case.
            var factoryMock = new Mock<ICommandFactory>();
            factoryMock.Setup(m => m.Create(It.IsAny<CmdOptions>()))
                .Returns((CmdOptions opt) => {
                    var f = new CommandFactory();
                    ICommand command = f.Create(opt);
                    action(command);
                    return commandMock.Object;
                });

            // The CommandLine.Run() will use this factory.
            Global.Instance.CommandFactory = factoryMock.Object;
        }

        [Test]
        public void GetHelpCommand()
        {
            using (new TestApplication()) {
                ICommand command = null;
                CommandFactorySetup(cmd => { command = cmd; });

                // We don't need to test the different types, that's done in CommandLineTest.
                CommandLine.Run(new[] { LongOpt("help") });
                Assert.That(command, Is.TypeOf<HelpCommand>());

                HelpCommand helpCommand = (HelpCommand)command;
                Assert.That(helpCommand.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowHelp));
            }
        }

        [Test]
        public void GetVersionCommand()
        {
            using (new TestApplication()) {
                ICommand command = null;
                CommandFactorySetup(cmd => { command = cmd; });

                // We don't need to test the different types, that's done in CommandLineTest.
                CommandLine.Run(new[] { LongOpt("version") });
                Assert.That(command, Is.TypeOf<HelpCommand>());

                HelpCommand helpCommand = (HelpCommand)command;
                Assert.That(helpCommand.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowVersion));
            }
        }

        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        [Test]
        public void GetFilterCommand()
        {
            using (new TestApplication()) {
                ICommand command = null;
                CommandFactorySetup(cmd => { command = cmd; });

                CommandLine.Run(new[] { EmptyFile });
                Assert.That(command, Is.TypeOf<FilterCommand>());
            }
        }

        private readonly string FibexFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "valid", "fibex-tcb.xml");

        [Test]
        public void GetNonVerboseCommand()
        {
            using (new TestApplication()) {
                ICommand command = null;
                CommandFactorySetup(cmd => { command = cmd; });

                CommandLine.Run(new[] {
                    LongOpt("fibex", FibexFile)
                });
                Assert.That(command, Is.TypeOf<NonVerboseCommand>());
            }
        }

        [Test]
        public void GetFilterCommandWithFibex()
        {
            using (new TestApplication()) {
                ICommand command = null;
                CommandFactorySetup(cmd => { command = cmd; });

                CommandLine.Run(new[] {
                    LongOpt("fibex", FibexFile), EmptyFile
                });
                Assert.That(command, Is.TypeOf<FilterCommand>());
            }
        }
    }
}
