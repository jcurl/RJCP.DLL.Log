namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;
    using RJCP.Core;

    /// <summary>
    /// Provides the speed of an interface.
    /// </summary>
    public sealed class SpeedOption : PcapOption
    {
        internal static SpeedOption Create(int option, int length, ReadOnlySpan<byte> buffer, bool littleEndian)
        {
            if (length != 8) return null;

            ulong speed = unchecked((ulong)BitOperations.To64Shift(buffer, littleEndian));
            return new SpeedOption(option, length, speed);
        }

        private SpeedOption(int option, int length, ulong value)
            : base(option, length)
        {
            Speed = value;
        }

        /// <summary>
        /// Gets the speed, in bits per second.
        /// </summary>
        /// <value>The speed, in bits per second.</value>
        public ulong Speed { get; }
    }
}
