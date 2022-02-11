namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VirtualOutputTest
    {
        [Test]
        public void BackgroundColor()
        {
            VirtualTerminal term = new VirtualTerminal {
                BackgroundColor = ConsoleColor.Blue
            };
            Assert.That(term.BackgroundColor, Is.EqualTo(ConsoleColor.Blue));
        }

        [Test]
        public void ForegroundColor()
        {
            VirtualTerminal term = new VirtualTerminal {
                ForegroundColor = ConsoleColor.Red
            };
            Assert.That(term.ForegroundColor, Is.EqualTo(ConsoleColor.Red));
        }

        [Test]
        public void Write()
        {
            VirtualTerminal term = new VirtualTerminal();
            term.StdErr.Write("standard error");
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("standard error"));

            term.StdOut.Write("standard output");
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("standard output"));
        }

        [Test]
        public void WriteFormat()
        {
            VirtualTerminal term = new VirtualTerminal();
            term.StdErr.Write("standard error : {0}", 1);
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("standard error : 1"));

            term.StdOut.Write("standard output : {0}", 0);
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("standard output : 0"));
        }

        [Test]
        public void WriteLine()
        {
            VirtualTerminal term = new VirtualTerminal();
            term.StdErr.WriteLine("standard error");
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("standard error"));

            term.StdOut.WriteLine("standard output");
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("standard output"));
        }

        [Test]
        public void WriteLineFormat()
        {
            VirtualTerminal term = new VirtualTerminal();
            term.StdErr.WriteLine("standard error : {0}", 1);
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("standard error : 1"));

            term.StdOut.WriteLine("standard output : {0}", 0);
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(1));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("standard output : 0"));
        }

        private static void Write(ITerminalOut writer, string line, bool charwise, bool writeLine)
        {
            if (!charwise) {
                if (!writeLine) {
                    writer.Write(line);
                } else {
                    writer.WriteLine(line);
                }
            } else {
                for (int i = 0; i < line.Length; i++) {
                    string c = line[i].ToString();
                    if (i != line.Length - 1 || !writeLine) {
                        writer.Write(c);
                    } else {
                        writer.WriteLine(c);
                    }
                }
            }
        }

        [TestCase("print\rformat\rline\r", false, false, TestName = "SplitWriteThreeLines(Mac, NewLine)")]
        [TestCase("print\nformat\nline\n", false, false, TestName = "SplitWriteThreeLines(Unix, NewLine)")]
        [TestCase("print\r\nformat\r\nline\r\n", false, false, TestName = "SplitWriteThreeLines(Windows, NewLine)")]
        [TestCase("print\rformat\rline", false, false, TestName = "SplitWriteThreeLines(Mac)")]
        [TestCase("print\nformat\nline", false, false, TestName = "SplitWriteThreeLines(Unix)")]
        [TestCase("print\r\nformat\r\nline", false, false, TestName = "SplitWriteThreeLines(Windows)")]
        [TestCase("print\rformat\rline", false, true, TestName = "SplitWriteLineThreeLines(Mac)")]
        [TestCase("print\nformat\nline", false, true, TestName = "SplitWriteLineThreeLines(Unix)")]
        [TestCase("print\r\nformat\r\nline", false, true, TestName = "SplitWriteLineThreeLines(Windows)")]
        [TestCase("print\rformat\rline\r", true, false, TestName = "CharWriteThreeLines(Mac, NewLine)")]
        [TestCase("print\nformat\nline\n", true, false, TestName = "CharWriteThreeLines(Unix, NewLine)")]
        [TestCase("print\r\nformat\r\nline\r\n", true, false, TestName = "CharWriteThreeLines(Windows, NewLine)")]
        [TestCase("print\rformat\rline", true, false, TestName = "CharWriteThreeLines(Mac)")]
        [TestCase("print\nformat\nline", true, false, TestName = "CharWriteThreeLines(Unix)")]
        [TestCase("print\r\nformat\r\nline", true, false, TestName = "CharWriteThreeLines(Windows)")]
        [TestCase("print\rformat\rline", true, true, TestName = "CharWriteLineThreeLines(Mac)")]
        [TestCase("print\nformat\nline", true, true, TestName = "CharWriteLineThreeLines(Unix)")]
        [TestCase("print\r\nformat\r\nline", true, true, TestName = "CharWriteLineThreeLines(Windows)")]
        public void SplitWriteThreeLines(string line, bool charwise, bool writeLine)
        {
            VirtualTerminal term = new VirtualTerminal();
            Write(term.StdErr, line, charwise, writeLine);
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(3));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("print"));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[1], Is.EqualTo("format"));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[2], Is.EqualTo("line"));

            Write(term.StdOut, line, charwise, writeLine);
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(3));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("print"));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[1], Is.EqualTo("format"));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[2], Is.EqualTo("line"));
        }

        [TestCase("print\rformat\rline\r", false, TestName = "SplitWriteLineThreeLinesWithNewLine(Mac)")]
        [TestCase("print\nformat\nline\n", false, TestName = "SplitWriteLineThreeLinesWithNewLine(Unix)")]
        [TestCase("print\r\nformat\r\nline\r\n", false, TestName = "SplitWriteLineThreeLinesWithNewLine(Windows)")]
        [TestCase("print\rformat\rline\r", true, TestName = "CharWriteLineThreeLinesWithNewLine(Mac)")]
        [TestCase("print\nformat\nline\n", true, TestName = "CharWriteLineThreeLinesWithNewLine(Unix)")]
        [TestCase("print\r\nformat\r\nline\r\n", true, TestName = "CharWriteLineThreeLinesWithNewLine(Windows)")]
        public void SplitWriteLineThreeLinesWithNewLine(string line, bool charwise)
        {
            VirtualTerminal term = new VirtualTerminal();
            Write(term.StdErr, line, charwise, true);
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(4));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[0], Is.EqualTo("print"));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[1], Is.EqualTo("format"));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[2], Is.EqualTo("line"));
            Assert.That(((VirtualStdErr)term.StdErr).Lines[3], Is.Empty);

            Write(term.StdOut, line, charwise, true);
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(4));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[0], Is.EqualTo("print"));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[1], Is.EqualTo("format"));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[2], Is.EqualTo("line"));
            Assert.That(((VirtualStdOut)term.StdOut).Lines[3], Is.Empty);
        }

        [TestCase("\r\r\r", false, false, 3, TestName = "SplitWriteThreeEmptyLines(Mac)")]
        [TestCase("\n\n\n", false, false, 3, TestName = "SplitWriteThreeEmptyLines(Unix)")]
        [TestCase("\r\n\r\n\r\n", false, false, 3, TestName = "SplitWriteThreeEmptyLines(Windows)")]
        [TestCase("\r\r\r", false, true, 4, TestName = "SplitWriteLineThreeEmptyLines(Mac)")]
        [TestCase("\n\n\n", false, true, 4, TestName = "SplitWriteLineThreeEmptyLines(Unix)")]
        [TestCase("\r\n\r\n\r\n", false, true, 4, TestName = "SplitWriteLineThreeEmptyLines(Windows)")]
        [TestCase("\r\r\r", true, false, 3, TestName = "CharWriteThreeEmptyLines(Mac)")]
        [TestCase("\n\n\n", true, false, 3, TestName = "CharWriteThreeEmptyLines(Unix)")]
        [TestCase("\r\n\r\n\r\n", true, false, 3, TestName = "CharWriteThreeEmptyLines(Windows)")]
        [TestCase("\r\r\r", true, true, 4, TestName = "CharWriteLineThreeEmptyLines(Mac)")]
        [TestCase("\n\n\n", true, true, 4, TestName = "CharWriteLineThreeEmptyLines(Unix)")]
        [TestCase("\r\n\r\n\r\n", true, true, 4, TestName = "CharWriteLineThreeEmptyLines(Windows)")]
        public void SplitWriteThreeEmptyLines(string line, bool charwise, bool writeLine, int count)
        {
            VirtualTerminal term = new VirtualTerminal();
            Write(term.StdErr, line, charwise, writeLine);
            Assert.That(((VirtualStdErr)term.StdErr).Lines.Count, Is.EqualTo(count));
            for (int i = 0; i < count; i++) {
                string terminalLine = ((VirtualStdErr)term.StdErr).Lines[i];
                Assert.That(terminalLine, Is.Empty, "StdErr {0} is not empty", i);
            }

            Write(term.StdOut, line, charwise, writeLine);
            Assert.That(((VirtualStdOut)term.StdOut).Lines.Count, Is.EqualTo(count));
            for (int i = 0; i < count; i++) {
                string terminalLine = ((VirtualStdOut)term.StdOut).Lines[i];
                Assert.That(terminalLine, Is.Empty, "StdOut {0} is not empty", i);
            }
        }
    }
}
