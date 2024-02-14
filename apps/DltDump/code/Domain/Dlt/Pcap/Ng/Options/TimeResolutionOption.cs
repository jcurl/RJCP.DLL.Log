namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;

    /// <summary>
    /// Specifies the time resolution for the interface, and allows conversion to a DateTime.
    /// </summary>
    public sealed class TimeResolutionOption : PcapOption
    {
        internal static TimeResolutionOption Create(int option, int length, ReadOnlySpan<byte> buffer)
        {
            if (length != 1) return null;

            MultiplierMode mode = MultiplierMode.DecimalSi;
            ulong res = buffer[0];
            if ((res & 0x80) != 0) {
                mode = MultiplierMode.FixedPoint;
                res &= 0x7F;
            }

            switch (mode) {
            case MultiplierMode.DecimalSi:
                if (res > 9) return null;
                break;
            case MultiplierMode.FixedPoint:
                if (res > 32) return null;
                break;
            }

            return new TimeResolutionOption(option, length, mode, (int)res);
        }

        /// <summary>
        /// Defines the mode for the value.
        /// </summary>
        public enum MultiplierMode
        {
            /// <summary>
            /// Undefined, should never occur.
            /// </summary>
            None,

            /// <summary>
            /// Is fixed point, with <see cref="Value"/> defining the number of bits.
            /// </summary>
            FixedPoint,

            /// <summary>
            /// Defines the fractional portion in SI units.
            /// </summary>
            DecimalSi
        }

        // The amount to shift, starting with 2 ^ 1
        private static readonly ulong[] BitMask = new ulong[] {
            0x00000001, 0x00000003, 0x00000007, 0x0000000F, 0x0000001F, 0x0000003F, 0x0000007F, 0x000000FF,
            0x000001FF, 0x000003FF, 0x000007FF, 0x00000FFF, 0x00001FFF, 0x00003FFF, 0x00007FFF, 0x0000FFFF,
            0x0001FFFF, 0x0003FFFF, 0x0007FFFF, 0x000FFFFF, 0x001FFFFF, 0x003FFFFF, 0x007FFFFF, 0x00FFFFFF,
            0x01FFFFFF, 0x03FFFFFF, 0x07FFFFFF, 0x0FFFFFFF, 0x1FFFFFFF, 0x3FFFFFFF, 0x7FFFFFFF, 0xFFFFFFFF
        };

        // The amount to divide to get the seconds, starting with 10^1
        private static readonly ulong[] TimeDivide = new ulong[] {
            10, 100, 1_000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000, 1_000_000_000
        };

        // The amount to multiply the modulo, to get the number of ns
        private static readonly ulong[] TimePortionMultiply = new ulong[] {
            100_000_000, 10_000_000, 1_000_000, 100_000, 10_000, 1_000, 100, 10, 1
        };

        private readonly ulong m_BitMask;
        private readonly ulong m_Divide;
        private readonly ulong m_Multiply;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeResolutionOption"/> class.
        /// </summary>
        /// <remarks>
        /// This is the default constructor, which creates a <see cref="TimeResolutionOption"/>
        /// with units if microseconds (e.g. 10^-6).
        /// </remarks>
        public TimeResolutionOption()
            : this(OptionCodes.IdbTsResolution, 0, MultiplierMode.DecimalSi, 6)
        { }

        private TimeResolutionOption(int option, int length, MultiplierMode mode, int value)
            : base(option, length)
        {
            Multiplier = mode;
            Value = value;

            if (value > 0) {
                if (mode == MultiplierMode.FixedPoint) {
                    m_BitMask = BitMask[value - 1];
                    m_Divide = 1UL << value;
                } else {
                    m_Multiply = TimePortionMultiply[value - 1];
                    m_Divide = TimeDivide[value - 1];
                }
            }
        }

        /// <summary>
        /// Defines the mode for the value.
        /// </summary>
        /// <value>The mode for the value.</value>
        public MultiplierMode Multiplier { get; }

        /// <summary>
        /// Gets the value, the SI units or the number of fixed point binary digits.
        /// </summary>
        /// <value>The value, the SI units or the number of fixed point binary digits.</value>
        public int Value { get; }

        private const long MaxTimeSeconds = 0x0000003A_FFF4417F;

        /// <summary>
        /// Gets the time stamp from a PCAP-NG 64-bit time stamp.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <returns>The converted time stamp.</returns>
        public DateTime GetTimeStamp(ulong timeStamp)
        {
            if (Value == 0) {
                if (timeStamp is > 0 and <= MaxTimeSeconds) {
                    return DateTimeOffset.FromUnixTimeSeconds((long)timeStamp).UtcDateTime;
                } else {
                    return DateTimeOffset.FromUnixTimeSeconds(MaxTimeSeconds)
                        .AddTicks(TimeSpan.TicksPerSecond - 1).UtcDateTime;
                }
            }

            long sec;
            long nsec;
            if (Multiplier == MultiplierMode.FixedPoint) {
                sec = (long)(timeStamp >> Value);
                nsec = (long)((double)1_000_000_000 / m_Divide * (timeStamp & m_BitMask));
            } else {
                sec = (long)(timeStamp / m_Divide);
                nsec = (long)(timeStamp % m_Divide * m_Multiply);
            }

            if (sec <= MaxTimeSeconds) {
                return DateTimeOffset.FromUnixTimeSeconds(sec)
                    .AddTicks(nsec / 100).UtcDateTime;
            } else {
                return DateTimeOffset.FromUnixTimeSeconds(MaxTimeSeconds)
                    .AddTicks(TimeSpan.TicksPerSecond - 1).UtcDateTime;
            }
        }
    }
}
