namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    /// <summary>
    /// Represents a PCAP Block in a PCAP-NG file.
    /// </summary>
    public interface IPcapBlock
    {
        /// <summary>
        /// Gets the block identifier.
        /// </summary>
        /// <value>
        /// The block identifier.
        /// </value>
        int BlockId { get; }

        /// <summary>
        /// Gets the length of this block.
        /// </summary>
        /// <value>
        /// The length of this block.
        /// </value>
        int Length { get; }
    }
}
