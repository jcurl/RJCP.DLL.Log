namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    /// <summary>
    /// The result of adding a fragment.
    /// </summary>
    public enum IpFragmentResult
    {
        /// <summary>
        /// The list of fragments is still incomplete, so that no reassembly can occur.
        /// </summary>
        Incomplete,

        /// <summary>
        /// All fragments have been reassembled.
        /// </summary>
        Reassembled,

        /// <summary>
        /// Adding the packet results in an error, there is an overlap.
        /// </summary>
        InvalidOverlap,

        /// <summary>
        /// Adding the packet is more than 15s after the last packet. Likely a packet was lost.
        /// </summary>
        InvalidTimeOut,

        /// <summary>
        /// Adding the packet is more than the last packet.
        /// </summary>
        InvalidOffset,

        /// <summary>
        /// Adding the last packet, when this fragment list already has a last packet.
        /// </summary>
        InvalidDuplicateLastPacket
    }
}
