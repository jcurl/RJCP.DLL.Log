namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.OutputStream;
    using Infrastructure;
    using NUnit.Framework;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class EnhancedPacketBlockTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockMicroSec(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockNanoSec(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthNanoSec : PcapBlocks.IdbEthNanoSecBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataNano : PcapBlocks.EpbDataNanoBigEndian, 0).ToList();

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622498)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockOutputStream(bool littleEndian)
        {
            using (MemoryOutput output = new MemoryOutput())
            using (BlockReader reader = new BlockReader(output)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines.Count, Is.EqualTo(0));

                var memLines = (from line in output.Lines select line.Line).ToList();
                Assert.That(memLines.Count, Is.EqualTo(1));
                Assert.That(memLines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(memLines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(memLines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(memLines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(memLines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockFirstInterfaceMicro(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthMicroSec : PcapBlocks.IdbEthMicroSecBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthNanoSec : PcapBlocks.IdbEthNanoSecBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockSecondInterfaceNano(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthMicroSec : PcapBlocks.IdbEthMicroSecBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthNanoSec : PcapBlocks.IdbEthNanoSecBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataNanoIntf1 : PcapBlocks.EpbDataNanoIntf1BigEndian, 0).ToList();

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622498)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockUnknownLinkType(bool littleEndian)
        {
            // The link type is 0x02.
            byte[] idb = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x14, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x14
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(idb, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines0 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines0.Count, Is.EqualTo(0));

                var lines1 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataIntf1 : PcapBlocks.EpbDataIntf1BigEndian, 0).ToList();
                Assert.That(lines1.Count, Is.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockUnknownSnapLen(bool littleEndian)
        {
            byte[] idb = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                    0x14, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                    0x00, 0x00, 0x00, 0x14
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(idb, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines0 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines0.Count, Is.EqualTo(0));

                var lines1 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataIntf1 : PcapBlocks.EpbDataIntf1BigEndian, 0).ToList();
                Assert.That(lines1.Count, Is.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockZeroSnapLen(bool littleEndian)
        {
            byte[] idb = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x14, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x14
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(idb, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines0 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines0.Count, Is.EqualTo(0));

                var lines1 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataIntf1 : PcapBlocks.EpbDataIntf1BigEndian, 0).ToList();
                Assert.That(lines1.Count, Is.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockUndefinedInterface(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines0 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines0.Count, Is.EqualTo(1));

                var lines1 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataIntf1 : PcapBlocks.EpbDataIntf1BigEndian, 0).ToList();
                Assert.That(lines1.Count, Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockNegativeInterface(bool littleEndian)
        {
            byte[] epb = (littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian).ToArray();
            BitOperations.Copy32Shift(-1, epb, 8, littleEndian);  // Offset 8 is Interface identifier.

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines = reader.DecodeBlock(epb, 0).ToList();
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeBlockInvalidInterface(bool littleEndian)
        {
            // The link type is too short, so not a valid block. But it should still be treated as interface 0.
            byte[] idb = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(idb, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                var lines0 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0).ToList();
                Assert.That(lines0.Count, Is.EqualTo(0));

                var lines1 = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataIntf1 : PcapBlocks.EpbDataIntf1BigEndian, 0).ToList();
                Assert.That(lines1.Count, Is.EqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeWithMultipleShb(bool littleEndian)
        {
            List<DltTraceLineBase> lines;

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 192);
                lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 336).ToList();
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));

                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 472);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthNanoSec : PcapBlocks.IdbEthNanoSecBigEndian, 664);
                lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbDataNano : PcapBlocks.EpbDataNanoBigEndian, 808).ToList();
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622498)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BufferTooSmall(bool littleEndian)
        {
            // The packet is less than 32 bytes, but is otherwise valid.
            byte[] data = littleEndian ?
                new byte[] {
                    0x06, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, 0xA8, 0x05, 0x00,
                    0xFE, 0x05, 0x68, 0xEC, 0x18, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xA8, 0x46,
                    0xEC, 0x68, 0x05, 0xFE, 0x00, 0x00, 0x00, 0x18
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 192);
                var lines = reader.DecodeBlock(data, 336);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BufferTooSmallForCaptureSize(bool littleEndian)
        {
            // The packet is less than 32 bytes, but is otherwise valid.
            byte[] data = littleEndian ?
                new byte[] {
                    0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, 0xA8, 0x05, 0x00,
                    0xFE, 0x05, 0x68, 0xEC, 0x65, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                    0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,

                    0x80, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xA8, 0x46,
                    0xEC, 0x68, 0x05, 0xFE, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x65,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                    0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,

                    0x00, 0x00, 0x00, 0x80
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 192);
                var lines = reader.DecodeBlock(data, 336);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BufferTooSmallForPacketSize(bool littleEndian)
        {
            // The packet is less than 32 bytes, but is otherwise valid.
            byte[] data = littleEndian ?
                new byte[] {
                    0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, 0xA8, 0x05, 0x00,
                    0xFE, 0x05, 0x68, 0xEC, 0x65, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,

                    0x80, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xA8, 0x46,
                    0xEC, 0x68, 0x05, 0xFE, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x65,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,

                    0x00, 0x00, 0x00, 0x80
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 192);
                var lines = reader.DecodeBlock(data, 336);
                Assert.That(lines, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeMissingShb(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                var lines = reader.DecodeBlock(littleEndian ? PcapBlocks.EpbData : PcapBlocks.EpbDataBigEndian, 0);

                Assert.That(lines, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InvalidEbpPacketLengthTooSmallEnoughBuffer(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x06, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00 };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                var lines = reader.DecodeBlock(data, 0);

                Assert.That(lines, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InvalidEbpPacketLengthTooSmall(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x06, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x08 };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                var lines = reader.DecodeBlock(data, 0);

                Assert.That(lines, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BufferLengthMismatch(bool littleEndian)
        {
            // The packet is less than 32 bytes, but is otherwise valid.
            byte[] data = littleEndian ?
                new byte[] {
                    0x06, 0x00, 0x00, 0x00, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, 0xA8, 0x05, 0x00,
                    0xFE, 0x05, 0x68, 0xEC, 0x65, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                    0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,
                    0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x00, 0x00, 0x00,

                    0x80, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xA8, 0x46,
                    0xEC, 0x68, 0x05, 0xFE, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x65,

                    // Packet
                    0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                    0x00, 0x57, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                    0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x43, 0xB5, 0xCF, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                    0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                    0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,
                    0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x00, 0x00, 0x00,

                    0x00, 0x00, 0x00, 0x80
                };

            using (BlockReader reader = new BlockReader()) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                _ = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 192);
                var lines = reader.DecodeBlock(data, 336);
                Assert.That(lines, Is.Null);
            }
        }
    }
}
