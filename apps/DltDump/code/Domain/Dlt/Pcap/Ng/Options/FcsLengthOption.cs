namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;

    /// <summary>
    /// Defines the Frame Checksum Length.
    /// </summary>
    public sealed class FcsLengthOption : PcapOption
    {
        internal static FcsLengthOption Create(int option, int length, ReadOnlySpan<byte> buffer)
        {
            if (length != 1) return null;

            int fcsLength = buffer[0];
            return new FcsLengthOption(option, length, fcsLength);
        }

        private FcsLengthOption(int option, int length, int fcsLength)
            : base(option, length)
        {
            FcsLength = fcsLength;
        }

        /// <summary>
        /// Gets the length of the Frame Checksum.
        /// </summary>
        /// <value>The length of the Frame Checksum.</value>
        public int FcsLength { get; }
    }
}
