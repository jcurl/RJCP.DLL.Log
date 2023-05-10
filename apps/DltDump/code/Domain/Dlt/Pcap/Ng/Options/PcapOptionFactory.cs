namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng.Options
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Factory for generating a <see cref="IPcapOption"/> from an option buffer.
    /// </summary>
    public class PcapOptionFactory
    {
        private static readonly IPcapOption EndOfOption = new EndOfOption();

        private readonly bool m_LittleEndian;
        private readonly Dictionary<int, HashSet<int>> m_Logged = new Dictionary<int, HashSet<int>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapOptionFactory"/> class.
        /// </summary>
        /// <param name="littleEndian">
        /// If <see langword="true"/>, then set binary decoding to be little endian, else binary decoding is big endian.
        /// </param>
        public PcapOptionFactory(bool littleEndian)
        {
            m_LittleEndian = littleEndian;
        }

        /// <summary>
        /// Creates the specified option.
        /// </summary>
        /// <param name="option">The option code.</param>
        /// <param name="length">The length of the buffer for the option code.</param>
        /// <param name="buffer">The buffer to parse.</param>
        /// <returns>
        /// An <see cref="IPcapOption"/> that is decoded, or <see langword="null"/> if there is an error decoding the
        /// buffer.
        /// </returns>
        /// <remarks>
        /// Returning <see langword="null"/> is a performance optimization instead of raising an exception as a
        /// corrupted file could generate this condition regularly.
        /// </remarks>
        public IPcapOption Create(int block, int option, int length, ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < length) return null;
            if (option == OptionCodes.EndOfOpt) return EndOfOption;

            IPcapOption result = null;
            switch (block) {
            case BlockCodes.SectionHeaderBlock:
                result = CreateShbOption(option, length, buffer);
                break;

            case BlockCodes.InterfaceDescriptionBlock:
                result = CreateIdbOption(option, length, buffer);
                break;

            default:
                if (ShouldLogUnknown(block, option)) {
                    Log.Pcap.TraceEvent(TraceEventType.Information,
                        "Unknown Block 0x{0:x} Option {1} of length {2}", block, option, length);
                }
                break;
            }

            if (result is null) return new PcapOption(option, length);
            return result;
        }

        private IPcapOption CreateShbOption(int option, int length, ReadOnlySpan<byte> buffer)
        {
            IPcapOption result = null;
            switch (option) {
            case OptionCodes.ShbComment:
            case OptionCodes.ShbHardware:
            case OptionCodes.ShbOs:
            case OptionCodes.ShbUserAppl:
                result = StringOption.Create(option, length, buffer);
                break;
            default:
                if (ShouldLogUnknown(BlockCodes.SectionHeaderBlock, option)) {
                    Log.Pcap.TraceEvent(TraceEventType.Information,
                        "Unknown SHB Option {0} of length {1}", option, length);
                }
                break;
            }
            return result;
        }

        private IPcapOption CreateIdbOption(int option, int length, ReadOnlySpan<byte> buffer)
        {
            IPcapOption result = null;
            switch (option) {
            case OptionCodes.IdbName:
            case OptionCodes.IdbDescription:
            case OptionCodes.IdbOs:
            case OptionCodes.IdbHardware:
                result = StringOption.Create(option, length, buffer);
                break;
            case OptionCodes.IdbSpeed:
            case OptionCodes.IdbTxSpeed:
            case OptionCodes.IdbRxSpeed:
                result = SpeedOption.Create(option, length, buffer, m_LittleEndian);
                if (result is null) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "Invalid IDB Option {0} 'if_XXspeed' of length {1}", option, length);
                }
                break;
            case OptionCodes.IdbTsResolution:
                result = TimeResolutionOption.Create(option, length, buffer);
                if (result is null) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "Invalid IDB Option {0} 'if_tsresol' of length {1}", option, length);
                }
                break;
            case OptionCodes.IdbFcsLen:
                result = FcsLengthOption.Create(option, length, buffer);
                if (result is null) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "Invalid IDB Option {0} 'if_fcslen' of length {1}", option, length);
                }
                break;
            default:
                if (ShouldLogUnknown(BlockCodes.InterfaceDescriptionBlock, option)) {
                    Log.Pcap.TraceEvent(TraceEventType.Information,
                        "Unknown IDB Option {0} of length {1}", option, length);
                }
                break;
            }
            return result;
        }

        /// <summary>
        /// Checks if we should log the unknown option for the block.
        /// </summary>
        /// <param name="block">The block to check for.</param>
        /// <param name="option">The option to check for.</param>
        /// <returns>
        /// Returns <see langword="true"/> if this entry should be logged, otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method helps to reduce spam in the logs, so that it is only logged once. If a file contains an unknown
        /// option for a block, then we are likely to see it multiple times, so only log the first instance.
        /// </remarks>
        private bool ShouldLogUnknown(int block, int option)
        {
            if (m_Logged.TryGetValue(block, out HashSet<int> options)) {
                if (options.Contains(option)) return false;
            } else {
                options = new HashSet<int>();
                m_Logged.Add(block, options);
            }

            options.Add(option);
            return true;
        }
    }
}
