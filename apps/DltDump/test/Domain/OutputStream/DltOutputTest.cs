namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using Domain.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;
    using RJCP.Diagnostics.Log.Dlt.ControlArgs;
    using TestResources;

    [TestFixture]
    public class DltOutputTest
    {
        private static readonly byte[] FileData = new byte[] {
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };

        private static readonly byte[] TcpData = new byte[] {
            0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };

        private static readonly byte[] SerData = new byte[] {
            0x44, 0x4C, 0x53, 0x01, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };

        private static byte[] GetFile(string fileName)
        {
            using (MemoryStream mem = new())
            using (FileStream file = new(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                file.CopyTo(mem);
                return mem.ToArray();
            }
        }

        [Test]
        public void NullFileName()
        {
            Assert.That(() => {
                _ = new DltOutput(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyFileName()
        {
            Assert.That(() => {
                _ = new DltOutput(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void DefaultProperties()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.False);
            }
        }

        [Test]
        public void DefaultPropertiesWithForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt", true)) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.True);
            }
        }

        [Test]
        public void WriteLineAsPacketFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length));
            }
        }

        [Test]
        public void WriteLinesAsPacketFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Write(TestLines.Verbose2, FileData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length * 2));
            }
        }

        [Test]
        public void WriteLineAsPacketTcp()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.raw", InputFormat.Network);
                output.Write(TestLines.Verbose, TcpData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. A storage header is added.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(TcpData.Length + 16));
            }
        }

        [Test]
        public void WriteLineAsPacketSer()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.ser", InputFormat.Serial);
                output.Write(TestLines.Verbose, SerData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. A storage header is added, and the serial header is removed.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(SerData.Length + 12));
            }
        }

        [Test]
        public void WriteLineAsPacketPcap()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.pcap", InputFormat.Pcap);
                output.Write(TestLines.Verbose, TcpData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. A storage header is added.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(TcpData.Length + 16));
            }
        }

        [Test]
        public void WriteLineAsPacketPcapNg()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.pcapng", InputFormat.Pcap);
                output.Write(TestLines.Verbose, TcpData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. A storage header is added.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(TcpData.Length + 16));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringInitNoFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringInitFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File_%FILE%.dlt")) {
                Assert.That(File.Exists("File_.dlt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                Assert.That(File.Exists("File_.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File_.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputNullStringFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File_%FILE%.dlt")) {
                Assert.That(File.Exists("File_.dlt"), Is.False);
                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                Assert.That(File.Exists("File_.dlt"), Is.True);

                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new("File_.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(2 * FileData.Length));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void SetInputFromValidToNullStringFileTemplate(string inputFileName)
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File_%FILE%.dlt")) {
                Assert.That(File.Exists("File_.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                Assert.That(File.Exists("File_input.dlt"), Is.True);
                FileInfo fileInfo = new("File_input.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length));

                output.SetInput(inputFileName, InputFormat.File);
                output.Write(TestLines.Verbose, FileData);
                output.Flush();

                // The data is written exactly as the packet says. There is no interpretation of the data.
                Assert.That(File.Exists("File_.dlt"), Is.True);
                FileInfo fileInfoEmpty = new("File_.dlt");
                Assert.That(fileInfoEmpty.Length, Is.EqualTo(FileData.Length));
            }
        }

        [Test]
        public void SetInputUnknownFormat()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", (InputFormat)255);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputUnsupportedFormatAutomatic()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput("input.dlt", InputFormat.Automatic);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void SetInputValid()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);

                // The file is still only created when written.
                Assert.That(File.Exists("File.dlt"), Is.False);
            }
        }

        [Test]
        public void WriteControlMessage()
        {
            DltControlTraceLine line =
                new(new GetSoftwareVersionResponse(ControlResponse.StatusOk, "Version"));

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                // We can now inject control messages. Although the software doesn't do this.
                Assert.That(File.Exists("out.dlt"), Is.True);
            }
        }

        [Test]
        public void WriteSkippedLine()
        {
            DltSkippedTraceLine line = new(100, "Test") {
                EcuId = "ECU1",
                Count = 50,
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                DeviceTimeStamp = new TimeSpan(0, 0, 0, 55, 80),
            };
            line.Features += DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature;

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x45, 0x43, 0x55, 0x31, // Storage Header
                0x35, 0x32, 0x00, 0x4A, 0x45, 0x43, 0x55, 0x31, 0x00, 0x08, 0x67, 0x90,                         // Standard Header
                0x31, 0x04, 0x53, 0x4B, 0x49, 0x50, 0x53, 0x4B, 0x49, 0x50,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x09, 0x00, 0x53, 0x6B, 0x69, 0x70, 0x70, 0x65, 0x64, 0x3A, 0x00,       // Arg 1 = Skipped:
                0x21, 0x00, 0x00, 0x00, 0x64,                                                                   // Arg 2 = 100
                0x00, 0x82, 0x00, 0x00, 0x0F, 0x00, 0x62, 0x79, 0x74, 0x65, 0x73, 0x3B, 0x20, 0x52, 0x65, 0x61,
                0x73, 0x6F, 0x6E, 0x3A, 0x00,                                                                   // Arg 3 = bytes; Reason:
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Arg 4 = Test
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineMinimal()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x21, 0x32, 0x00, 0x19,                                                                         // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineTimeStamp()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x21, 0x32, 0x00, 0x19,                                                                         // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineTimeStamps()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                DeviceTimeStamp = new TimeSpan(0, 0, 0, 55, 80),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.DevTimeStampFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x31, 0x32, 0x00, 0x1D, 0x00, 0x08, 0x67, 0x90,                                                 // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineEcuId()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                EcuId = "ECU1",
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.EcuIdFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x45, 0x43, 0x55, 0x31, // Storage Header
                0x25, 0x32, 0x00, 0x1D, 0x45, 0x43, 0x55, 0x31,                                                 // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineEmptyEcuId()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                EcuId = "",
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.EcuIdFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x25, 0x32, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x00,                                                 // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineSessionId()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                SessionId = unchecked((int)0xDEADBEEF),
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.SessionIdFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x29, 0x32, 0x00, 0x1D, 0xDE, 0xAD, 0xBE, 0xEF,                                                 // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteLineEcuIdAppIdCtxId()
        {
            DltTraceLine line = new(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.EcuIdFeature +
                           DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x45, 0x43, 0x55, 0x31, // Storage Header
                0x25, 0x32, 0x00, 0x1D, 0x45, 0x43, 0x55, 0x31,                                                 // Standard Header
                0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void DltConvertNonVerboseToVerbose()
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetIsVerbose(false)
                .SetMessageId(10)
                .SetCount(126)
                .SetDeviceTimeStamp(TimeSpan.FromSeconds(1.45).Ticks)
                .SetDltType(DltType.LOG_INFO)
                .SetTimeStamp(new DateTime(2023, 6, 4, 15, 27, 22, 450, DateTimeKind.Utc));
            builder.AddArgument(new StringDltArg("NonVerbose Line"));
            builder.AddArgument(new SignedIntDltArg(65535));
            DltTraceLineBase line = builder.GetResult();

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x5A, 0xAD, 0x7C, 0x64, 0xD0, 0xDD, 0x06, 0x00, 0x45, 0x43, 0x55, 0x31, // Storage Header
                0x35, 0x7E, 0x00, 0x34, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x38, 0xA4,                         // Standard Header
                0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x4E, 0x6F, 0x6E, 0x56, 0x65, 0x72, 0x62, 0x6F, 0x73, 0x65, // Arg 1
                0x20, 0x4C, 0x69, 0x6E, 0x65, 0x00,
                0x23, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00                                                  // Arg 2 = 65535
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt") {
                ConvertNonVerbose = true
            }) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line, FileData), Is.True);
                output.Flush();

                // Even though we provided the original data as 'FileData`, it is overwritten as if no packet was
                // provided.
                Assert.That(GetFile("out.dlt"), Is.EqualTo(expected));
            }
        }

        [Test]
        public void DltConvertNonVerboseUnknownArg()
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetIsVerbose(false)
                .SetMessageId(10)
                .SetCount(126)
                .SetDeviceTimeStamp(TimeSpan.FromSeconds(1.45).Ticks)
                .SetDltType(DltType.LOG_INFO)
                .SetTimeStamp(new DateTime(2023, 6, 4, 15, 27, 22, 450, DateTimeKind.Utc));
            builder.AddArgument(new StringDltArg("NonVerbose Line"));
            builder.AddArgument(new UnknownNonVerboseDltArg(new byte[] { 0x00, 0x00 }));
            DltTraceLineBase line = builder.GetResult();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt") {
                ConvertNonVerbose = true
            }) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line, FileData), Is.True);
                output.Flush();

                // The original data is returned because of 'UnknownNonVerboseDltArg'.
                Assert.That(GetFile("out.dlt"), Is.EqualTo(FileData));
            }
        }

        [Test]
        public void DltConvertNonVerboseNvArg()
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetIsVerbose(false)
                .SetMessageId(10)
                .SetCount(126)
                .SetDeviceTimeStamp(TimeSpan.FromSeconds(1.45).Ticks)
                .SetDltType(DltType.LOG_INFO)
                .SetTimeStamp(new DateTime(2023, 6, 4, 15, 27, 22, 450, DateTimeKind.Utc));
            builder.AddArgument(new NonVerboseDltArg(new byte[] { 0x00, 0x00 }));
            DltTraceLineBase line = builder.GetResult();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt") {
                ConvertNonVerbose = true
            }) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line, FileData), Is.True);
                output.Flush();

                // The original data is returned because of 'NonVerboseDltArg'.
                Assert.That(GetFile("out.dlt"), Is.EqualTo(FileData));
            }
        }

        [Test]
        public void DltConvertVerbose()
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetIsVerbose(true)
                .SetCount(126)
                .SetDeviceTimeStamp(TimeSpan.FromSeconds(1.45).Ticks)
                .SetDltType(DltType.LOG_INFO)
                .SetTimeStamp(new DateTime(2023, 6, 4, 15, 27, 22, 450, DateTimeKind.Utc));
            builder.AddArgument(new StringDltArg("NonVerbose Line"));
            DltTraceLineBase line = builder.GetResult();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt") {
                ConvertNonVerbose = true
            }) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line, FileData), Is.True);
                output.Flush();

                // The original data is returned because it is already verbose.
                Assert.That(GetFile("out.dlt"), Is.EqualTo(FileData));
            }
        }

        [Test]
        public void DltConvertControl()
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetIsVerbose(false)
                .SetCount(126)
                .SetDeviceTimeStamp(TimeSpan.FromSeconds(1.45).Ticks)
                .SetDltType(DltType.CONTROL_RESPONSE)
                .SetTimeStamp(new DateTime(2023, 6, 4, 15, 27, 22, 450, DateTimeKind.Utc));
            builder.SetControlPayload(new GetSoftwareVersionResponse(ControlResponse.StatusOk, "Version 1"));
            DltTraceLineBase line = builder.GetResult();

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new("out.dlt") {
                ConvertNonVerbose = true
            }) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line, FileData), Is.True);
                output.Flush();

                // The original data is returned because it is a control message.
                Assert.That(GetFile("out.dlt"), Is.EqualTo(FileData));
            }
        }
    }
}
