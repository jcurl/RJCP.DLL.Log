namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class FilterCommandTest
    {
        // The FilterCommand class needs a CmdOptions object as a constructor. We can't create this in a test case, so
        // must use the CommandLine functionality to do this for us, via CommandLine.Run(). Therefore, this test harness
        // tests both the command line conversion and the functionality of the FilterCommand.

        // Everything that the FilterApp needs would need to be mocked, and those components then tested separately to
        // minimize the amount of integration testing required.

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

        [Test]
        public void InputFileMissing()
        {
            string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "NonExistent.dlt");
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
                    file
                }), Is.EqualTo(ExitCode.OptionsError));

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

                Assert.That(CommandLine.Run(new string[] {
                    "EmptyFile.dlt"
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo("EmptyFile.dlt"));
            }
        }

        [Test]
        public void InputFileAbsolute()
        {
            string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
                    file
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(file));
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

                Assert.That(CommandLine.Run(new string[] {
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
            string file1 = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");
            string file2 = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile2.dlt");

            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
                    file1, file2
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(2));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(file1));
                Assert.That(cmdOptions.Arguments[1], Is.EqualTo(file2));
            }
        }
    }
}
