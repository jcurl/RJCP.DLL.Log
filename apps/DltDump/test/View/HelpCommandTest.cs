namespace RJCP.App.DltDump.View
{
    using NUnit.Framework;
    using RJCP.Core.CommandLine;

    [TestFixture]
    public class HelpCommandTest
    {
        [Test]
        public void ShowHelpCommand()
        {
            Options cmdLine = Options.Parse(null, null);
            HelpCommand cmd = new(cmdLine, HelpCommand.Mode.ShowHelp);
            Assert.That(cmd.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowHelp));
            Assert.That(cmd.Run(), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void ShowVersionCommand()
        {
            Options cmdLine = Options.Parse(null, null);
            HelpCommand cmd = new(cmdLine, HelpCommand.Mode.ShowVersion);
            Assert.That(cmd.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowVersion));
            Assert.That(cmd.Run(), Is.EqualTo(ExitCode.Success));
        }
    }
}
