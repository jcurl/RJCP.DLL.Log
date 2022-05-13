namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    /// <summary>
    /// List of supported PCAP block codes.
    /// </summary>
    public static class BlockCodes
    {
        public const int SectionHeaderBlock = 0x0A0D0D0A;
        public const int InterfaceDescriptionBlock = 1;
        public const int EnhancedPacketBlock = 6;
    }
}
