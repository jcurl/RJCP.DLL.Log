namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class TeraTermTraceReaderTest
    {
        private static async Task TextStreamCheck(ITraceReader<LogTraceLine> reader, IEnumerable<LogTraceLine> lines)
        {
            LogTraceLine line;
            foreach (LogTraceLine expectedLine in lines) {
                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo(expectedLine.Text));
                Assert.That(line.Line, Is.EqualTo(expectedLine.Line),
                    $"Expected Line {expectedLine.Line}; got {line.Line} for '{expectedLine.Text}'");
                Assert.That(line.TimeStamp, Is.EqualTo(expectedLine.TimeStamp),
                    $"Expected Line {expectedLine.Line}; got {line.Line} for '{expectedLine.Text}'");
                Assert.That(line.TimeStamp.Kind, Is.EqualTo(DateTimeKind.Local));
            }
            line = await reader.GetLineAsync();
            Assert.That(line, Is.Null);
        }

        [Test]
        public async Task ReadTeraTermFile()
        {
            string path = Path.Combine(Deploy.TestDirectory, "TestResources", "TextFiles", "TeraTerm.txt");
            using (ITraceReader<LogTraceLine> reader = await new TeraTermTraceReaderFactory().CreateAsync(path)) {
                LogTraceLine[] expected = {
                    new LogTraceLine("login: root", 0, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 861) },
                    new LogTraceLine("Password: ", 1, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 861) },
                    new LogTraceLine("hostname:/dev/shmem> *** JCURL:", 2, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 971) },
                    new LogTraceLine("*** JCURL: -=> 1 <=-", 3, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 971) },
                    new LogTraceLine("*** JCURL:", 4, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 971) },
                    new LogTraceLine("/opt/bin/LcCommander appreset", 5, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 30, 971) },
                    new LogTraceLine("application reset", 6, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 32, 96) },
                    new LogTraceLine("[MSG]@229406 (4104.6): LcRequester: Exec: Current=Normal; Requested=Reset", 7, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 32, 393) },
                    new LogTraceLine("[MSG]@230547 (4104.6): LcRequester: Exec: Current=_preResetNormal; Requested=Reset", 8, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 33, 533) },
                    new LogTraceLine("System shutting down", 9, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 33, 580) },
                    new LogTraceLine(string.Empty, 10, 0)
                        { TimeStamp = new DateTime(2011, 8, 19, 16, 25, 33, 580) },
                    new LogTraceLine(string.Empty, 11, 0)
                        { TimeStamp = new DateTime(2021, 1, 1, 0, 0, 0) },
                    new LogTraceLine("Log File Parsed", 12, 0)
                        { TimeStamp = new DateTime(2021, 1, 1, 0, 0, 0) }
                };

                await TextStreamCheck(reader, expected);
            }
        }

        [Test]
        public async Task ReadTextFile()
        {
            string path = Path.Combine(Deploy.TestDirectory, "TestResources", "TextFiles", "TextFile.txt");
            using (ITraceReader<LogTraceLine> reader = await new TeraTermTraceReaderFactory().CreateAsync(path)) {
                LogTraceLine[] expected = {
                    new LogTraceLine("This is Line 1", 0, 0)
                        { TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0) },
                    new LogTraceLine("And now for Line 2.", 1, 0)
                        { TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0) },
                };

                await TextStreamCheck(reader, expected);
            }
        }

        [TestCase("[foo]")]
        [TestCase("[Fri Aus 19 16:25:30.861 2011] Test")]       // Month is unknown
        [TestCase("[Fri Aug 32 16:25:30.861 2011] Test")]       // Date is out of range
        [TestCase("[Fri Aug 19 24:25:30.861 2011] Test")]       // Hour is out of range
        [TestCase("[Fri Aug 19 16:61:30.861 2011] Test")]       // Minute is out of range
        [TestCase("[Fri Aug 19 16:25:61.861 2011] Test")]       // Second is out of range
        [TestCase("[Fri Aug xx 16:25:61.861 2011] Test")]       // Date is unknown
        [TestCase("[Fri Aug 19 xx:25:30.861 2011] Test")]       // Hour is unknown
        [TestCase("[Fri Aug 19 16:xx:30.861 2011] Test")]       // Minute is unknown
        [TestCase("[Fri Aug 19 16:25:xx.861 2011] Test")]       // Second is unknown
        [TestCase("[Fri Aug 19 16:25:30.x61 2011] Test")]       // Milliseconds is unknown
        [TestCase("[Fri Aug 19 16:25:30.861 2x11] Test")]       // Year is unknown
        [TestCase("[Fri Aug 19 16:25:30.861 -999] Test")]       // Year is unknown
        public async Task ParseInvalidTimeStamp(string line)
        {
            byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(line);
            using (Stream stream = new MemoryStream(buffer))
            using (ITraceReader<LogTraceLine> reader = await new TeraTermTraceReaderFactory().CreateAsync(stream)) {
                LogTraceLine logLine = await reader.GetLineAsync();

                Assert.That(logLine.Text, Is.EqualTo(line));
            }
        }
    }
}
