﻿namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Infrastructure.Dlt;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
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

        [Test]
        public void InputFileMissing()
        {
            string file = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "NonExistent.dlt");
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
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
            using (new TestApplication()) {
                // It checks that the file actually exists
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
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
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
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

                Assert.That(CommandLine.Run(new string[] {
                    new Uri(EmptyFile).AbsoluteUri
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(new Uri(EmptyFile).AbsoluteUri));
            }
        }

        [Test]
        public void ShowPosition()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
                    LongOpt("position"), EmptyFile
                }), Is.EqualTo(ExitCode.Success));
                Assert.That(cmdOptions.Arguments.Count, Is.EqualTo(1));
                Assert.That(cmdOptions.Arguments[0], Is.EqualTo(EmptyFile));
                Assert.That(cmdOptions.Position, Is.True);
            }
        }

        [Test]
        public void DefaultSetInputFormatFile()
        {
            using (new TestApplication()) {
                CmdOptions cmdOptions = null;
                CommandFactorySetup(opt => cmdOptions = opt);

                Assert.That(CommandLine.Run(new string[] {
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

                Assert.That(CommandLine.Run(new string[] {
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

                Assert.That(CommandLine.Run(new string[] {
                    LongOpt("format", "foo"), EmptyFile
                }), Is.EqualTo(ExitCode.OptionsError));
            }
        }
    }
}
