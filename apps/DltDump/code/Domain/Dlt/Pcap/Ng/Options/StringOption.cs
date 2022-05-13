namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;
    using System.Text;

    /// <summary>
    /// A PCAP-NG option that contains a single string value.
    /// </summary>
    public sealed class StringOption : PcapOption
    {
        internal static StringOption Create(int option, int length, ReadOnlySpan<byte> buffer)
        {
            try {
                string value = Encoding.UTF8.GetString(buffer[..length]);
                return new StringOption(option, length, value);
            } catch (ArgumentException) {
                // Invalid unicode points
                return new StringOption(option, length, string.Empty);
            }
        }

        private StringOption(int option, int length, string value)
            : base(option, length)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the option.
        /// </summary>
        /// <value>The value of the option.</value>
        public string Value { get; }
    }
}
