namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using NUnit.Framework;
    using Options;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class SectionHeaderBlockTest
    {
        private static readonly ITraceDecoderFactory<DltTraceLineBase> DefaultPcapFactory = new PcapTraceDecoderFactory();

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlock(bool littleEndian)
        {
            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.ShbData : PcapBlocks.ShbDataBigEndian, 0);

                Assert.That(block, Is.TypeOf<SectionHeaderBlock>());
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(192));

                SectionHeaderBlock shbBlock = (SectionHeaderBlock)block;
                Assert.That(shbBlock.IsLittleEndian, Is.EqualTo(littleEndian));
                Assert.That(shbBlock.MajorVersion, Is.EqualTo(1));
                Assert.That(shbBlock.MinorVersion, Is.EqualTo(0));

                Assert.That(shbBlock.Options, Has.Count.EqualTo(3));
                Assert.That(shbBlock.Options[0].OptionCode, Is.EqualTo(OptionCodes.ShbHardware));
                Assert.That(shbBlock.Options[0].Length, Is.EqualTo(54));
                Assert.That(shbBlock.Options[0], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)shbBlock.Options[0]).Value, Is.EqualTo("Intel(R) Core(TM) i7-6700T CPU @ 2.80GHz (with SSE4.2)"));
                Assert.That(shbBlock.Options[1].OptionCode, Is.EqualTo(OptionCodes.ShbOs));
                Assert.That(shbBlock.Options[1].Length, Is.EqualTo(37));
                Assert.That(shbBlock.Options[1], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)shbBlock.Options[1]).Value, Is.EqualTo("64-bit Windows 10 (2009), build 19043"));
                Assert.That(shbBlock.Options[2].OptionCode, Is.EqualTo(OptionCodes.ShbUserAppl));
                Assert.That(shbBlock.Options[2].Length, Is.EqualTo(50));
                Assert.That(shbBlock.Options[2], Is.TypeOf<StringOption>());
                Assert.That(((StringOption)shbBlock.Options[2]).Value, Is.EqualTo("Dumpcap (Wireshark) 3.4.3 (v3.4.3-0-g6ae6cd335aa9)"));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BlockTooSmall(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x18, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0x18, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x18, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x18
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);

                // Even though the block is invalid, we return a "generic" block from the reader. This is in the hope we can continue
                // parsing until the next Section Header Block.
                Assert.That(block, Is.Not.InstanceOf<SectionHeaderBlock>());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MajorVersion2(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x02, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x02, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);

                // Even though the block is invalid, we return a "generic" block from the reader. This is in the hope we can continue
                // parsing until the next Section Header Block.
                Assert.That(block, Is.Not.InstanceOf<SectionHeaderBlock>());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MinorVersion5(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x05, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x05,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);

                // Even though the block is invalid, we return a "generic" block from the reader. This is in the hope we can continue
                // parsing until the next Section Header Block.
                Assert.That(block, Is.Not.InstanceOf<SectionHeaderBlock>());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MajorMinorVersionUnknown(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x02, 0x00, 0x05, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x02, 0x00, 0x05,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);

                // Even though the block is invalid, we return a "generic" block from the reader. This is in the hope we can continue
                // parsing until the next Section Header Block.
                Assert.That(block, Is.Not.InstanceOf<SectionHeaderBlock>());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CorruptedOptions(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x20, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x02, 0x00, 0x36, 0x00, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x20, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x02, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new BlockReader(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);

                // Even though the block is invalid, we return a "generic" block from the reader. This is in the hope we can continue
                // parsing until the next Section Header Block.
                Assert.That(block, Is.Not.InstanceOf<SectionHeaderBlock>());
            }
        }
    }
}
