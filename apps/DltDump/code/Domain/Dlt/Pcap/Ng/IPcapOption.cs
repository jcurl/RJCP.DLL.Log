namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    /// <summary>
    /// Interface describing a PCAP-NG option.
    /// </summary>
    public interface IPcapOption
    {
        /// <summary>
        /// Gets the option code.
        /// </summary>
        /// <value>The option code.</value>
        int OptionCode { get; }

        /// <summary>
        /// Gets the length of this option.
        /// </summary>
        /// <value>The length of this option.</value>
        int Length { get; }
    }
}
