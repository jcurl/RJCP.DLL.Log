namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Diagnostics;
    using RJCP.Core;

    /// <summary>
    /// Provide details from the Section Header Block
    /// </summary>
    public sealed class SectionHeaderBlock : IPcapBlock
    {
        /// <summary>
        /// Decode the buffer to get the <see cref="SectionHeaderBlock"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the complete block.</param>
        /// <param name="littleEndian">
        /// Decode using little endian if <see langword="true"/>, else big endian if <see langword="false"/>.
        /// </param>
        /// <param name="position">The position in the stream where the block starts.</param>
        /// <returns>The <see cref="SectionHeaderBlock"/>.</returns>
        /// <remarks>
        /// This block does not check the identifier (it must be the <see cref="BlockCodes.SectionHeaderBlock"/> or the
        /// length fields (which must be the same length as <paramref name="buffer"/>). It is assumed that the component
        /// calling this method has already checked the fields are correct (see <see cref="BlockReader"/>).
        /// </remarks>
        internal static SectionHeaderBlock GetSectionHeaderBlock(ReadOnlySpan<byte> buffer, bool littleEndian, long position)
        {
            if (buffer.Length < 28) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Section Header Block offset 0x{0:x} with buffer length too short (got {1}, expect >= 28)",
                    position, buffer.Length);
                return null;
            }

            CheckBlock(buffer, littleEndian, position);

            int major = BitOperations.To16Shift(buffer[12..], littleEndian);
            int minor = BitOperations.To16Shift(buffer[14..], littleEndian);
            if (major != 1 || minor != 0) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Section Header Block offset 0x{0:x} with unknown version {1}.{2}",
                    position, major, minor);
                return null;
            }

            PcapOptions options = new PcapOptions(littleEndian);
            int optionLength = options.Decode(BlockCodes.SectionHeaderBlock, buffer[24..^4]);
            if (optionLength == -1) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Section Header Block offset 0x{0:x} with corruption while decoding options", position);
                return null;
            }

            return new SectionHeaderBlock(buffer.Length, options) {
                IsLittleEndian = littleEndian,
                MajorVersion = major,
                MinorVersion = minor,
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
            int blockId = BitOperations.To32ShiftLittleEndian(buffer);
            if (blockId != BlockCodes.SectionHeaderBlock) {
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Section Header Block offset 0x{0:x} with incorrect block identifier 0x{1:x}",
                    position, blockId);
                throw new InvalidOperationException("Invalid Block for decoding");
            }

            bool checkLittleEndian;
            int byteOrder = BitOperations.To32ShiftBigEndian(buffer[8..]);
            switch (byteOrder) {
            case 0x1A2B3C4D:
                checkLittleEndian = false;
                break;
            case 0x4D3C2B1A:
                checkLittleEndian = true;
                break;
            default:
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Section Header Block offset 0x{0:x} corrupted byte magic 0x{1:x}",
                    position, byteOrder);
                throw new InvalidOperationException("Invalid Block for decoding");
            }

            if (checkLittleEndian != littleEndian) {
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Section Header Block offset 0x{0:x} invalid endianness given", position);
                throw new InvalidOperationException("Invalid Block for decoding");
            }

            int blockLength = BitOperations.To32Shift(buffer[4..], littleEndian);
            if (buffer.Length != blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Error,
                    "Section Header Block offset 0x{0:x} invalid block length given", position);
                throw new InvalidOperationException("Invalid Block for decoding");
            }
        }

        private SectionHeaderBlock(int length, PcapOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            Length = length;
            Options = options;
            Options.IsReadOnly = true;
        }

        /// <summary>
        /// Gets the block identifier.
        /// </summary>
        /// <value>
        /// The block identifier.
        /// </value>
        public int BlockId { get { return BlockCodes.SectionHeaderBlock; } }

        /// <summary>
        /// Gets the length of this block.
        /// </summary>
        /// <value>
        /// The length of this block.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets a value indicating whether all blocks following this one is little endian.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the magic byte define this and following blocks as little endian, else is
        /// <see langword="false"/> for big endian.
        /// </value>
        public bool IsLittleEndian { get; private set; }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>
        /// The major version.
        /// </value>
        public int MajorVersion { get; private set; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public int MinorVersion { get; private set; }

        /// <summary>
        /// Gets the options associated with this block.
        /// </summary>
        /// <value>
        /// The options associated with this block.
        /// </value>
        public PcapOptions Options { get; }
    }
}
