namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTerminalTest
    {
        [Test]
        public void ConsoleTerminalObject()
        {
            ConsoleTerminal console = new ConsoleTerminal();

            Assert.That(console.StdOut, Is.Not.Null);
            Assert.That(console.StdOut, Is.TypeOf<StdOut>());

            Assert.That(console.StdErr, Is.Not.Null);
            Assert.That(console.StdErr, Is.TypeOf<StdErr>());
        }

        private static bool IsRedirected()
        {
            return Console.IsOutputRedirected && Console.IsErrorRedirected;
        }

        [Test]
        public void ForegroundColor()
        {
            ConsoleColor initialColor = IsRedirected() ? ConsoleColor.Gray : Console.ForegroundColor;
            try {
                ITerminal term = new ConsoleTerminal();
                Assert.That(term.ForegroundColor, Is.EqualTo(initialColor));

                term.ForegroundColor = ConsoleColor.Cyan;
                if (IsRedirected())
                    Assert.That(term.ForegroundColor, Is.EqualTo(ConsoleColor.Cyan));
                else
                    Assert.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Cyan));
            } finally {
                if (!IsRedirected()) Console.ForegroundColor = initialColor;
            }
        }

        [Test]
        public void BackgroundColor()
        {
            ConsoleColor initialColor = IsRedirected() ? ConsoleColor.Black : Console.BackgroundColor;
            try {
                ITerminal term = new ConsoleTerminal();
                Assert.That(term.BackgroundColor, Is.EqualTo(initialColor));

                term.BackgroundColor = ConsoleColor.Red;
                if (IsRedirected())
                    Assert.That(term.BackgroundColor, Is.EqualTo(ConsoleColor.Red));
                else
                    Assert.That(Console.BackgroundColor, Is.EqualTo(ConsoleColor.Red));
            } finally {
                if (!IsRedirected()) Console.BackgroundColor = initialColor;
            }
        }

        [Test]
        public void TerminalWidth()
        {
            ITerminal term = new ConsoleTerminal();
            Assert.That(term.TerminalWidth, Is.Not.EqualTo(0));
        }

        [Test]
        public void TerminalHeight()
        {
            ITerminal term = new ConsoleTerminal();
            Assert.That(term.TerminalHeight, Is.Not.EqualTo(0));
        }
    }
}
