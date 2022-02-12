namespace RJCP.App.DltDump.View
{
    using NUnit.Framework;

    [TestFixture]
    public class HelpCommandTest
    {
        [Test]
        public void ShowHelpCommand()
        {
            HelpCommand cmd = new HelpCommand(HelpCommand.Mode.ShowHelp);
            Assert.That(cmd.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowHelp));
            Assert.That(cmd.Run(), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void ShowVersionCommand()
        {
            HelpCommand cmd = new HelpCommand(HelpCommand.Mode.ShowVersion);
            Assert.That(cmd.HelpMode, Is.EqualTo(HelpCommand.Mode.ShowVersion));
            Assert.That(cmd.Run(), Is.EqualTo(ExitCode.Success));
        }
    }
}
