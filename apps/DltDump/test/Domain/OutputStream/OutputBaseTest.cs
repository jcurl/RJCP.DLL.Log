namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using System.Text;
    using Infrastructure.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
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
                    }, Throws.TypeOf<IOException>());
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
    }
}
