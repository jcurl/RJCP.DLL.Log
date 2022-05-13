namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using NUnit.Framework;

    [TestFixture]
    public class FcsLengthOptionTest
    {
        [TestCase(0, true)]
        [TestCase(0, false)]
        [TestCase(4, true)]
        [TestCase(4, false)]
        public void FcsLength(int value, bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x0D, 0x00, 0x01, 0x00, (byte)value } :
                new byte[] { 0x00, 0x0D, 0x00, 0x01, (byte)value };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<FcsLengthOption>());
            Assert.That(option.OptionCode, Is.EqualTo(13));
            Assert.That(option.Length, Is.EqualTo(1));

            FcsLengthOption fcsOption = (FcsLengthOption)option;
            Assert.That(fcsOption.FcsLength, Is.EqualTo(value));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FcsLengthInvalidLength(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x0D, 0x00, 0x02, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x0D, 0x00, 0x02, 0x00, 0x00 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(13));
            Assert.That(option.Length, Is.EqualTo(2));
        }
    }
}
