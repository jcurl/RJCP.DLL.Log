namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Dlt.Pcap.Ng.Options;
    using Domain.OutputStream;
    using Infrastructure;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class InterfaceDescriptionBlockTest
    {
        private static readonly ITraceDecoderFactory<DltTraceLineBase> DefaultPcapFactory = new PcapTraceDecoderFactory();

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlock(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(144));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(1));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(4));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbName));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(50));
                Assert.That(idbBlock.Options[0], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)idbBlock.Options[0]).Value, Is.EqualTo("\\Device\\NPF_{28FE7AE2-5884-4182-92CA-531A6F377876}"));
                Assert.That(idbBlock.Options[1].OptionCode, Is.EqualTo(OptionCodes.IdbDescription));
                Assert.That(idbBlock.Options[1].Length, Is.EqualTo(8));
                Assert.That(idbBlock.Options[1], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)idbBlock.Options[1]).Value, Is.EqualTo("Ethernet"));
                Assert.That(idbBlock.Options[2].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[2].Length, Is.EqualTo(1));
                Assert.That(idbBlock.Options[2], Is.TypeOf<TimeResolutionOption>());
                Assert.That(((TimeResolutionOption)idbBlock.Options[2]).Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
                Assert.That(((TimeResolutionOption)idbBlock.Options[2]).Value, Is.EqualTo(6));
                Assert.That(idbBlock.Options[3].OptionCode, Is.EqualTo(OptionCodes.IdbOs));
                Assert.That(idbBlock.Options[3].Length, Is.EqualTo(37));
                Assert.That(idbBlock.Options[3], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)idbBlock.Options[3]).Value, Is.EqualTo("64-bit Windows 10 (2009), build 19043"));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockSmallEth(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(20));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));
                Assert.That(idbBlock.Options, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockSmallSll(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkSll : PcapBlocks.IdbSmallLinkSllBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(20));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_LINUX_SLL));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));
                Assert.That(idbBlock.Options, Is.Empty);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockMicroSecDefault(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(20));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));
                Assert.That(idbBlock.Options, Is.Empty);

                Assert.That(idbBlock.GetTimeStamp(0x0005A846, 0xEC6805FE),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));

                Assert.That(idbBlock.GetTimeStamp(0xFFFFFFFF, 0xFFFFFFFF),
                    Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999900)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockMicroSec(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthMicroSec : PcapBlocks.IdbEthMicroSecBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(1));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(1));
                Assert.That(idbBlock.Options[0], Is.TypeOf<TimeResolutionOption>());
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Value, Is.EqualTo(6));

                Assert.That(idbBlock.GetTimeStamp(0x0005A846, 0xEC6805FE),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));

                Assert.That(idbBlock.GetTimeStamp(0xFFFFFFFF, 0xFFFFFFFF),
                    Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999900)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockMicroSecNoEndOfOpt(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthMicroSecNoEOpt : PcapBlocks.IdbEthMicroSecNoEOptBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(28));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(1));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(1));
                Assert.That(idbBlock.Options[0], Is.TypeOf<TimeResolutionOption>());
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Value, Is.EqualTo(6));

                Assert.That(idbBlock.GetTimeStamp(0x0005A846, 0xEC6805FE),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));

                Assert.That(idbBlock.GetTimeStamp(0xFFFFFFFF, 0xFFFFFFFF),
                    Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999900)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockNanoSec(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbEthNanoSec : PcapBlocks.IdbEthNanoSecBigEndian, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(1));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(1));
                Assert.That(idbBlock.Options[0], Is.TypeOf<TimeResolutionOption>());
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Value, Is.EqualTo(9));

                Assert.That(idbBlock.GetTimeStamp(0x1619550B, 0x76576895),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622100)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockFixedPt10bit(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x8A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x8A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(1));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(1));
                Assert.That(idbBlock.Options[0], Is.TypeOf<TimeResolutionOption>());
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
                Assert.That(((TimeResolutionOption)idbBlock.Options[0]).Value, Is.EqualTo(10));

                Assert.That(idbBlock.GetTimeStamp(0x0000017B, 0xA83A287D),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(122070312)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InvalidTimeResolution(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x02, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x02, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));

                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                Assert.That(idbBlock.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                Assert.That(idbBlock.SnapLength, Is.EqualTo(0x40000));

                Assert.That(idbBlock.Options, Has.Count.EqualTo(1));
                Assert.That(idbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                Assert.That(idbBlock.Options[0].Length, Is.EqualTo(2));
                Assert.That(idbBlock.Options[0], Is.Not.InstanceOf<TimeResolutionOption>());

                Assert.That(idbBlock.GetTimeStamp(0x0005A846, 0xEC6805FE),
                    Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));

                Assert.That(idbBlock.GetTimeStamp(0xFFFFFFFF, 0xFFFFFFFF),
                    Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999900)));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void UnknownLinkType(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.Not.InstanceOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TruncatedIdb(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IdbTooSmall(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.Not.InstanceOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(16));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OptionsCorrupted(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x09, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x09, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.Not.InstanceOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(32));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NegativeSnapLength(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.Not.InstanceOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ZeroSnapLength(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);

                Assert.That(block, Is.Not.InstanceOf<InterfaceDescriptionBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.InterfaceDescriptionBlock));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockNoShb(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbData : PcapBlocks.IdbDataBigEndian, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LengthMismatch(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LengthUnknown(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                    0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                    0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodePacket(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);
                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());

                ReadOnlySpan<byte> packet = littleEndian ?
                    PcapBlocks.EpbData.AsSpan(28..^4) :
                    PcapBlocks.EpbDataBigEndian.AsSpan(28..^4);

                DateTime timeStamp = new DateTime(2020, 6, 7, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000);
                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;
                IList<DltTraceLineBase> lines = idbBlock.DecodePacket(packet, timeStamp, 0).ToList();
                Assert.That(lines, Has.Count.EqualTo(1));

                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodePacketOutputStream(bool littleEndian)
        {
            using (MemoryOutput output = new MemoryOutput())
            using (BlockReader reader = new BlockReader(new PcapTraceDecoderFactory(output))) {
                _ = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.IdbSmallLinkEth : PcapBlocks.IdbSmallLinkEthBigEndian, 0);
                Assert.That(block, Is.TypeOf<InterfaceDescriptionBlock>());

                ReadOnlySpan<byte> packet = littleEndian ?
                    PcapBlocks.EpbData.AsSpan(28..^4) :
                    PcapBlocks.EpbDataBigEndian.AsSpan(28..^4);

                DateTime timeStamp = new DateTime(2020, 6, 7, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000);
                InterfaceDescriptionBlock idbBlock = (InterfaceDescriptionBlock)block;

                IList<DltTraceLineBase> lines = idbBlock.DecodePacket(packet, timeStamp, 0).ToList();
                Assert.That(lines, Is.Empty);

                IList<DltTraceLineBase> memLines = (from line in output.Lines select line.Line).ToList();

                Assert.That(memLines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(memLines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(memLines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(memLines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }
    }
}
