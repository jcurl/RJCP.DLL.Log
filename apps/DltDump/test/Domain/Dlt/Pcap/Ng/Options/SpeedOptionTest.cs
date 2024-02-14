namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class SpeedOptionTest
    {
        [TestCase(OptionCodes.IdbSpeed, true)]
        [TestCase(OptionCodes.IdbSpeed, false)]
        [TestCase(OptionCodes.IdbTxSpeed, true)]
        [TestCase(OptionCodes.IdbTxSpeed, false)]
        [TestCase(OptionCodes.IdbRxSpeed, true)]
        [TestCase(OptionCodes.IdbRxSpeed, false)]
        public void Speed100Mbps(int optionCode, bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0xFF, 0xFF, 0x08, 0x00, 0x40, 0x42, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0xFF, 0xFF, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x42, 0x40 };
            BitOperations.Copy16Shift(optionCode, data, 0, littleEndian);

            PcapOptions options = new(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<SpeedOption>());
            Assert.That(option.OptionCode, Is.EqualTo(optionCode));
            Assert.That(option.Length, Is.EqualTo(8));

            SpeedOption speedOption = (SpeedOption)option;
            Assert.That(speedOption.Speed, Is.EqualTo(1_000_000));
        }

        [TestCase(OptionCodes.IdbSpeed, true)]
        [TestCase(OptionCodes.IdbSpeed, false)]
        [TestCase(OptionCodes.IdbTxSpeed, true)]
        [TestCase(OptionCodes.IdbTxSpeed, false)]
        [TestCase(OptionCodes.IdbRxSpeed, true)]
        [TestCase(OptionCodes.IdbRxSpeed, false)]
        public void SpeedWrongLength(int optionCode, bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0xFF, 0xFF, 0x04, 0x00, 0x40, 0x42, 0x0F, 0x00 } :
                new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x0F, 0x42, 0x40 };
            BitOperations.Copy16Shift(optionCode, data, 0, littleEndian);

            PcapOptions options = new(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(optionCode));
            Assert.That(option.Length, Is.EqualTo(4));
        }
    }
}
