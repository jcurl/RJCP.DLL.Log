namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections;
    using NUnit.Framework;
    using Options;
    using RJCP.Core;

    [TestFixture]
    public class PcapOptionsTest
    {
        [TestCase(false)]
        [TestCase(true)]
        public void DefaultCollection(bool littleEndian)
        {
            PcapOptions options = new PcapOptions(littleEndian);
            Assert.That(options.Count, Is.EqualTo(0));
            Assert.That(options.Contains(0), Is.False);
            Assert.That(options.IndexOf(0), Is.EqualTo(-1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void DecodeEndOfOptions(bool littleEndian)
        {
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.TypeOf<EndOfOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.EndOfOpt));
            Assert.That(option.Length, Is.EqualTo(0));

            Assert.That(options.Count, Is.EqualTo(0));
            Assert.That(options.Contains(0), Is.False);
            Assert.That(options.IndexOf(0), Is.EqualTo(-1));
        }

        [TestCase(BlockCodes.SectionHeaderBlock, false)]
        [TestCase(BlockCodes.SectionHeaderBlock, true)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, false)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, true)]
        [TestCase(BlockCodes.EnhancedPacketBlock, false)]
        [TestCase(BlockCodes.EnhancedPacketBlock, true)]
        [TestCase(0x1EADBEEF, false)]
        [TestCase(0x1EADBEEF, true)]
        public void UnknownOption(int blockCode, bool littleEndian)
        {
            byte[] data = new byte[] { 0xEA, 0xFF, 0x00, 0x00, 0x00, 0x00 };
            BitOperations.Copy16Shift(2, data.AsSpan(2), littleEndian);
            int optionCode = littleEndian ? 0xFFEA : 0xEAFF;

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(blockCode, data);
            Assert.That(option, Is.TypeOf<PcapOption>());

            PcapOption pcapOption = (PcapOption)option;
            Assert.That(pcapOption.OptionCode, Is.EqualTo(optionCode));
            Assert.That(pcapOption.Length, Is.EqualTo(2));

            Assert.That(options.Count, Is.EqualTo(1));
            Assert.That(options.Contains(optionCode), Is.True);
            Assert.That(options.IndexOf(optionCode), Is.EqualTo(0));
            Assert.That(options[0].OptionCode, Is.EqualTo(optionCode));

            // Run it a second time
            option = options.Add(blockCode, data);
            Assert.That(option, Is.TypeOf<PcapOption>());

            pcapOption = (PcapOption)option;
            Assert.That(pcapOption.OptionCode, Is.EqualTo(optionCode));
            Assert.That(pcapOption.Length, Is.EqualTo(2));

            Assert.That(options.Count, Is.EqualTo(2));
            Assert.That(options.Contains(optionCode), Is.True);
            Assert.That(options.IndexOf(optionCode), Is.EqualTo(0));
            Assert.That(options.IndexOf(optionCode, 1), Is.EqualTo(1));
            Assert.That(options[1].OptionCode, Is.EqualTo(optionCode));

            // Run it a third time with a different block
            byte[] data2 = new byte[] { 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x01 };
            BitOperations.Copy16Shift(3, data2.AsSpan(2), littleEndian);
            optionCode = littleEndian ? 0x807F : 0x7F80;

            option = options.Add(blockCode, data2);
            Assert.That(option, Is.TypeOf<PcapOption>());

            pcapOption = (PcapOption)option;
            Assert.That(pcapOption.OptionCode, Is.EqualTo(optionCode));
            Assert.That(pcapOption.Length, Is.EqualTo(3));

            Assert.That(options.Count, Is.EqualTo(3));
            Assert.That(options.Contains(optionCode), Is.True);
            Assert.That(options.IndexOf(optionCode), Is.EqualTo(2));
            Assert.That(options.IndexOf(optionCode, 1), Is.EqualTo(2));
            Assert.That(options[2].OptionCode, Is.EqualTo(optionCode));
        }

        [Test]
        public void InvalidOptionZeroLength()
        {
            byte[] data = Array.Empty<byte>();
            PcapOptions options = new PcapOptions(true);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.Null);
        }

        [Test]
        public void InvalidOptionIncompleteOptionField()
        {
            byte[] data = new byte[] { 0x00 };
            PcapOptions options = new PcapOptions(true);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.Null);
        }

        [Test]
        public void InvalidOptionNoLengthField()
        {
            byte[] data = new byte[] { 0x00, 0x00 };
            PcapOptions options = new PcapOptions(true);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.Null);
        }

        [Test]
        public void InvalidOptionPartialLengthField()
        {
            byte[] data = new byte[] { 0x00, 0x00, 0x00 };
            PcapOptions options = new PcapOptions(true);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.Null);
        }

        [Test]
        public void InvalidLength()
        {
            byte[] data = new byte[] { 0x01, 0x00, 0x05, 0x00, 0x41 };
            PcapOptions options = new PcapOptions(true);
            IPcapOption option = options.Add(BlockCodes.SectionHeaderBlock, data);
            Assert.That(option, Is.Null);
        }

        private static readonly byte[] FcsData = new byte[] { 0x0D, 0x00, 0x01, 0x00, 0x00 };

        private static readonly byte[] SpeedData = new byte[]
            { 0x08, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private static readonly byte[] TsResData = new byte[] { 0x09, 0x00, 0x01, 0x00, 0x06 };
        private static readonly byte[] TsResDataInval = new byte[] { 0x09, 0x00, 0x02, 0x00, 0x06, 0x00 };

        [Test]
        public void IndexOf()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResData);

            Assert.That(options.IndexOf(OptionCodes.IdbFcsLen), Is.EqualTo(0));
            Assert.That(options.IndexOf(OptionCodes.IdbSpeed), Is.EqualTo(1));
            Assert.That(options.IndexOf(OptionCodes.IdbTsResolution), Is.EqualTo(2));
            Assert.That(options.IndexOf(OptionCodes.IdbName), Is.EqualTo(-1));
        }

        [Test]
        public void IndexOfInval()
        {
            // Even with invalid data, the option code is translated, but the type cannot be interpreted.
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResDataInval);

            Assert.That(options.IndexOf(OptionCodes.IdbFcsLen), Is.EqualTo(0));
            Assert.That(options.IndexOf(OptionCodes.IdbSpeed), Is.EqualTo(1));
            Assert.That(options.IndexOf(OptionCodes.IdbTsResolution), Is.EqualTo(2));
            Assert.That(options.IndexOf(OptionCodes.IdbName), Is.EqualTo(-1));
        }

        [Test]
        public void IndexOfMultiple()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResData);

            Assert.That(options.IndexOf(OptionCodes.IdbSpeed), Is.EqualTo(0));
            Assert.That(options.IndexOf(OptionCodes.IdbFcsLen), Is.EqualTo(1));
            Assert.That(options.IndexOf(OptionCodes.IdbTsResolution), Is.EqualTo(3));
            Assert.That(options.IndexOf(OptionCodes.IdbName), Is.EqualTo(-1));

            Assert.That(options.IndexOf(OptionCodes.IdbSpeed, 0), Is.EqualTo(0));
            Assert.That(options.IndexOf(OptionCodes.IdbFcsLen, 0), Is.EqualTo(1));
            Assert.That(options.IndexOf(OptionCodes.IdbTsResolution, 0), Is.EqualTo(3));
            Assert.That(options.IndexOf(OptionCodes.IdbName, 0), Is.EqualTo(-1));

            Assert.That(options.IndexOf(OptionCodes.IdbSpeed, 1), Is.EqualTo(2));
            Assert.That(options.IndexOf(OptionCodes.IdbSpeed, 2), Is.EqualTo(2));
            Assert.That(options.IndexOf(OptionCodes.IdbSpeed, 3), Is.EqualTo(-1));

            Assert.That(options.IndexOf(OptionCodes.IdbSpeed, 4), Is.EqualTo(-1));
        }

        [Test]
        public void Indexer()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResData);

            Assert.That(options[0].OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
            Assert.That(options[1].OptionCode, Is.EqualTo(OptionCodes.IdbFcsLen));
            Assert.That(options[2].OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
            Assert.That(options[3].OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
        }

        [Test]
        public void Enumerator()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResData);

            int i = 0;
            foreach (IPcapOption option in options) {
                switch (i) {
                case 0:
                    Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
                    break;
                case 1:
                    Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbFcsLen));
                    break;
                case 2:
                    Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
                    break;
                case 3:
                    Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                    break;
                default:
                    Assert.Fail("Too many elements in enumerable");
                    break;
                }

                i++;
            }
        }

        [Test]
        public void EnumeratorObject()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, TsResData);

            IEnumerable enumerable = options;
            int i = 0;
            foreach (object option in enumerable) {
                Assert.That(option, Is.InstanceOf<IPcapOption>());

                IPcapOption pcapOption = (IPcapOption)option;
                switch (i) {
                case 0:
                    Assert.That(pcapOption.OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
                    break;
                case 1:
                    Assert.That(pcapOption.OptionCode, Is.EqualTo(OptionCodes.IdbFcsLen));
                    break;
                case 2:
                    Assert.That(pcapOption.OptionCode, Is.EqualTo(OptionCodes.IdbSpeed));
                    break;
                case 3:
                    Assert.That(pcapOption.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
                    break;
                default:
                    Assert.Fail("Too many elements in enumerable");
                    break;
                }

                i++;
            }
        }

        [Test]
        public void DefaultReadOnly()
        {
            PcapOptions options = new PcapOptions(true);
            Assert.That(options.IsReadOnly, Is.False);
        }

        [Test]
        public void SetReadOnly()
        {
            PcapOptions options = new PcapOptions(true) {
                IsReadOnly = true
            };

            Assert.That(() => {
                _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            }, Throws.TypeOf<InvalidOperationException>());
            Assert.That(options.Count, Is.EqualTo(0));
        }

        [Test]
        public void SetReadOnly2()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.InterfaceDescriptionBlock, SpeedData);
            options.IsReadOnly = true;

            Assert.That(() => {
                _ = options.Add(BlockCodes.InterfaceDescriptionBlock, FcsData);
            }, Throws.TypeOf<InvalidOperationException>());
            Assert.That(options.Count, Is.EqualTo(1));
            Assert.That(options[0], Is.TypeOf<SpeedOption>());
        }

        [Test]
        public void SetReadOnlyCantReset()
        {
            PcapOptions options = new PcapOptions(true) {
                IsReadOnly = true
            };

            Assert.That(() => {
                options.IsReadOnly = false;
            }, Throws.TypeOf<InvalidOperationException>());
        }

        private static readonly byte[] ShbData = new byte[] {
            0x0A, 0x0D, 0x0D, 0x0A, 0xC0, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A,
            0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0x02, 0x00, 0x36, 0x00, 0x49, 0x6E, 0x74, 0x65, 0x6C, 0x28, 0x52, 0x29,
            0x20, 0x43, 0x6F, 0x72, 0x65, 0x28, 0x54, 0x4D, 0x29, 0x20, 0x69, 0x37,
            0x2D, 0x36, 0x37, 0x30, 0x30, 0x54, 0x20, 0x43, 0x50, 0x55, 0x20, 0x40,
            0x20, 0x32, 0x2E, 0x38, 0x30, 0x47, 0x48, 0x7A, 0x20, 0x28, 0x77, 0x69,
            0x74, 0x68, 0x20, 0x53, 0x53, 0x45, 0x34, 0x2E, 0x32, 0x29, 0x00, 0x00,
            0x03, 0x00, 0x25, 0x00, 0x36, 0x34, 0x2D, 0x62, 0x69, 0x74, 0x20, 0x57,
            0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x31, 0x30, 0x20, 0x28, 0x32,
            0x30, 0x30, 0x39, 0x29, 0x2C, 0x20, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x20,
            0x31, 0x39, 0x30, 0x34, 0x33, 0x00, 0x00, 0x00, 0x04, 0x00, 0x32, 0x00,
            0x44, 0x75, 0x6D, 0x70, 0x63, 0x61, 0x70, 0x20, 0x28, 0x57, 0x69, 0x72,
            0x65, 0x73, 0x68, 0x61, 0x72, 0x6B, 0x29, 0x20, 0x33, 0x2E, 0x34, 0x2E,
            0x33, 0x20, 0x28, 0x76, 0x33, 0x2E, 0x34, 0x2E, 0x33, 0x2D, 0x30, 0x2D,
            0x67, 0x36, 0x61, 0x65, 0x36, 0x63, 0x64, 0x33, 0x33, 0x35, 0x61, 0x61,
            0x39, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00
        };

        [Test]
        public void DecodeOptions()
        {
            PcapOptions options = new PcapOptions(true);
            int length = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24));
            Assert.That(length, Is.EqualTo(164));
            Assert.That(options.IsReadOnly, Is.True);
            Assert.That(options.Count, Is.EqualTo(3));

            Assert.That(options[0].OptionCode, Is.EqualTo(OptionCodes.ShbHardware));
            Assert.That(options[0].Length, Is.EqualTo(0x36));
            Assert.That(((StringOption)options[0]).Value, Is.EqualTo("Intel(R) Core(TM) i7-6700T CPU @ 2.80GHz (with SSE4.2)"));

            Assert.That(options[1].OptionCode, Is.EqualTo(OptionCodes.ShbOs));
            Assert.That(options[1].Length, Is.EqualTo(0x25));
            Assert.That(((StringOption)options[1]).Value, Is.EqualTo("64-bit Windows 10 (2009), build 19043"));

            Assert.That(options[2].OptionCode, Is.EqualTo(OptionCodes.ShbUserAppl));
            Assert.That(options[2].Length, Is.EqualTo(0x32));
            Assert.That(((StringOption)options[2]).Value, Is.EqualTo("Dumpcap (Wireshark) 3.4.3 (v3.4.3-0-g6ae6cd335aa9)"));
        }

        [TestCase(159)] // The length is OK, rounding up the length.
        [TestCase(160)]
        [TestCase(161)]
        [TestCase(162)]
        [TestCase(163)]
        public void DecodeOptionsNoEndOfOpts(int blockLen)
        {
            PcapOptions options = new PcapOptions(true);
            int length = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24, blockLen));
            Assert.That(length, Is.EqualTo(160));
            Assert.That(options.IsReadOnly, Is.True);
            Assert.That(options.Count, Is.EqualTo(3));

            Assert.That(options[0].OptionCode, Is.EqualTo(OptionCodes.ShbHardware));
            Assert.That(options[0].Length, Is.EqualTo(0x36));
            Assert.That(((StringOption)options[0]).Value, Is.EqualTo("Intel(R) Core(TM) i7-6700T CPU @ 2.80GHz (with SSE4.2)"));

            Assert.That(options[1].OptionCode, Is.EqualTo(OptionCodes.ShbOs));
            Assert.That(options[1].Length, Is.EqualTo(0x25));
            Assert.That(((StringOption)options[1]).Value, Is.EqualTo("64-bit Windows 10 (2009), build 19043"));

            Assert.That(options[2].OptionCode, Is.EqualTo(OptionCodes.ShbUserAppl));
            Assert.That(options[2].Length, Is.EqualTo(0x32));
            Assert.That(((StringOption)options[2]).Value, Is.EqualTo("Dumpcap (Wireshark) 3.4.3 (v3.4.3-0-g6ae6cd335aa9)"));
        }

        [Test]
        public void DecodeOptionsTruncated()
        {
            PcapOptions options = new PcapOptions(true);
            int length = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24, 6));
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(options.IsReadOnly, Is.False);
            Assert.That(options.Count, Is.EqualTo(0));
        }

        [Test]
        public void DecodeOptionsTruncatedMultipleOptions()
        {
            // Decoding options is atomic. All must be decoded, else none will be added.

            PcapOptions options = new PcapOptions(true);
            int length = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24, 72));
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(options.IsReadOnly, Is.False);
            Assert.That(options.Count, Is.EqualTo(0));
        }

        [Test]
        public void DecodeReadOnly()
        {
            PcapOptions options = new PcapOptions(true) {
                IsReadOnly = true
            };

            Assert.That(() => {
                _ = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24, 6));
            }, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DecodeClears()
        {
            PcapOptions options = new PcapOptions(true);
            _ = options.Add(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(84, 44));

            Assert.That(options.IsReadOnly, Is.False);
            Assert.That(options.Count, Is.EqualTo(1));
            Assert.That(options[0].OptionCode, Is.EqualTo(OptionCodes.ShbOs));

            // Now decode the entire block
            int length = options.Decode(BlockCodes.SectionHeaderBlock, ShbData.AsSpan(24, 164));
            Assert.That(length, Is.EqualTo(164));
            Assert.That(options.IsReadOnly, Is.True);
            Assert.That(options.Count, Is.EqualTo(3));

            Assert.That(options[0].OptionCode, Is.EqualTo(OptionCodes.ShbHardware));
            Assert.That(options[0].Length, Is.EqualTo(0x36));
            Assert.That(((StringOption)options[0]).Value, Is.EqualTo("Intel(R) Core(TM) i7-6700T CPU @ 2.80GHz (with SSE4.2)"));

            Assert.That(options[1].OptionCode, Is.EqualTo(OptionCodes.ShbOs));
            Assert.That(options[1].Length, Is.EqualTo(0x25));
            Assert.That(((StringOption)options[1]).Value, Is.EqualTo("64-bit Windows 10 (2009), build 19043"));

            Assert.That(options[2].OptionCode, Is.EqualTo(OptionCodes.ShbUserAppl));
            Assert.That(options[2].Length, Is.EqualTo(0x32));
            Assert.That(((StringOption)options[2]).Value, Is.EqualTo("Dumpcap (Wireshark) 3.4.3 (v3.4.3-0-g6ae6cd335aa9)"));
        }
    }
}
