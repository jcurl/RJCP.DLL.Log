namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Interprets a block as bytes, converting it to a PCAP-NG block.
    /// </summary>
    /// <remarks>
    /// Supports reading a PCAP-NG block. This class maintains state while reading the blocks, for example the
    /// endianness of the file based on the last seen Section Header Block.
    /// </remarks>
    public sealed class BlockReader : IDisposable
    {
        private const int MinimumBlockSize = 12;
        private readonly List<InterfaceDescriptionBlock> m_Interfaces = new List<InterfaceDescriptionBlock>();
        private bool m_HasSectionHeader = false;
        private bool m_LittleEndian;
        private readonly ITraceDecoderFactory<DltTraceLineBase> m_TraceDecoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockReader"/> class.
        /// </summary>
        /// <param name="factory">The factory to return a <see cref="IPcapTraceDecoder"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
        public BlockReader(ITraceDecoderFactory<DltTraceLineBase> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);

            m_TraceDecoderFactory = factory;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="buffer">The buffer that must be at least 12 bytes long.</param>
        /// <returns>A <see cref="PcapBlock"/> with information about the parsed block.</returns>
        /// <remarks>
        /// If this is a Section Header Block, the block is read to obtain the endianness, and the length is correctly
        /// returned. For other blocks, a Section Header Block must have been decoded with the method
        /// <see cref="GetBlock(ReadOnlySpan{byte}, long)"/>.
        /// <para>
        /// If the <paramref name="buffer"/> is less than 12 bytes, a block is returned with identifer zero, and length
        /// of zero. This indicates an error.
        /// </para>
        /// </remarks>
        public PcapBlock GetHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < MinimumBlockSize) {
                // Not enough bytes, we don't know.
                return default;
            }

            bool littleEndian = m_LittleEndian;
            int blockId = BitOperations.To32Shift(buffer, littleEndian);
            if (blockId == BlockCodes.SectionHeaderBlock) {
                // This is a Section Header Block. Always get the endianness.
                if (!GetEndianness(buffer, out littleEndian)) return default;
            } else if (!m_HasSectionHeader) {
                // Must have at least seen the Section Header Block.
                return default;
            }
            int blockLength = BitOperations.To32Shift(buffer[4..], littleEndian);

            return new PcapBlock(blockId, blockLength);
        }

        private bool GetEndianness(ReadOnlySpan<byte> buffer, out bool littleEndian)
        {
            int byteOrder = BitOperations.To32ShiftBigEndian(buffer[8..]);
            switch (byteOrder) {
            case 0x1A2B3C4D:
                littleEndian = false;
                return true;
            case 0x4D3C2B1A:
                littleEndian = true;
                return true;
            default:
                littleEndian = m_LittleEndian;
                return false;
            }
        }

        /// <summary>
        /// Interprets the buffer and returns a block describing it.
        /// </summary>
        /// <param name="buffer">The buffer, that is expected to be complete.</param>
        /// <param name="position">The position in the input stream where the buffer is being read..</param>
        /// <returns>
        /// A <see cref="IPcapBlock"/>. The length can be used to advance the file stream pointer to start reading the
        /// next block. To determine what the size of the block should be while reading, use the
        /// <see cref="GetHeader(ReadOnlySpan{byte})"/> on the first 12 bytes, to know how much more data to read before
        /// calling this method.
        /// <para>
        /// When reading the block, if a Section Header Block is found, the endianness is read and this is used for
        /// decoding all future blocks with <see cref="GetBlock(ReadOnlySpan{byte}, long)"/> and
        /// <see cref="GetHeader(ReadOnlySpan{byte})"/>.
        /// </para>
        /// <para>If there was an error decoding the block, <see langword="null"/> is returned.</para>
        /// </returns>
        public IPcapBlock GetBlock(ReadOnlySpan<byte> buffer, long position)
        {
            if (buffer.Length < MinimumBlockSize) {
                // Not enough bytes, we don't know.
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block offset 0x{0:x} with buffer length too short (got {1}, expected >= 12)",
                    position, buffer.Length);
                return null;
            }

            bool littleEndian = m_LittleEndian;
            int blockId = BitOperations.To32Shift(buffer, littleEndian);
            if (blockId == BlockCodes.SectionHeaderBlock) {
                // This is a Section Header Block. Always get the endianness.
                if (!GetEndianness(buffer, out littleEndian)) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "PCAP Section Header Block offset 0x{0:x} corruption with invalid magic bytes", position);
                    return null;
                }
            } else if (!m_HasSectionHeader) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block offset 0x{0:x} found without a previous Section Header Block", position);
                return null;
            }

            int blockLength = BitOperations.To32Shift(buffer[4..], littleEndian);
            if (buffer.Length < blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} with buffer length less than PCAP block length (got {2}, needed {3})",
                    blockId, position, buffer.Length, blockLength);
                return null;
            }

            if (blockLength < MinimumBlockSize) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} corruption with block length too short (got {2}, needed {3})",
                    blockId, position, blockLength, MinimumBlockSize);
                return null;
            }

            int blockLengthEnd = BitOperations.To32Shift(buffer[(blockLength - 4)..], littleEndian);
            if (blockLengthEnd != blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} corruption with block length start and end mismatch (start {2}, end {3})",
                    blockId, position, blockLength, blockLengthEnd);
                return null;
            }

            IPcapBlock block = null;
            switch (blockId) {
            case BlockCodes.SectionHeaderBlock:
                Log.Pcap.TraceEvent(TraceEventType.Information,
                    "PCAP Block Section Header Block offset 0x{0:x} found with {1}",
                    position, littleEndian ? "little endian" : "big endian");
                block = SectionHeaderBlock.GetSectionHeaderBlock(buffer, littleEndian, position);
                if (block is object) {
                    m_HasSectionHeader = true;
                    m_LittleEndian = littleEndian;
                } else {
                    m_HasSectionHeader = false;
                }
                ClearInterfaces();
                break;
            case BlockCodes.InterfaceDescriptionBlock:
                block = InterfaceDescriptionBlock.GetInterfaceDescriptionBlock(buffer, littleEndian, m_TraceDecoderFactory, position);
                m_Interfaces.Add(block as InterfaceDescriptionBlock);
                break;
            }
            if (block is null)
                return new PcapBlock(blockId, blockLength);
            return block;
        }

        /// <summary>
        /// Decodes the block content, which should contain a network frame.
        /// </summary>
        /// <param name="buffer">The PCAP-NG block that contains the network frame.</param>
        /// <param name="position">The position in the stream where the block starts.</param>
        /// <returns>A collection of decoded DLT lines.</returns>
        /// <remarks>
        /// This method decodes an Enhanced Packet Block, then extracts the frame within it, then decodes the contents
        /// of that frame.
        /// </remarks>
        public IEnumerable<DltTraceLineBase> DecodeBlock(ReadOnlySpan<byte> buffer, long position)
        {
            if (!m_HasSectionHeader) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block offset 0x{0:x} found without a previous Section Header Block", position);
                return null;
            }

            if (buffer.Length < MinimumBlockSize) {
                // Not enough bytes, we don't know.
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block offset 0x{0:x} with buffer length too short (got {1}, expected >= 12)",
                    position, buffer.Length);
                return null;
            }

            int blockId = BitOperations.To32Shift(buffer, m_LittleEndian);

            int blockLength = BitOperations.To32Shift(buffer[4..], m_LittleEndian);
            if (buffer.Length < blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} with buffer length less than PCAP block length (got {2}, needed {3})",
                    blockId, position, buffer.Length, blockLength);
                return null;
            }

            if (blockLength < MinimumBlockSize) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} corruption with block length too short (got {2}, needed {3})",
                    blockId, position, blockLength, MinimumBlockSize);
                return null;
            }

            int blockLengthEnd = BitOperations.To32Shift(buffer[(blockLength - 4)..], m_LittleEndian);
            if (blockLengthEnd != blockLength) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "PCAP Block 0x{0:x} offset 0x{1:x} corruption with block length start and end mismatch (start {2}, end {3})",
                    blockId, position, blockLength, blockLengthEnd);
                return null;
            }

            switch (blockId) {
            case BlockCodes.EnhancedPacketBlock:
                if (blockLength < 32) break;
                int interfaceId = BitOperations.To32Shift(buffer[8..], m_LittleEndian);
                if (interfaceId < 0 || interfaceId >= m_Interfaces.Count) break;
                InterfaceDescriptionBlock intf = m_Interfaces[interfaceId];
                if (intf is null) break;

                int captured = BitOperations.To32Shift(buffer[20..], m_LittleEndian);
                if (captured > intf.SnapLength) break;
                if (blockLength < 32 + captured) break;

                uint tsHigh = unchecked((uint)BitOperations.To32Shift(buffer[12..], m_LittleEndian));
                uint tsLow = unchecked((uint)BitOperations.To32Shift(buffer[16..], m_LittleEndian));
                DateTime timeStamp = intf.GetTimeStamp(tsHigh, tsLow);
                return intf.DecodePacket(buffer[28..(28 + captured)], timeStamp, position + 28);
            }

            return Array.Empty<DltTraceLineBase>();
        }

        /// <summary>
        /// Resets the state of this instance.
        /// </summary>
        public void Reset()
        {
            m_HasSectionHeader = false;
            ClearInterfaces();
        }

        private void ClearInterfaces()
        {
            foreach (InterfaceDescriptionBlock idb in m_Interfaces) {
                if (idb is object) idb.Dispose();
            }
            m_Interfaces.Clear();
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed) {
                ClearInterfaces();
                m_IsDisposed = true;
            }
        }
    }
}
