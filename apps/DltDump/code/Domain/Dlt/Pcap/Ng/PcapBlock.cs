namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    /// <summary>
    /// Describes a generic PCAP-NG block.
    /// </summary>
    public readonly struct PcapBlock : IPcapBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcapBlock"/> class.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="length">The length of the block.</param>
        public PcapBlock(int blockId, int length)
        {
            BlockId = blockId;
            Length = length;
        }

        /// <summary>
        /// Gets the block identifier.
        /// </summary>
        /// <value>
        /// The block identifier.
        /// </value>
        public int BlockId { get; }

        /// <summary>
        /// Gets the length of this block.
        /// </summary>
        /// <value>
        /// The length of this block.
        /// </value>
        public int Length { get; }
    }
}
