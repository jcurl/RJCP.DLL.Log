namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    /// <summary>
    /// Describes the last option
    /// </summary>
    public sealed class EndOfOption : IPcapOption
    {
        /// <summary>
        /// Gets the option code, which is 0 (zero).
        /// </summary>
        /// <value>The option code, which is 0 (zero).</value>
        public int OptionCode { get; }

        /// <summary>
        /// Gets the length of this option, which is 0 (zero).
        /// </summary>
        /// <value>The length of this option, which is 0 (zero).</value>
        public int Length { get; }
    }
}
