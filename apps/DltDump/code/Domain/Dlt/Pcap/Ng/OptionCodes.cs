namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    /// <summary>
    /// List of supported PCAP option codes.
    /// </summary>
    public static class OptionCodes
    {
        public const int EndOfOpt = 0;

        public const int ShbComment = 1;
        public const int ShbHardware = 2;
        public const int ShbOs = 3;
        public const int ShbUserAppl = 4;

        public const int IdbName = 2;
        public const int IdbDescription = 3;
        public const int IdbSpeed = 8;
        public const int IdbTsResolution = 9;
        public const int IdbOs = 12;
        public const int IdbFcsLen = 13;
        public const int IdbHardware = 15;
        public const int IdbTxSpeed = 16;
        public const int IdbRxSpeed = 17;
    }
}
