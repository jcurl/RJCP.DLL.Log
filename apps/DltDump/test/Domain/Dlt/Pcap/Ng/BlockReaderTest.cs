namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class BlockReaderTest
    {
        private static readonly ITraceDecoderFactory<DltTraceLineBase> DefaultPcapFactory = new PcapTraceDecoderFactory();

        [TestCase(true)]
        [TestCase(false)]
        public void TestSectionHeaderBlock(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(littleEndian ? PcapBlocks.ShbData : PcapBlocks.ShbDataBigEndian);

                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(192));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSectionHeaderBlockMinimumSize(bool littleEndian)
        {
            ReadOnlySpan<byte> buffer = littleEndian ?
                PcapBlocks.ShbData.AsSpan(0, 12) :
                PcapBlocks.ShbDataBigEndian.AsSpan(0, 12);

            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(buffer);

                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(192));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSectionHeaderBlockTooSmall(bool littleEndian)
        {
            ReadOnlySpan<byte> buffer = littleEndian ?
                PcapBlocks.ShbData.AsSpan(0, 11) :
                PcapBlocks.ShbDataBigEndian.AsSpan(0, 11);

            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(buffer);
                Assert.That(block.BlockId, Is.EqualTo(0));
                Assert.That(block.Length, Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSectionHeaderBlockInvalidMagic(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(buffer);
                Assert.That(block.BlockId, Is.EqualTo(0));
                Assert.That(block.Length, Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSectionHeaderBlockLargeBuffer(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C, 0x01, 0x00, 0x00, 0x00
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(buffer);
                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestPcapBlockNoSectionHeader(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                PcapBlock block = reader.GetHeader(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian);
                Assert.That(block.BlockId, Is.EqualTo(0));
                Assert.That(block.Length, Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlock(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.ShbData : PcapBlocks.ShbDataBigEndian, 0);

                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(192));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockSmallest(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);

                Assert.That(block.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockMinimumSizeTruncated(bool littleEndian)
        {
            ReadOnlySpan<byte> buffer = littleEndian ?
                PcapBlocks.ShbData.AsSpan(0, 12) :
                PcapBlocks.ShbDataBigEndian.AsSpan(0, 12);

            // Because GetBlock expects a complete buffer, reading the block should fail.
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockTooSmall(bool littleEndian)
        {
            ReadOnlySpan<byte> buffer = littleEndian ?
                PcapBlocks.ShbSmall.AsSpan(0, 11) :
                PcapBlocks.ShbSmallBigEndian.AsSpan(0, 11);

            // Because GetBlock expects a complete buffer, reading the block should fail.
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockBadMagic(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockTruncated(bool littleEndian)
        {
            ReadOnlySpan<byte> buffer = littleEndian ?
                PcapBlocks.ShbSmall.AsSpan(0, 24) :
                PcapBlocks.ShbSmallBigEndian.AsSpan(0, 24);

            // Because GetBlock expects a complete buffer, reading the block should fail.
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockCorruptedLengthTooSmall(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockCorruptedLengthTooSmallWithMagic(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x08, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x00, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x08, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x00, 0x00, 0x00
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadSectionHeaderBlockLengthMismatch(bool littleEndian)
        {
            byte[] buffer = littleEndian ?
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x20, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x20
                };

            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(buffer, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadPcapBlock(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock shbblock = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                Assert.That(shbblock, Is.Not.Null);

                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block.BlockId, Is.EqualTo(0x20));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadPcapBlockNoSectionHeader(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadPcapSwitchSectionheader(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock shbblock = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                Assert.That(shbblock, Is.Not.Null);

                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block.BlockId, Is.EqualTo(0x20));
                Assert.That(block.Length, Is.EqualTo(28));

                shbblock = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmallBigEndian : PcapBlocks.ShbSmall, 0);
                Assert.That(shbblock, Is.Not.Null);

                block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmallBigEndian : PcapBlocks.CustomSmall, 0);
                Assert.That(block.BlockId, Is.EqualTo(0x20));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestDoesntSetEndianness(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock shbblock = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                Assert.That(shbblock, Is.Not.Null);

                // Even though this block is big endian, the reader still decodes little endian, until read.
                PcapBlock testBlock = reader.GetHeader(littleEndian ? PcapBlocks.ShbSmallBigEndian : PcapBlocks.ShbSmall);
                Assert.That(testBlock.BlockId, Is.EqualTo(BlockCodes.SectionHeaderBlock));
                Assert.That(testBlock.Length, Is.EqualTo(28));

                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block.BlockId, Is.EqualTo(0x20));
                Assert.That(block.Length, Is.EqualTo(28));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadPcapBlockReset(bool littleEndian)
        {
            using (BlockReader reader = new(DefaultPcapFactory)) {
                IPcapBlock shbblock = reader.GetBlock(littleEndian ? PcapBlocks.ShbSmall : PcapBlocks.ShbSmallBigEndian, 0);
                Assert.That(shbblock, Is.Not.Null);

                IPcapBlock block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block.BlockId, Is.EqualTo(0x20));
                Assert.That(block.Length, Is.EqualTo(28));

                reader.Reset();
                block = reader.GetBlock(littleEndian ? PcapBlocks.CustomSmall : PcapBlocks.CustomSmallBigEndian, 0);
                Assert.That(block, Is.Null); // Null, as we don't have a Section Header Block after a reset.
            }
        }
    }
}
