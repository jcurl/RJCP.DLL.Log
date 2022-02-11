namespace RJCP.App.DltDump.View
{
    using System;
    using NUnit.Framework;
    using static Infrastructure.OptionsGen;

    [TestFixture]
    public class CommandLineHelpTest
    {
        [Test]
        public void NoOptions()
        {
            Assert.That(CommandLine.Run(Array.Empty<string>()), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void HelpLong()
        {
            Assert.That(CommandLine.Run(new string[] {
                LongOpt("help")
            }), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void HelpShort()
        {
            Assert.That(CommandLine.Run(new string[] {
                ShortOpt('?')
            }), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void VersionLong()
        {
            Assert.That(CommandLine.Run(new string[] {
                LongOpt("version")
            }), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void HelpVersion()
        {
            Assert.That(CommandLine.Run(new string[] {
                LongOpt("version"), ShortOpt('?')
            }), Is.EqualTo(ExitCode.Success));
        }

        [Test]
        public void InvalidOption()
        {
            Assert.That(CommandLine.Run(new string[] {
                LongOpt("invalidoption")
            }), Is.EqualTo(ExitCode.OptionsError));
        }

        [Test]
        public void NotVersion()
        {
            Assert.That(CommandLine.Run(new string[] {
                ShortOpt('v')
            }), Is.EqualTo(ExitCode.OptionsError));
        }
    }
}
