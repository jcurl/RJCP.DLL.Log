namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture(typeof(StdOut))]
    [TestFixture(typeof(StdErr))]
    public class ConsoleTerminalOutTest<T> where T : ITerminalOut
    {
        private static ITerminalOut GetOutputWriter()
        {
            if (typeof(T) == typeof(StdOut)) return new StdOut();
            if (typeof(T) == typeof(StdErr)) return new StdErr();
            return null;
        }

        private static TextWriter GetOriginalOut()
        {
            if (typeof(T) == typeof(StdOut)) return Console.Out;
            if (typeof(T) == typeof(StdErr)) return Console.Error;
            return null;
        }

        private static void SetOutput(TextWriter writer)
        {
            if (typeof(T) == typeof(StdOut)) {
                Console.SetOut(writer);
            } else {
                Console.SetError(writer);
            }
        }

        [TestCase("Lorem ipsum dolor sit amet, duo accumsan scripserit ad", false, new string[0], TestName = "Console_Write")]
        [TestCase("Lorem ipsum dolor sit amet, duo accumsan {0} ad", true, new[] { "scripserit" }, TestName = "Console_WriteFormat")]
        public void ConsoleWrite(string messageDisplayed, bool format, string[] parameters)
        {
            TextWriter originalOut = GetOriginalOut();
            try {
                ITerminalOut writer = GetOutputWriter();

                if (format) messageDisplayed = string.Format(messageDisplayed, parameters);

                using (StringWriter strWriter = new StringWriter()) {
                    SetOutput(strWriter);

                    writer.Write(messageDisplayed);
                    Assert.That(strWriter.ToString(), Is.EqualTo(messageDisplayed));
                }
            } finally {
                SetOutput(originalOut);
            }
        }

        [TestCase("Lorem ipsum dolor sit amet, duo accumsan scripserit ad", false, new string[0], TestName = "Console_WriteLine")]
        [TestCase("Lorem ipsum dolor sit amet, duo accumsan {0} ad", true, new[] { "scripserit" }, TestName = "Console_WriteLineFormat")]
        public void ConsoleWriteLine(string message, bool format, string[] parameters)
        {
            TextWriter originalOut = GetOriginalOut();
            try {
                ITerminalOut writer = GetOutputWriter();

                if (format) message = string.Format(message, parameters);
                string expectedMessage = string.Concat(message, Environment.NewLine);

                using (StringWriter strWriter = new StringWriter()) {
                    SetOutput(strWriter);

                    writer.WriteLine(message);
                    Assert.That(strWriter.ToString(), Is.EqualTo(expectedMessage));
                }
            } finally {
                SetOutput(originalOut);
            }
        }
    }
}
