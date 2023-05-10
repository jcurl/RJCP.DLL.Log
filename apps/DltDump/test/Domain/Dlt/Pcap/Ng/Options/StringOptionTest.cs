namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class StringOptionTest
    {
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbComment, false)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbComment, true)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbHardware, false)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbHardware, true)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbOs, false)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbOs, true)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbUserAppl, false)]
        [TestCase(BlockCodes.SectionHeaderBlock, OptionCodes.ShbUserAppl, true)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbName, false)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbName, true)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbDescription, false)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbDescription, true)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbOs, false)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbOs, true)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbHardware, false)]
        [TestCase(BlockCodes.InterfaceDescriptionBlock, OptionCodes.IdbHardware, true)]
        public void DecodeStringOption(int blockCode, int optionCode, bool littleEndian)
        {
            byte[] data = new byte[14];

            BitOperations.Copy16Shift(optionCode, data.AsSpan(), littleEndian);
            int length = Encoding.UTF8.GetBytes("TestString", 0, 10, data, 4);
            BitOperations.Copy16Shift(length, data.AsSpan(2), littleEndian);

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(blockCode, data);
            Assert.That(option, Is.TypeOf<StringOption>());
            Assert.That(option.OptionCode, Is.EqualTo(optionCode));
            Assert.That(option.Length, Is.EqualTo(data.Length - 4));

            StringOption strOption = (StringOption)option;
            Assert.That(strOption.Value, Is.EqualTo("TestString"));

            Assert.That(options, Has.Count.EqualTo(1));
            Assert.That(options.Contains(0), Is.False);
            Assert.That(options.IndexOf(0), Is.EqualTo(-1));

            Assert.That(options.Contains(optionCode), Is.True);
            Assert.That(options.IndexOf(optionCode), Is.EqualTo(0));
        }
    }
}
