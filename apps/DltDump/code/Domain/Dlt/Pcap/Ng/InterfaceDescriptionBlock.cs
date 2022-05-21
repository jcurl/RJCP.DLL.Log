namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Domain.Dlt.Pcap.Ng.Options;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Provide details from the Interface Description Block.
    /// </summary>
    public sealed class InterfaceDescriptionBlock : IPcapBlock, IDisposable
    {
        /// <summary>
        /// Decode the buffer to get the <see cref="InterfaceDescriptionBlock"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the complete block.</param>
        /// <param name="littleEndian">
        /// Decode using little endian if <see langword="true"/>, else big endian if <see langword="false"/>.
        /// </param>
        /// <param name="position">The position in the stream where the block starts.</param>
        /// <returns>The <see cref="InterfaceDescriptionBlock"/>.</returns>
        /// <remarks>
        /// This block does not check the identifier (it must be the <see cref="BlockCodes.InterfaceDescriptionBlock"/> or the
        /// length fields (which must be the same length as <paramref name="buffer"/>). It is assumed that the component
        /// calling this method has already checked the fields are correct (see <see cref="BlockReader"/>).
        /// </remarks>
        internal static InterfaceDescriptionBlock GetInterfaceDescriptionBlock(ReadOnlySpan<byte> buffer, bool littleEndian, IOutputStream outputStream, long position)
        {
            if (buffer.Length < 20) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Interface Description Block offset 0x{0:x} with buffer length too short (got {1}, expect >= 20)",
                    position, buffer.Length);
                return null;
            }

            CheckBlock(buffer, littleEndian, position);

            int linkType = BitOperations.To16Shift(buffer[8..], littleEndian);
            switch (linkType) {
            case LinkTypes.LINKTYPE_ETHERNET:
            case LinkTypes.LINKTYPE_LINUX_SLL:
                break;
            default:
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Interface Description Block offset 0x{0:x} with unsupported link type {1}",
                    position, linkType);
                return null;
            }

            int snapLen = BitOperations.To32Shift(buffer[12..], littleEndian);
            if (snapLen <= 0) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Interface Description Block offset 0x{0:x} with invalid snap length 0x{1:x}",
                    position, snapLen);
                return null;
            }

            PcapOptions options = new PcapOptions(littleEndian);
            int optionLength = options.Decode(BlockCodes.InterfaceDescriptionBlock, buffer[16..^4]);
            if (optionLength == -1) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Interface Description Block offset 0x{0:x} with corruption while decoding options", position);
                return null;
            }

            PacketDecoder decoder = outputStream == null ?
                new PacketDecoder(linkType) :
                new PacketDecoder(linkType, outputStream);
            return new InterfaceDescriptionBlock(buffer.Length, options, decoder) {
                LinkType = linkType,
                SnapLength = snapLen
            };
        }

        /// <summary>
        /// Checks the block for correctness in DEBUG mode.
        /// </summary>
        /// <param name="buffer">The buffer containing the Section Header Block.</param>
        /// <param name="littleEndian">
        /// If <see langword="true"/>, little endian, else <see langword="false"/> is big endian.
        /// </param>
        /// <param name="position">The position in the stream containing the error.</param>
        /// <exception cref="InvalidOperationException">Invalid Block for decoding.</exception>
        /// <remarks>
        /// This method is only used for checking in debug mode. The <see cref="BlockReader"/> should not call
        /// <see cref="GetSectionHeaderBlock(ReadOnlySpan{byte}, bool, long)"/> if these conditions are not already met.
        /// </remarks>
        [Conditional("DEBUG")]
        private static void CheckBlock(ReadOnlySpan<byte> buffer, bool littleEndian, long position)
        {
            // This code is for DEBUG mode only, and is used for consistency checking when running unit tests.
            int blockId = BitOperations.To32Shift(buffer, littleEndian);
            if (blockId != BlockCodes.InterfaceDescriptionBlock) {
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Interface Description Block offset 0x{0:x} with incorrect block identifier 0x{1:x}",
                    position, blockId);
                throw new InvalidOperationException("Invalid Block for decoding");
            }

            int blockLength = BitOperations.To32Shift(buffer[4..], littleEndian);
            if (buffer.Length != blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Interface Description Block offset 0x{0:x} invalid block length given", position);
                throw new InvalidOperationException("Invalid Block for decoding");
            }
        }

        private TimeResolutionOption m_TimeResolution;
        private readonly PacketDecoder m_Decoder;

        private InterfaceDescriptionBlock(int length, PcapOptions options, PacketDecoder decoder)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (decoder == null) throw new ArgumentNullException(nameof(decoder));

            Length = length;
            Options = options;
            Options.IsReadOnly = true;
            InitializeTimeResolution();

            if (Log.Pcap.ShouldTrace(TraceEventType.Information)) {
                string name = "'Unknown Name'";
                int nameIndex = Options.IndexOf(OptionCodes.IdbName);
                if (nameIndex != -1 && Options[nameIndex] is StringOption nameOption)
                    name = nameOption.Value;

                string resolution;
                if (m_TimeResolution.Multiplier == TimeResolutionOption.MultiplierMode.DecimalSi) {
                    resolution = $"10^-{m_TimeResolution.Value}";
                } else {
                    resolution = $"2^-{m_TimeResolution.Value}";
                }

                Log.Pcap.TraceEvent(TraceEventType.Information,
                    "Interface Description Block {0} with time resolution {1}", name, resolution);
            }

            m_Decoder = decoder;
        }

        private void InitializeTimeResolution()
        {
            int index = 0;
            do {
                index = Options.IndexOf(OptionCodes.IdbTsResolution, index);
                if (index >= 0) {
                    if (Options[index] is TimeResolutionOption timeResOption) {
                        m_TimeResolution = timeResOption;
                        return;
                    }

                    // Else, there is the option, but it wasn't decoded properly, so keep searching.
                } else {
                    m_TimeResolution = new TimeResolutionOption();
                    return;
                }

                index++;
            } while (true);
        }

        /// <summary>
        /// Gets the block identifier.
        /// </summary>
        /// <value>
        /// The block identifier.
        /// </value>
        public int BlockId { get { return BlockCodes.InterfaceDescriptionBlock; } }

        /// <summary>
        /// Gets the length of this block.
        /// </summary>
        /// <value>
        /// The length of this block.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets the link type used to decode the PCAP packet.
        /// </summary>
        /// <value>
        /// The link type.
        /// </value>
        public int LinkType { get; private set; }

        /// <summary>
        /// Gets the snap length used when recording packets.
        /// </summary>
        /// <value>
        /// The snap length used when recording packets.
        /// </value>
        public int SnapLength { get; private set; }

        /// <summary>
        /// Gets the options associated with this block.
        /// </summary>
        /// <value>
        /// The options associated with this block.
        /// </value>
        public PcapOptions Options { get; }

        /// <summary>
        /// Gets the time stamp from a PCAP-NG header field
        /// </summary>
        /// <param name="high">The high 32-bits of the time stamp.</param>
        /// <param name="low">The low 32-bits of the time stamp.</param>
        /// <returns>The converted <see cref="DateTime"/>.</returns>
        public DateTime GetTimeStamp(uint high, uint low)
        {
            ulong timeStamp = ((ulong)high << 32) + low;
            return m_TimeResolution.GetTimeStamp(timeStamp);
        }

        /// <summary>
        /// Decodes the packet, starting from the captured data (not the PCAP-NG block).
        /// </summary>
        /// <param name="buffer">
        /// The buffer containing the captured data for decoding DLT, without the PCAP-NG block data present.
        /// </param>
        /// <param name="timeStamp">The time stamp at when the packet was captured.</param>
        /// <param name="position">The position in the stream for the start of this data.</param>
        /// <returns></returns>
        public IEnumerable<DltTraceLineBase> DecodePacket(ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            return m_Decoder.DecodePacket(buffer, timeStamp, position);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed) {
                m_Decoder.Dispose();
                m_IsDisposed = true;
            }
        }
    }
}
