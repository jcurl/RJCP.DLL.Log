namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Domain.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core.Environment;
    using TestResources;

    [TestFixture]
    public class OutputBaseTest
    {
        [Test]
        public void NullFileName()
        {
            Assert.That(() => {
                _ = new TestOutputBase(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyFileName()
        {
            Assert.That(() => {
                _ = new TestOutputBase(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WriteTextLine()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void WriteFormatTextLine()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                output.ShowPosition = true;

                Assert.That(File.Exists("File.txt"), Is.False);

                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(10 + TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void WriteMultipleTextLine()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                int expectedLength =
                    TestLines.Verbose.ToString().Length + Environment.NewLine.Length +
                    TestLines.Verbose2.ToString().Length + Environment.NewLine.Length;
                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(expectedLength));
            }
        }

        [Test]
        public void WriteAsciiTextLine()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                output.Encoding = Encoding.ASCII;
                Assert.That(File.Exists("File.txt"), Is.False);

                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void NullEncoding()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                Assert.That(() => {
                    output.Encoding = null;
                }, Throws.TypeOf<ArgumentNullException>());

                Assert.That(output.Encoding, Is.Not.Null);
            }
        }

        private static readonly byte[] PacketFile = new byte[] {
            0x44, 0x43, 0x5C, 0x01,                         // DLT\1
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header Time Stamp
            0x45, 0x43, 0x55, 0x02,                         // ECUID
            0x3d, 0x00, 0x00, 0x22,                         //
            0x45, 0x43, 0x55, 0x02,                         // ECUID
            0x00, 0x00, 0x00, 0x00,                         // Session ID
            0x00, 0x00, 0x00, 0x00,                         // Time Stamp
            0x41, 0x01,                                     // Log Info, 1 arg
            0x44, 0x4c, 0x54, 0x31,                         // App ID
            0x44, 0x4c, 0x54, 0x32,                         // CTX ID
            0x00, 0x02, 0x00, 0x00,                         // Arg 1: String
            0x03, 0x41, 0x31, 0x00                          //   A1
        };

        private static readonly byte[] PacketNet = new byte[] {
            0x3d, 0x00, 0x00, 0x22,                         //
            0x45, 0x43, 0x55, 0x02,                         // ECUID
            0x00, 0x00, 0x00, 0x00,                         // Session ID
            0x00, 0x00, 0x00, 0x00,                         // Time Stamp
            0x41, 0x01,                                     // Log Info, 1 arg
            0x44, 0x4c, 0x54, 0x31,                         // App ID
            0x44, 0x4c, 0x54, 0x32,                         // CTX ID
            0x00, 0x02, 0x00, 0x00,                         // Arg 1: String
            0x03, 0x41, 0x31, 0x00                          //   A1
        };

        private static readonly byte[] PacketSerial = new byte[] {
            0x44, 0x4c, 0x53, 0x01,                         // DLS\1
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header Time Stamp
            0x45, 0x43, 0x55, 0x02,                         // ECUID
            0x3d, 0x00, 0x00, 0x22,                         //
            0x45, 0x43, 0x55, 0x02,                         // ECUID
            0x00, 0x00, 0x00, 0x00,                         // Session ID
            0x00, 0x00, 0x00, 0x00,                         // Time Stamp
            0x41, 0x01,                                     // Log Info, 1 arg
            0x44, 0x4c, 0x54, 0x31,                         // App ID
            0x44, 0x4c, 0x54, 0x32,                         // CTX ID
            0x00, 0x02, 0x00, 0x00,                         // Arg 1: String
            0x03, 0x41, 0x31, 0x00                          //   A1
        };

        [Test]
        public void WriteFullPacket()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, PacketFile);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(PacketFile.Length));
            }
        }

        [Test]
        public void WriteFullPacketFromNetwork()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.Network);
                output.Write(TestLines.Verbose, PacketNet);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(PacketNet.Length + 16));
            }
        }

        [Test]
        public void WriteFullPacketFromSerial()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.Serial);
                output.Write(TestLines.Verbose, PacketSerial);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(PacketSerial.Length + 12));
            }
        }

        [Test]
        public void WriteTextLineAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                Assert.That(File.Exists("File.txt"), Is.False);

                output.Dispose();
                Assert.That(() => {
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteFormatTextLineAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                output.ShowPosition = true;

                Assert.That(File.Exists("File.txt"), Is.False);

                output.Dispose();
                Assert.That(() => {
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteFullPacketAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Dispose();
                Assert.That(() => {
                    output.Write(TestLines.Verbose, PacketFile);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteFullPacketFromNetworkAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.Network);
                output.Dispose();
                Assert.That(() => {
                    output.Write(TestLines.Verbose, PacketNet);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteFullPacketFromSerialAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.Serial);
                output.Dispose();
                Assert.That(() => {
                    output.Write(TestLines.Verbose, PacketSerial);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void FlushAfterDispose()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.Serial);
                output.Write(TestLines.Verbose, PacketSerial);
                output.Dispose();
                Assert.That(() => {
                    output.Flush();
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        private static readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        [Test]
        public void OutputBaseNoForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                Deploy.Item(EmptyFile, pad.Path);

                using (TestOutputBase output = new TestOutputBase("EmptyFile.dlt", false)) {
                    Assert.That(output.Force, Is.False);

                    output.SetInput("input.dlt", InputFormat.Serial);
                    Assert.That(() => {
                        // Will fail, because the file already exists
                        output.Write(TestLines.Verbose);
                    }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());
                }
            }
        }

        [Test]
        public void OutputBaseWithForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                Deploy.Item(EmptyFile, pad.Path);

                using (TestOutputBase output = new TestOutputBase("EmptyFile.dlt", true)) {
                    Assert.That(output.Force, Is.True);

                    output.SetInput("input.dlt", InputFormat.Serial);
                    output.Write(TestLines.Verbose);
                    output.Flush();

                    FileInfo fileInfo = new FileInfo("EmptyFile.dlt");
                    Assert.That(fileInfo.Length,
                        Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                }
            }
        }

        [Test]
        public void TemplateEnvironmentVar()
        {
            Environment.SetEnvironmentVariable("TESTVAR", "testVar");
            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File_%TESTVAR%.txt", false)) {
                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("File_testVar.txt"), Is.True);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateFile(bool force)
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", force)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                Assert.That(File.Exists("input.txt"), Is.True);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateFileCollision(bool force)
        {
            string inputFile1 = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";
            string inputFile2 = Platform.IsWinNT() ? @"c:\foo\input.dlt" : "/foo/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", force)) {
                output.SetInput(inputFile1, InputFormat.File);
                output.Write(TestLines.Verbose);
                Assert.That(() => {
                    output.SetInput(inputFile2, InputFormat.File);

                    // The 'Write' causes the exceptions as opens are delayed to know the time stamp, etc. The exception
                    // should occur regardless if the file exists or not.
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.Null);

                Assert.That(File.Exists("input.txt"), Is.True);
            }
        }

        [Test]
        public void TemplateFileExistsNoForce()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", false)) {
                pad.DeployEmptyFile("input.txt");
                Assert.That(() => {
                    output.SetInput(inputFile, InputFormat.File);

                    // The 'Write' causes the exceptions as opens are delayed to know the time stamp, etc. The file
                    // already exists, and so opening it should result in an exception.
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());

                Assert.That(File.Exists("input.txt"), Is.True);
            }
        }

        [Test]
        public void TemplateFileExistsNoForceTwoInputs()
        {
            string inputFile1 = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";
            string inputFile2 = Platform.IsWinNT() ? @"c:\other.dlt" : "/other.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", false)) {
                pad.DeployEmptyFile("input.txt");
                Assert.That(() => {
                    output.SetInput(inputFile1, InputFormat.File);

                    // The 'Write' causes the exceptions as opens are delayed to know the time stamp, etc. The file
                    // already exists, and so opening it should result in an exception.
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());

                output.SetInput(inputFile2, InputFormat.File);
                output.Write(TestLines.Verbose);

                Assert.That(File.Exists("input.txt"), Is.True);
                Assert.That(File.Exists("other.txt"), Is.True);
            }
        }

        [Test]
        public void TemplateFileExistsForce()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", true)) {
                pad.DeployEmptyFile("input.txt");
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                FileInfo fileInfo = new FileInfo("input.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void TemplateDateTime()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%CDATETIME%.txt", true)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                string fileName = string.Format("{0:yyyyMMdd\\THHmmss}.txt", TestLines.Verbose.TimeStamp.ToLocalTime());

                FileInfo fileInfo = new FileInfo(fileName);
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void TemplateDate()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%CDATE%.txt", true)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                string fileName = string.Format("{0:yyyyMMdd}.txt", TestLines.Verbose.TimeStamp.ToLocalTime());

                FileInfo fileInfo = new FileInfo(fileName);
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void TemplateTime()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%CTIME%.txt", true)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Flush();

                string fileName = string.Format("{0:HHmmss}.txt", TestLines.Verbose.TimeStamp.ToLocalTime());

                FileInfo fileInfo = new FileInfo(fileName);
                Assert.That(fileInfo.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateSplitNone(bool force)
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file.txt", 1, force)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                long expectedLength =
                    TestLines.Verbose.ToString().Length + Environment.NewLine.Length +
                    TestLines.Verbose2.ToString().Length + Environment.NewLine.Length;
                FileInfo fileInfo = new FileInfo("file.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(expectedLength));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateSplitCtr(bool force)
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file_%CTR%.txt", 1, force)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                FileInfo fileInfo1 = new FileInfo("file_001.txt");
                Assert.That(fileInfo1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2 = new FileInfo("file_002.txt");
                Assert.That(fileInfo2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateSplitDateTime(bool force)
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file_%CDATETIME%.txt", 1, force)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                string date1 = TestLines.Verbose.TimeStamp.ToLocalTime().ToString(@"yyyyMMdd\THHmmss");
                string date2 = TestLines.Verbose2.TimeStamp.ToLocalTime().ToString(@"yyyyMMdd\THHmmss");

                FileInfo fileInfo1 = new FileInfo($"file_{date1}.txt");
                Assert.That(fileInfo1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2 = new FileInfo($"file_{date2}.txt");
                Assert.That(fileInfo2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TemplateSplitDateSame(bool force)
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file_%CDATE%.txt", 1, force)) {
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose);
                output.Flush();

                string date = TestLines.Verbose.TimeStamp.ToLocalTime().ToString(@"yyyyMMdd");

                long expectedLength =
                    TestLines.Verbose.ToString().Length + Environment.NewLine.Length;
                FileInfo fileInfo1 = new FileInfo($"file_{date}.txt");
                Assert.That(fileInfo1.Length, Is.EqualTo(expectedLength * 2));
            }
        }

        [Test]
        public void TemplateSplitCtrExistsForce()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file_%CTR%.txt", 1, true)) {
                pad.DeployEmptyFile("file_002.txt");
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                FileInfo fileInfo1 = new FileInfo("file_001.txt");
                Assert.That(fileInfo1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2 = new FileInfo("file_002.txt");
                Assert.That(fileInfo2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void TemplateSplitCtrExistsNoForce()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("file_%CTR%.txt", 1, false)) {
                pad.DeployEmptyFile("file_002.txt");
                output.SetInput(inputFile, InputFormat.File);
                output.Write(TestLines.Verbose);
                Assert.That(() => {
                    output.Write(TestLines.Verbose2);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());
                Assert.That(() => {
                    output.Write(TestLines.Verbose2);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());
                output.Flush();

                FileInfo fileInfo1 = new FileInfo("file_001.txt");
                Assert.That(fileInfo1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2 = new FileInfo("file_002.txt");
                Assert.That(fileInfo2.Length, Is.EqualTo(0));  // File was not overwritten

                // And we didn't get to create the third file.
                Assert.That(File.Exists("file_003.txt"), Is.False);
            }
        }

        [Test]
        public void TemplateSplitMultipleFiles()
        {
            string inputFile1 = Platform.IsWinNT() ? @"c:\input1.dlt" : "/input1.dlt";
            string inputFile2 = Platform.IsWinNT() ? @"c:\input2.dlt" : "/input2.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%_%CTR%.txt", 1, false)) {
                output.SetInput(inputFile1, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                output.SetInput(inputFile2, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                FileInfo fileInfo1_1 = new FileInfo("input1_001.txt");
                Assert.That(fileInfo1_1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo1_2 = new FileInfo("input1_002.txt");
                Assert.That(fileInfo1_2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));

                FileInfo fileInfo2_1 = new FileInfo("input2_001.txt");
                Assert.That(fileInfo2_1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2_2 = new FileInfo("input2_002.txt");
                Assert.That(fileInfo2_2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void TemplateSplitMultipleFilesInput1Exists()
        {
            string inputFile1 = Platform.IsWinNT() ? @"c:\input1.dlt" : "/input1.dlt";
            string inputFile2 = Platform.IsWinNT() ? @"c:\input2.dlt" : "/input2.dlt";

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%_%CTR%.txt", 1, false)) {
                pad.DeployEmptyFile("input1_002.txt");
                output.SetInput(inputFile1, InputFormat.File);
                output.Write(TestLines.Verbose);
                Assert.That(() => {
                    output.Write(TestLines.Verbose2);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());
                output.Flush();

                output.SetInput(inputFile2, InputFormat.File);
                output.Write(TestLines.Verbose);
                output.Write(TestLines.Verbose2);
                output.Flush();

                FileInfo fileInfo1_1 = new FileInfo("input1_001.txt");
                Assert.That(fileInfo1_1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo1_2 = new FileInfo("input1_002.txt");
                Assert.That(fileInfo1_2.Length, Is.EqualTo(0));

                FileInfo fileInfo2_1 = new FileInfo("input2_001.txt");
                Assert.That(fileInfo2_1.Length,
                    Is.EqualTo(TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
                FileInfo fileInfo2_2 = new FileInfo("input2_002.txt");
                Assert.That(fileInfo2_2.Length,
                    Is.EqualTo(TestLines.Verbose2.ToString().Length + Environment.NewLine.Length));
            }
        }

        [Test]
        public void ProtectedFiles()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            // The `IOutputStreamFactory` would define this and give it to the `IOutputStream` object.
            InputFiles inputs = new InputFiles();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", inputs, 1, true)) {
                using (Stream newFile = new FileStream("input.txt", FileMode.CreateNew)) { /* Empty File */ }

                // Must be added any time before any write is done.
                inputs.AddProtectedFile("input.txt");
                output.SetInput(inputFile, InputFormat.File);
                Assert.That(() => {
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.Null);
                output.Flush();
            }
        }

        [Test]
        [Platform(Include = "Win32")]
        public void ProtectedFilesCaseInsensitive()
        {
            string inputFile = Platform.IsWinNT() ? @"c:\input.dlt" : "/input.dlt";

            // The `IOutputStreamFactory` would define this and give it to the `IOutputStream` object.
            InputFiles inputs = new InputFiles();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("%FILE%.txt", inputs, 1, true)) {
                using (Stream newFile = new FileStream("input.txt", FileMode.CreateNew)) { /* Empty File */ }

                // Must be added any time before any write is done.
                inputs.AddProtectedFile("INPUT.txt");
                output.SetInput(inputFile, InputFormat.File);
                Assert.That(() => {
                    output.Write(TestLines.Verbose);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.Null);
                output.Flush();
            }
        }

        [Test]
        public void AutoFlushPeriod()
        {
            // We can set the AutoFlushPeriod, but it's not possible to test as there is no way to easily confirm it
            // gave it to the OutputWriter, other than through coverage in the private OpenWriter method.

            using (ScratchPad pad = Deploy.ScratchPad())
            using (TestOutputBase output = new TestOutputBase("File.txt")) {
                output.ShowPosition = true;
                output.AutoFlushPeriod = 50;

                Assert.That(File.Exists("File.txt"), Is.False);

                output.Write(TestLines.Verbose);
                Thread.Sleep(500);

                Assert.That(File.Exists("File.txt"), Is.True);

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length,
                    Is.EqualTo(10 + TestLines.Verbose.ToString().Length + Environment.NewLine.Length));
            }
        }
    }
}
