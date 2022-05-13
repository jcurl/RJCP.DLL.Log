namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    /// <summary>
    /// A generic option that has no decoder.
    /// </summary>
    public class PcapOption : IPcapOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcapOption"/> class.
        /// </summary>
        /// <param name="optionCode">The option code.</param>
        public PcapOption(int optionCode, int length)
        {
            OptionCode = optionCode;
            Length = length;
        }

        /// <summary>
        /// Gets the option code.
        /// </summary>
        /// <value>The option code.</value>
        public int OptionCode { get; }

        /// <summary>
        /// Gets the length of this option.
        /// </summary>
        /// <value>The length of this option.</value>
        public int Length { get; }
    }
}
