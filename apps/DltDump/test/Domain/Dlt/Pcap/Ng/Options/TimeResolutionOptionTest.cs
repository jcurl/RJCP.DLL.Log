namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;
    using Infrastructure;
    using NUnit.Framework;

    [TestFixture]
    public class TimeResolutionOptionTest
    {
        [Test]
        public void DecodeTimeResMicroSecDefault()
        {
            TimeResolutionOption timeOption = new TimeResolutionOption();
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(6));  // Units of microseconds, 10^-6

            DateTime result = timeOption.GetTimeStamp(0x0005A846_EC6805FE);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecDecimalSi(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x00 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x00 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(0));

            DateTime result = timeOption.GetTimeStamp(0x00000000_5EEA0E8A);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResMilliSecDecimalSi(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x03 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x03 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(3));  // Units of milliseconds, 10^-3

            DateTime result = timeOption.GetTimeStamp(0x00000172_C248CEDA);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970000000)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResMicroSecDecimalSi(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x06 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x06 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(6));  // Units of microseconds, 10^-6

            DateTime result = timeOption.GetTimeStamp(0x0005A846_EC6805FE);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622000)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResNanoSecDecimalSi(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x09 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x09 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(9));  // Units of nanoseconds, 10^-9

            // 970622101, but Windows doesn't have the resolution, only to 100ns
            DateTime result = timeOption.GetTimeStamp(0x1619550B_76576895);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(970622100)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResMicroSecDecimalSi_2bytes(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x02, 0x00, 0x06, 0x00 } :
                new byte[] { 0x00, 0x09, 0x00, 0x02, 0x00, 0x06 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecDecimalSiMaxValue(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x00 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x00 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(0));

            DateTime result = timeOption.GetTimeStamp(0x0000003A_FFF4417F);
            Assert.That(result, Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecDecimalSiMaxValue2(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x00 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x00 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(0));

            DateTime result = timeOption.GetTimeStamp(0x7FFFFFFF_FFFFFFFF);
            Assert.That(result, Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999999)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResMilliSecDecimalSiMaxValue(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x03 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x03 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(3));

            DateTime result = timeOption.GetTimeStamp(0x0000E677_D21FDBFF);
            Assert.That(result, Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999000000)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResMilliSecDecimalSiMaxValue2(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x03 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x03 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.DecimalSi));
            Assert.That(timeOption.Value, Is.EqualTo(3));

            DateTime result = timeOption.GetTimeStamp(0x7FFFFFFF_FFFFFFFF);
            Assert.That(result, Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).AddNanoSeconds(999999999)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResPicoSecDecimalSi(bool littleEndian)
        {
            // This resolution is not supported.
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x0C } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x0C };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x80 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x80 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
            Assert.That(timeOption.Value, Is.EqualTo(0));

            DateTime result = timeOption.GetTimeStamp(0x00000000_5EEA0E8A);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_2units(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x81 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x81 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
            Assert.That(timeOption.Value, Is.EqualTo(1));

            DateTime result = timeOption.GetTimeStamp(0x00000000_BDD41D15);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(500000000)));
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_1024units(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x8A } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x8A };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
            Assert.That(timeOption.Value, Is.EqualTo(10));

            DateTime result = timeOption.GetTimeStamp(0x0000017B_A83A2832);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(48_828_125))); // 50 / 2^10
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_1048576units(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0x94 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0x94 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
            Assert.That(timeOption.Value, Is.EqualTo(20));

            DateTime result = timeOption.GetTimeStamp(0x0005EEA0_E8A00C80);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(3_051_757))); // 3200 / 2^20
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_1048576units_2bytes(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x02, 0x00, 0x14, 0x80 } :
                new byte[] { 0x00, 0x09, 0x00, 0x02, 0x80, 0x14 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_32bitunits(bool littleEndian)
        {
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0xA0 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0xA0 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<TimeResolutionOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));

            TimeResolutionOption timeOption = (TimeResolutionOption)option;
            Assert.That(timeOption.Multiplier, Is.EqualTo(TimeResolutionOption.MultiplierMode.FixedPoint));
            Assert.That(timeOption.Value, Is.EqualTo(32));

            DateTime result = timeOption.GetTimeStamp(0x5EEA0E8A_4FD52AC9);
            Assert.That(result, Is.EqualTo(new DateTime(2020, 6, 17, 12, 37, 30).AddNanoSeconds(311_846_422))); // 0x4FD52AC9 / 2^32
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DecodeTimeResSecBinary_33bitunits(bool littleEndian)
        {
            // This resolution is not supported.
            byte[] data = littleEndian ?
                new byte[] { 0x09, 0x00, 0x01, 0x00, 0xA1 } :
                new byte[] { 0x00, 0x09, 0x00, 0x01, 0xA1 };

            PcapOptions options = new PcapOptions(littleEndian);
            IPcapOption option = options.Add(BlockCodes.InterfaceDescriptionBlock, data);
            Assert.That(option, Is.TypeOf<PcapOption>());
            Assert.That(option.OptionCode, Is.EqualTo(OptionCodes.IdbTsResolution));
            Assert.That(option.Length, Is.EqualTo(1));
        }
    }
}
