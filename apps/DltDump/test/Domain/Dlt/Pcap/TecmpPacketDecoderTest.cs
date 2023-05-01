namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class TecmpPacketDecoderTest
    {
        private static readonly ITraceDecoderFactory<DltTraceLineBase> DefaultPcapFactory = new PcapTraceDecoderFactory();

        private static readonly DateTime Time1 = new DateTime(2022, 4, 24, 17, 2, 54, DateTimeKind.Utc).AddMilliseconds(580);

        #region Packet Building for common testing
        [Flags]
        public enum CmFlags
        {
            InSegmentedMessage = 0x0000,
            EndSegmentedMessage = 0x0001,
            StartSegmentedMessage = 0x0002,
            UnsegmentedMessage = 0x0003,

            Spy = 0x0004,
            Multiframe = 0x0008,
            Overflow = 0x8000,

            Receive = Spy | Multiframe
        }

        [Flags]
        private enum DataFlags
        {
            None = 0,
            CrcError = 0x2000,
            Tx = 0x4000,
            Overflow = 0x8000
        }

        private static (byte hi, byte lo) Split16(int value)
        {
            byte lo = (byte)(value & 0xFF);
            byte hi = (byte)((value >> 8) & 0xFF);

            return (hi, lo);
        }

        private static void AddEthMac(List<byte> packet)
        {
            packet.AddRange(new byte[] { 0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E });   // DstMac, SrcMac
        }

        private static void AddVlan(List<byte> packet, byte vlan)
        {
            packet.AddRange(new byte[] { 0x81, 0x00, 0x00, vlan });
        }

        private static void AddTecmpHeader(List<byte> packet)
        {
            AddTecmpHeader(packet, CmFlags.Receive | CmFlags.UnsegmentedMessage, 3, 3, 0x80);
        }

        private static void AddTecmpHeader(List<byte> packet, CmFlags cmFlags, byte version, byte logstream, int datastream)
        {
            (byte cmhi, byte cmlo) = Split16((int)cmFlags);
            (byte dshi, byte dslo) = Split16(datastream);
            packet.AddRange(new byte[] {
                0x99, 0xFE,                                                             // TECMP protocol
                0x00, 0x80, 0x72, 0x14, version, logstream, dshi, dslo, 0x00, 0x00, cmhi, cmlo, // TECMP header
            });
        }

        private static void AddTecmpPayload(List<byte> packet, List<byte> payload)
        {
            AddTecmpPayload(packet, payload, DataFlags.None);
        }

        private static void AddTecmpPayload(List<byte> packet, List<byte> payload, DataFlags flags)
        {
            (byte lhi, byte llo) = Split16(payload.Count);
            (byte dfhi, byte dflo) = Split16((int)flags);

            packet.AddRange(new byte[] {
                0x00, 0x00, 0x01, 0x2a, 0x00, 0x00, 0x2e, 0x68, 0xcf, 0xfe, 0x5b, 0x60, // TECMP payload
                lhi, llo, dfhi, dflo,                                                 //   length, data flags
            });
            packet.AddRange(payload);
        }

        private static void AddUdpPayload(List<byte> packet, List<byte> udpPayload)
        {
            (byte ihi, byte ilo) = Split16(28 + udpPayload.Count);
            (byte uhi, byte ulo) = Split16(8 + udpPayload.Count);

            packet.AddRange(new byte[] {
                0x08, 0x00,                                                             // IPv4 Proto
                0x45, 0x00, ihi,  ilo,  0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x65, // IPv4 Header (CS is ignored)
                0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF, 0x2A, 0x63,
                0x0D, 0xA2, 0x0D, 0xA2, uhi,  ulo,  0xB5, 0xCF,                         // UDP Header (CS is ignored)
            });
            packet.AddRange(udpPayload);
        }

        private static void AddDltPayload(List<byte> packet, string message)
        {
            byte[] encMessage = Encoding.UTF8.GetBytes(message);
            (byte dlthi, byte dltlo) = Split16(encMessage.Length + 33);
            (byte shi, byte slo) = Split16(encMessage.Length + 1);

            List<byte> dltPayload = new List<byte>(new byte[] {
                0x3D, 0x0B, dlthi, dltlo, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, // DLT
                0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
                0x58, 0x31, 0x00, 0x02, 0x00, 0x00, slo, shi
            });
            dltPayload.AddRange(encMessage);
            dltPayload.Add(0x00);

            AddUdpPayload(packet, dltPayload);
        }
        #endregion

        [TestCase(0, 0, 0, 0, TestName = "DecodeTecmpPacket")]
        [TestCase(0x45, 0, 0, 0, TestName = "DecodeVlanTecmpPacket")]
        [TestCase(0x45, 0x01, 0, 0, TestName = "DecodeVlan2TecmpPacket")]
        [TestCase(0, 0, 0x49, 0, TestName = "DecodeTecmpPacketVlan")]
        [TestCase(0, 0, 0x49, 0x01, TestName = "DecodeTecmpPacketVlan2")]
        [TestCase(0x45, 0, 0x49, 0, TestName = "DecodeVlanTecmpPacketVlan")]
        [TestCase(0x45, 0, 0x49, 0x01, TestName = "DecodeVlanTecmpPacketVlan2")]
        [TestCase(0x45, 0x01, 0x49, 0, TestName = "DecodeVlan2TecmpPacketVlan")]
        [TestCase(0x45, 0x01, 0x49, 0x01, TestName = "DecodeVlan2TecmpPacketVlan2")]
        public void DecodeTecmpPacket(byte ethVlanOuter, byte ethVlanInner, byte captureVlanOuter, byte captureVlanInner)
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            if (ethVlanOuter != 0) AddVlan(packet, ethVlanOuter);
            if (ethVlanInner != 0) AddVlan(packet, ethVlanInner);
            AddTecmpHeader(packet);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            if (captureVlanOuter != 0) AddVlan(payload, captureVlanOuter);
            if (captureVlanInner != 0) AddVlan(payload, captureVlanInner);
            AddDltPayload(payload, "DLT Argument test string..");

            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(Time1));
            }
        }

        [TestCase(0, 0, 0, 0, TestName = "DecodeTecmpPacketMulti")]
        [TestCase(0x45, 0, 0, 0, TestName = "DecodeVlanTecmpPacketMulti")]
        [TestCase(0x45, 0x01, 0, 0, TestName = "DecodeVlan2TecmpPacketMulti")]
        [TestCase(0, 0, 0x49, 0, TestName = "DecodeTecmpPacketVlanMulti")]
        [TestCase(0, 0, 0x49, 0x01, TestName = "DecodeTecmpPacketVlan2Multi")]
        [TestCase(0x45, 0, 0x49, 0, TestName = "DecodeVlanTecmpPacketVlanMulti")]
        [TestCase(0x45, 0, 0x49, 0x01, TestName = "DecodeVlanTecmpPacketVlan2Multi")]
        [TestCase(0x45, 0x01, 0x49, 0, TestName = "DecodeVlan2TecmpPacketVlanMulti")]
        [TestCase(0x45, 0x01, 0x49, 0x01, TestName = "DecodeVlan2TecmpPacketVlan2Multi")]
        public void DecodeTecmpPacketMulti(byte ethVlanOuter, byte ethVlanInner, byte captureVlanOuter, byte captureVlanInner)
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            if (ethVlanOuter != 0) AddVlan(packet, ethVlanOuter);
            if (ethVlanInner != 0) AddVlan(packet, ethVlanInner);
            AddTecmpHeader(packet);

            List<byte> payload1 = new List<byte>();
            AddEthMac(payload1);
            if (captureVlanOuter != 0) AddVlan(payload1, captureVlanOuter);
            if (captureVlanInner != 0) AddVlan(payload1, captureVlanInner);
            AddDltPayload(payload1, "DLT Argument test string..");
            AddTecmpPayload(packet, payload1);

            List<byte> payload2 = new List<byte>();
            AddEthMac(payload2);
            if (captureVlanOuter != 0) AddVlan(payload2, captureVlanOuter);
            if (captureVlanInner != 0) AddVlan(payload2, captureVlanInner);
            AddDltPayload(payload2, "DLT Argument test string 2");
            AddTecmpPayload(packet, payload2);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(2));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(Time1));
                Assert.That(lines[1].Text, Is.EqualTo("DLT Argument test string 2"));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(Time1));
            }
        }

        [Test]
        public void DecodeTecmpPacketMulti2()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet);

            List<byte> payload1 = new List<byte>();
            AddEthMac(payload1);
            AddDltPayload(payload1, "DLT Argument test string..");
            AddTecmpPayload(packet, payload1);

            List<byte> payload2 = new List<byte>();
            AddEthMac(payload2);
            AddDltPayload(payload2, "DLT Argument test string 2");
            AddTecmpPayload(packet, payload2);

            List<byte> payload3 = new List<byte>();
            AddEthMac(payload3);
            AddDltPayload(payload3, "DLT Argument test string 3");
            AddTecmpPayload(packet, payload3);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(3));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(Time1));
                Assert.That(lines[1].Text, Is.EqualTo("DLT Argument test string 2"));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(Time1));
                Assert.That(lines[2].Text, Is.EqualTo("DLT Argument test string 3"));
                Assert.That(lines[2].TimeStamp, Is.EqualTo(Time1));
            }
        }

        [TestCase(2, 3, 0x80, TestName = "TecmpUnknownVersion")]
        [TestCase(3, 2, 0x80, TestName = "TecmpUnknownLogStream")]
        [TestCase(3, 3, 0x7F, TestName = "TecmpUnknownDataStream")]
        public void TecmpUnknown(byte version, byte logStream, int dataStream)
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet, CmFlags.UnsegmentedMessage | CmFlags.Receive, version, logStream, dataStream);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            AddDltPayload(payload, "DLT Argument test string..");
            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void UnknownProto()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            packet.AddRange(new byte[] {
                0x99, 0xFF,                                                             // TECMP protocol
                0x00, 0x80, 0x72, 0x14, 0x03, 0x03, 0x00, 0x80, 0x00, 0x00, 0x00, 0x0F, // TECMP header
            });

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            AddDltPayload(payload, "DLT Argument test string..");
            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [TestCase(CmFlags.StartSegmentedMessage, TestName = "TecmpUnknownStartSegment")]
        [TestCase(CmFlags.InSegmentedMessage, TestName = "TecmpUnknownInSegment")]
        [TestCase(CmFlags.EndSegmentedMessage, TestName = "TecmpUnknownEndSegment")]
        public void TecmpUnknownCmFlags(CmFlags flags)
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet, CmFlags.Receive | flags, 0x03, 0x03, 0x80);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            AddDltPayload(payload, "DLT Argument test string..");
            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void TecmpCmFlagOverflow()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet, CmFlags.Receive | CmFlags.UnsegmentedMessage | CmFlags.Overflow, 0x03, 0x03, 0x80);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            AddDltPayload(payload, "DLT Argument test string..");
            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(Time1));
            }
        }

        [Test]
        public void EmptyPayload()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet);
            AddTecmpPayload(packet, new List<byte>());

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void IncompleteDltPayload()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            AddUdpPayload(payload, new List<byte>(new byte[] {
                0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, // DLT
                0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
                //0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20,
                //0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73,
                //0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x2E, 0x2E, 0x00
            }));
            AddTecmpPayload(packet, payload);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void PayloadOverTwoPackets()
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            AddTecmpHeader(packet);

            List<byte> payload1 = new List<byte>();
            AddEthMac(payload1);
            AddUdpPayload(payload1, new List<byte>(new byte[] {
                0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, // DLT
                0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
            }));
            AddTecmpPayload(packet, payload1);

            List<byte> payload2 = new List<byte>();
            AddEthMac(payload2);
            AddUdpPayload(payload2, new List<byte>(new byte[] {
                0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20,
                0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73,
                0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x2E, 0x2E, 0x00
            }));
            AddTecmpPayload(packet, payload2);

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(Time1));
            }
        }

        [TestCase(0, 0, 0, 0, TestName = "DecodeIncompleteTecmpPacket")]
        [TestCase(0x45, 0, 0, 0, TestName = "DecodeIncompleteVlanTecmpPacket")]
        [TestCase(0x45, 0x01, 0, 0, TestName = "DecodeIncompleteVlan2TecmpPacket")]
        [TestCase(0, 0, 0x49, 0, TestName = "DecodeIncompleteTecmpPacketVlan")]
        [TestCase(0, 0, 0x49, 0x01, TestName = "DecodeIncompleteTecmpPacketVlan2")]
        [TestCase(0x45, 0, 0x49, 0, TestName = "DecodeIncompleteVlanTecmpPacketVlan")]
        [TestCase(0x45, 0, 0x49, 0x01, TestName = "DecodeIncompleteVlanTecmpPacketVlan2")]
        [TestCase(0x45, 0x01, 0x49, 0, TestName = "DecodeIncompleteVlan2TecmpPacketVlan")]
        [TestCase(0x45, 0x01, 0x49, 0x01, TestName = "DecodeIncompleteVlan2TecmpPacketVlan2")]
        public void DecodeIncompleteTecmpPacket(byte ethVlanOuter, byte ethVlanInner, byte captureVlanOuter, byte captureVlanInner)
        {
            List<byte> packet = new List<byte>();
            AddEthMac(packet);
            if (ethVlanOuter != 0) AddVlan(packet, ethVlanOuter);
            if (ethVlanInner != 0) AddVlan(packet, ethVlanInner);
            AddTecmpHeader(packet);

            List<byte> payload = new List<byte>();
            AddEthMac(payload);
            if (captureVlanOuter != 0) AddVlan(payload, captureVlanOuter);
            if (captureVlanInner != 0) AddVlan(payload, captureVlanInner);
            AddDltPayload(payload, "DLT Argument test string..");

            AddTecmpPayload(packet, payload);

            byte[] bytePacket = packet.ToArray();
            for (int i = 0; i < bytePacket.Length; i++) {
                Console.WriteLine($"Packet Size = {i} of {bytePacket.Length}");
                using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                    IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                        packetDecoder.DecodePacket(bytePacket.AsSpan(0, i), Time1, 20));
                    Assert.That(lines.Count, Is.EqualTo(0));
                }
            }
        }

        private static readonly int[] CorruptLengths = {
            0, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
            108, 110, 65535
        };

        [TestCaseSource(nameof(CorruptLengths))]
        public void CorruptLength(int length)
        {
            (byte lhi, byte llo) = Split16(length);

            // Nominal length is 0x6d = 109
            List<byte> packet = new List<byte>(new byte[] {
                0x10, 0xdf, 0x23, 0x41, 0xe4, 0xc2, 0x74, 0xe7, 0xb1, 0x14, 0x44, 0x5e,  // [00..0B] PCAP MAC Dst/Src
                0x81, 0x00, 0x00, 0x45, 0x81, 0x00, 0x00, 0x01,                          // [0C..13] VLAN Inner / Outer
                0x99, 0xfe,                                                              // [14..15] Proto TECMP
                0x00, 0x80, 0x72, 0x14, 0x03, 0x03, 0x00, 0x80, 0x00, 0x00, 0x00, 0x0f,  // [16..21] TECMP Header
                0x00, 0x00, 0x01, 0x2a, 0x00, 0x00, 0x2e, 0x68, 0xcf, 0xfe, 0x5b, 0x60,  // [22..2D] TECMP Payload
                lhi,  llo,  0x00, 0x00,                                                  // [2E..31]

                0x10, 0xdf, 0x23, 0x41, 0xe4, 0xc2, 0x74, 0xe7, 0xb1, 0x14, 0x44, 0x5e,  // [32..3D] Captured MAC Dst/Src
                0x81, 0x00, 0x00, 0x49, 0x81, 0x00, 0x00, 0x01,                          // [3E..45] Captured VLAN Inner / Outer
                0x08, 0x00,                                                              // [46..47] Proto IPv4
                0x45, 0x00, 0x00, 0x57, 0x3a, 0x25, 0x00, 0x00, 0x01, 0x11, 0xa3, 0x65,  // [48..53] IPv4 Header
                0xc0, 0xa8, 0x01, 0x01, 0xef, 0xff, 0x2a, 0x63,                          // [54..5B]
                0x0d, 0xa2, 0x0d, 0xa2, 0x00, 0x43, 0xb5, 0xcf,                          // [5C..63] UDP Header
                0x3d, 0x0b, 0x00, 0x3b, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, 0x8e,  // [64..6F] DLT Payload
                0x00, 0x01, 0x54, 0x4a, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,  // [70..7B]
                0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1b, 0x00, 0x44, 0x4c, 0x54, 0x20,  // [7C..87]
                0x41, 0x72, 0x67, 0x75, 0x6d, 0x65, 0x6e, 0x74, 0x20, 0x74, 0x65, 0x73,  // [88..93]
                0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67, 0x2e, 0x2e, 0x00         // [94..9E]
            });

            using (PacketDecoder packetDecoder = new PacketDecoder(LinkTypes.LINKTYPE_ETHERNET, DefaultPcapFactory)) {
                IList<DltTraceLineBase> lines = new List<DltTraceLineBase>(
                    packetDecoder.DecodePacket(packet.ToArray(), Time1, 20));
                Assert.That(lines.Count, Is.EqualTo(0));
            }
        }
    }
}
