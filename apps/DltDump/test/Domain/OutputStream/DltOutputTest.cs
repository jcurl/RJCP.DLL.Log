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
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.False);
            }
        }

        [Test]
        public void DefaultPropertiesWithForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt", true)) {
                Assert.That(File.Exists("File.dlt"), Is.False);
                Assert.That(output.SupportsBinary, Is.True);
                Assert.That(output.Force, Is.True);
            }
        }

        [Test]
        public void WriteLineAsPacketFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, FileData.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length));
            }
        }

        [Test]
        public void WriteLinesAsPacketFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.dlt", InputFormat.File);
                output.Write(TestLines.Verbose, FileData.AsSpan());
                output.Write(TestLines.Verbose2, FileData.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. There is no interpretation of the data.
                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(FileData.Length * 2));
            }
        }

        [Test]
        public void WriteLineAsPacketTcp()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                output.SetInput("input.raw", InputFormat.Network);
                output.Write(TestLines.Verbose, TcpData.AsSpan());
                output.Flush();

                Assert.That(File.Exists("File.dlt"), Is.True);

                // The data is written exactly as the packet says. A storage header is added.
                FileInfo fileInfo = new FileInfo("File.dlt");
                Assert.That(fileInfo.Length, Is.EqualTo(TcpData.Length + 16));
            }
        }

        [Test]
        public void SetInputNullString()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
                Assert.That(File.Exists("File.dlt"), Is.False);

                Assert.That(() => {
                    output.SetInput(null, InputFormat.File);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void SetInputUnknownFormat()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("File.dlt")) {
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
            using (DltOutput output = new DltOutput("File.dlt")) {
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
            using (DltOutput output = new DltOutput("File.dlt")) {
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
                new DltControlTraceLine(new GetSoftwareVersionResponse(ControlResponse.StatusOk, "Version"));

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.False);
                output.Flush();

                // File doesn't exist, because nothing was written.
                Assert.That(File.Exists("out.dlt"), Is.False);
            }
        }

        [Test]
        public void WriteSkippedLine()
        {
            DltSkippedTraceLine line = new DltSkippedTraceLine(100, "Test") {
                EcuId = "ECU1",
                Count = 50,
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                DeviceTimeStamp = new TimeSpan(0, 0, 0, 55, 80),
            };
            line.Features += DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature;

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x45, 0x43, 0x55, 0x31, // Storage Header
                0x35, 0x32, 0x00, 0x3D, 0x45, 0x43, 0x55, 0x31, 0x00, 0x08, 0x67, 0x90,                         // Standard Header
                0x31, 0x01, 0x53, 0x4B, 0x49, 0x50, 0x53, 0x4B, 0x49, 0x50,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x21, 0x00, 0x53, 0x6B, 0x69, 0x70, 0x70, 0x65, 0x64, 0x3A, 0x20, 0x31, // Verbose Payload
                0x30, 0x30, 0x20, 0x62, 0x79, 0x74, 0x65, 0x73, 0x3B, 0x20, 0x52, 0x65, 0x61, 0x73, 0x6F, 0x6E,
                0x3A, 0x20, 0x54, 0x65, 0x73, 0x74, 0x00
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineMinimal()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineTimeStamp()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineTimeStamps()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineEcuId()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineEmptyEcuId()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineSessionId()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
                Count = 50,
                Type = DltType.LOG_INFO,
                SessionId = unchecked((int)0xDEADBEEF),
                TimeStamp = new DateTime(2022, 4, 16, 18, 47, 23, 387, DateTimeKind.Utc),
                Features = DltLineFeatures.LogTimeStampFeature + DltLineFeatures.SessionIdFeature
            };

            byte[] expected = {
                0x44, 0x4C, 0x54, 0x01, 0x3B, 0x0F, 0x5B, 0x62, 0xB8, 0xE7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, // Storage Header
                0x29, 0x32, 0x00, 0x1D, 0xEF, 0xBE, 0xAD, 0xDE,                                                 // Standard Header
                0x41, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                                     // Ext. Header
                0x00, 0x82, 0x00, 0x00, 0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x00                                // Verbose Payload
            };

            using (ScratchPad pad = Deploy.ScratchPad())
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void WriteLineEcuIdAppIdCtxId()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("Test") }) {
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
            using (DltOutput output = new DltOutput("out.dlt")) {
                Assert.That(File.Exists("out.dlt"), Is.False);
                output.SetInput("input.dlt", InputFormat.File);
                Assert.That(output.Write(line), Is.True);
                output.Flush();

                using (MemoryStream mem = new MemoryStream())
                using (FileStream file = new FileStream("out.dlt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    file.CopyTo(mem);
                    Assert.That(mem.ToArray(), Is.EqualTo(expected));
                }
            }
        }
    }
}
