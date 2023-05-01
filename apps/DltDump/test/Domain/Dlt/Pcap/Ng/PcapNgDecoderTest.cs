namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using Infrastructure;
    using NUnit.Framework;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Test decoding legacy PCAP formats.
    /// </summary>
    /// <typeparam name="TDec">The type of the decoder.</typeparam>
    /// <remarks>
    /// Both decoders should behave the same and transparent for the same input. The <see cref="DltPcapTraceDecoder"/>
    /// tries to determine the right format based on the first 32-bits.
    /// </remarks>
    [TestFixture(typeof(DltPcapNgDecoder), true)]
    [TestFixture(typeof(DltPcapNgDecoder), false)]
    [TestFixture(typeof(DltPcapTraceDecoder), true)]
    [TestFixture(typeof(DltPcapTraceDecoder), false)]
    public class PcapNgDecoderTest<TDec> : PcapDecoderTestBase where TDec : class, ITraceDecoder<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcapNgDecoderTest{TDec}"/> class.
        /// </summary>
        /// <param name="outputStream">
        /// If set to <see langword="true"/>, test with an <see cref="IOutputStream"/>.
        /// </param>
        public PcapNgDecoderTest(bool outputStream) : base(outputStream) { }

        private TDec Create()
        {
            return Create(false);
        }

        private TDec Create(bool nullOutput)
        {
            ITraceDecoderFactory<DltTraceLineBase> factory;
            if (nullOutput || MemOutputStream == null) {
                factory = new PcapTraceDecoderFactory(null, null);
                if (typeof(TDec) == typeof(DltPcapNgDecoder)) return new DltPcapNgDecoder(factory) as TDec;
                if (typeof(TDec) == typeof(DltPcapTraceDecoder)) return new DltPcapTraceDecoder() as TDec;
            } else {
                factory = new PcapTraceDecoderFactory(MemOutputStream, null);
                if (typeof(TDec) == typeof(DltPcapNgDecoder)) return new DltPcapNgDecoder(factory) as TDec;
                if (typeof(TDec) == typeof(DltPcapTraceDecoder)) return new DltPcapTraceDecoder(MemOutputStream, null) as TDec;
            }

            throw new NotImplementedException();
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidHeader(int chunk)
        {
            byte[] header = new byte[] {
                0x0D, 0x0D, 0x0D, 0x0D, 0x1C, 0x00, 0x04, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x04, 0x00
            };

            using (TDec decoder = Create()) {
                // The first decode might now throw, as it sees an error it marks it as corrupted.
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                    decoder.Flush();
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMajorVersion(int chunk)
        {
            byte[] header = new byte[] {
                0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x02, 0x00, 0x00, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
            };

            // We can't ignore the wrong version, as we theoretically don't know how to decode any further.
            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, new[] { header, PcapBlocks.IdbData, PcapBlocks.EpbData }, chunk);
                    decoder.Flush();
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMinorVersion(int chunk)
        {
            byte[] header = new byte[] {
                0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x01, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
            };

            // We can't ignore the wrong version, as we theoretically don't know how to decode any further.
            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, new[] { header, PcapBlocks.IdbData, PcapBlocks.EpbData }, chunk);
                    decoder.Flush();
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidSnapLength(int chunk)
        {
            byte[] idb = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x14, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                // The IDB is incorrect, but shouldn't raise an error, only that packets on this interface are ignored.
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, idb, PcapBlocks.EpbData }, chunk);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void UnknownLinkType(int chunk)
        {
            byte[] idb = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
                0x14, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                // The IDB link type is unknown, but shouldn't raise an error, only that packets on this interface are ignored.
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, idb, PcapBlocks.EpbData }, chunk);
                Assert.That(lines, Is.Empty);
            }
        }

        [Test]
        public void DisposeTest()
        {
            using (TDec decoder = Create()) {
                decoder.Dispose();
                Assert.That(() => {
                    _ = decoder.Decode(PcapBlocks.ShbSmall, 0);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void DisposeTest2()
        {
            using (TDec decoder = Create()) {
                _ = decoder.Decode(PcapBlocks.ShbSmall, 0);
                decoder.Dispose();
                Assert.That(() => {
                    _ = decoder.Decode(PcapBlocks.IdbSmallLinkEth, 0);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodePacket(int chunk)
        {
            using (TDec decoder = Create()) {
                // The IDB is incorrect, but shouldn't raise an error, only that packets on this interface are ignored.
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, PcapBlocks.IdbSmallLinkEth, PcapBlocks.EpbData }, chunk);
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        private static byte[] GetLargePacket(int blockId)
        {
            byte[] epbHeader = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x46, 0xA8, 0x05, 0x00, 0xFE, 0x05, 0x68, 0xEC
            };
            byte[] largePacket = new byte[70000];
            BitOperations.Copy32ShiftLittleEndian(blockId, largePacket, 0); // Header is an EPB
            BitOperations.Copy32ShiftLittleEndian(largePacket.Length, largePacket, 4);
            BitOperations.Copy32ShiftLittleEndian(largePacket.Length, largePacket, largePacket.Length - 4);
            Array.Copy(epbHeader, 0, largePacket, 8, epbHeader.Length);
            BitOperations.Copy32ShiftLittleEndian(largePacket.Length - 32, largePacket, 20);
            BitOperations.Copy32ShiftLittleEndian(largePacket.Length - 32, largePacket, 24);
            return largePacket;
        }

        /// <summary>
        /// Decodes two frames, with a "large" frame in between.
        /// </summary>
        /// <remarks>
        /// If we see a valid PCAP frame that is bigger than 2048 bytes, we don't decode it, but "skip" it. This is
        /// because it's bigger than an Ethernet MTU (1500 bytes).
        /// </remarks>
        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeWithDiscard(int chunk)
        {
            byte[] largePacket = GetLargePacket(6);
            using (TDec decoder = Create()) {
                // There is a packet that is so large it wont' fit in internal buffers. It will be ignored.
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, PcapBlocks.IdbSmallLinkEth, largePacket }, chunk);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeWithDiscardBeforeIdb(int chunk)
        {
            byte[] largePacket = GetLargePacket(50);
            using (TDec decoder = Create()) {
                // There is a packet between SHB and IDB that is so large it wont' fit in internal buffers. It will be ignored.
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, largePacket, PcapBlocks.IdbSmallLinkEth, PcapBlocks.EpbData }, chunk);
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeWithDiscardBeforeEbp(int chunk)
        {
            byte[] largePacket = GetLargePacket(6);
            using (TDec decoder = Create()) {
                // Large packet between IDB and EPB
                var lines = Decode(decoder, new[] { PcapBlocks.ShbSmall, PcapBlocks.IdbSmallLinkEth, largePacket, PcapBlocks.EpbData }, chunk);
                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void IgnoreUnknownPacket(int chunk)
        {
            using (TDec decoder = Create()) {
                // Have SHB, IDB, EPB, unknown, EPB
                var lines = Decode(decoder, new[] {
                    PcapBlocks.ShbSmall, PcapBlocks.IdbSmallLinkEth, PcapBlocks.EpbData, PcapBlocks.CustomSmall, PcapBlocks.EpbData
                }, chunk);
                Assert.That(lines.Count, Is.EqualTo(2));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[1].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void MultipleShb(int chunk)
        {
            using (TDec decoder = Create()) {
                // Have SHB, IDB, EPB, SHB, IDB, EPB, and can even change the endianness in the file.
                var lines = Decode(decoder, new[] {
                    PcapBlocks.ShbData, PcapBlocks.IdbData, PcapBlocks.EpbData,
                    PcapBlocks.ShbSmallBigEndian, PcapBlocks.IdbEthNanoSecBigEndian, PcapBlocks.EpbDataNanoBigEndian
                }, chunk);
                Assert.That(lines.Count, Is.EqualTo(2));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622498)));
                Assert.That(lines[1].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }

        [Test]
        public void FlushBeforeDecode()
        {
            using (TDec decoder = Create()) {
                var lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void CorruptedPcapNgFile(int chunk)
        {
            byte[] epbCorruptData = new byte[] {
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

                0xFF, 0xEE, 0xFF, 0xEE
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, new[] {
                    PcapBlocks.ShbData, PcapBlocks.IdbData, PcapBlocks.EpbData, epbCorruptData
                }, chunk);

                Assert.That(() => {
                    Decode(decoder, PcapBlocks.ShbData, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30, DateTimeKind.Utc).AddNanoSeconds(970622000)));
                Assert.That(lines[0].Text, Is.EqualTo("DLT Argument test string.."));
            }
        }
    }
}