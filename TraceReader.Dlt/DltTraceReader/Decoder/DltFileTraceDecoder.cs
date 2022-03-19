namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using Dlt;
    using Dlt.Control;
    using Dlt.NonVerbose;
    using Dlt.Verbose;
    using RJCP.Core;

    /// <summary>
    /// Decodes a DLT frame from the definition of a storage header.
    /// </summary>
    public class DltFileTraceDecoder : DltTraceDecoderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceDecoder"/> class.
        /// </summary>
        public DltFileTraceDecoder() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceDecoder"/> class.
        /// </summary>
        /// <param name="verboseDecoder">The object that knows how to decode verbose payloads.</param>
        /// <param name="nonVerboseDecoder">The object that knows how to decode non-verbose payloads.</param>
        /// <param name="controlDecoder">The object that knows how to decode control payloads.</param>
        /// <param name="lineBuilder">The line builder responsible for constructing each DLT line.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="verboseDecoder"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="nonVerboseDecoder"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="controlDecoder"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="lineBuilder"/> is <see langword="null"/>
        /// </exception>
        public DltFileTraceDecoder(IVerboseDltDecoder verboseDecoder, INonVerboseDltDecoder nonVerboseDecoder, IControlDltDecoder controlDecoder, IDltLineBuilder lineBuilder)
            : base(verboseDecoder, nonVerboseDecoder, controlDecoder, lineBuilder) { }

        private readonly static byte[] marker = { 0x44, 0x4C, 0x54, 0x01 };

        /// <summary>
        /// Searches for the start of the DLT frame.
        /// </summary>
        /// <param name="buffer">The buffer to search for.</param>
        /// <param name="skip">The amount of bytes to skip.</param>
        /// <returns>If the start of the frame was identified and scanning the packet can start.</returns>
        /// <remarks>
        /// Searching for the start of the frame depends on the format of the frame. For example, there might not be a
        /// start (in which case this method always returns 0, indicating that the current position may be the start of
        /// the frame), or where the frame start string is found.
        /// <para>
        /// When searching, it must return the amount of data that should be skipped. Skipped data may be all the data
        /// in the buffer that can be discarded that occurs before the marker. If no marker is found, then skipped data
        /// must return the number of bytes that may be discarded, with the assumption that scanning will occur again.
        /// For example, let's say we're looking for the marker "DLT\1" and there are 50 bytes, of which the marker is
        /// not found. The amount of data that can be skipped is 47 bytes at a minimum, if the end of the buffer looks
        /// like it may be the start of a frame but is incomplete (e.g. "DLT" but the 0x01 is missing).
        /// </para>
        /// </remarks>
        protected override bool ScanStartFrame(ReadOnlySpan<byte> buffer, out int skip)
        {
            // This method is called so that the number of bytes `buffer.Length` is more than `marker.Length`, because
            // the value of `StandardHeaderOffset` is more than `marker.Length`. Thus, we don't do boundary checks in
            // this code.

            int fi = marker.Length - 1;
            int bi = marker.Length - 1;
            int nbi = bi;

            do {
                if (buffer[bi] == marker[fi]) {
                    if (fi == 0) {
                        skip = bi;
                        return true;
                    }
                    fi--;
                    bi--;
                } else {
                    // The increment makes the assumption that there are no characters repeating in the search frame
                    // 'frame'. It then jumps based on the number of bytes actually compared. If there are repeating
                    // characters in the frame, then the most optimal increment is much more complex to calculate.
                    // Incrementing by only one byte is the safest (and slower).
                    nbi += marker.Length - fi;
                    fi = marker.Length - 1;
                    bi = nbi;
                }
            } while (bi < buffer.Length);

            // Check the end of the buffer, looking for a partial match for the marker. For example, we might be
            // scanning the buffer "xxxxxDLT", in which case we want to discard the first 5 bytes, but keep the last 3
            // in case the 4th byte (which we don't know about yet) is the match we need.
            bi = buffer.Length - marker.Length + 1;
            fi = 0;
            do {
                if (buffer[bi] == marker[fi]) {
                    fi++;
                } else {
                    fi = 0;
                }
                bi++;
            } while (bi < buffer.Length);
            skip = buffer.Length - fi;
            return false;
        }

        /// <summary>
        /// Gets the offset to where the standard header starts once the start of a packet is found.
        /// </summary>
        /// <value>The offset to where the standard header starts, as defined by <see cref="ScanStartFrame"/>.</value>
        /// <remarks>The start of a DLT protocol frame is the standard header, so the offset is always zero.</remarks>
        protected override int StandardHeaderOffset { get { return 16; } }

        /// <summary>
        /// Gets the number of bytes that should be discarded in case of an invalid packet.
        /// </summary>
        /// <value>
        /// The minimum number of bytes to discard in case of an invalid packet, assuming the start of the frame has
        /// been identified.
        /// </value>
        /// <remarks>
        /// The minimum bytes to discard depends on the size of the marker that <see cref="ScanStartFrame"/> is
        /// searching for. If there is no marker, then the minimum amount of bytes to skip should be 1, else it should
        /// be the size of the marker.
        /// </remarks>
        protected override int MinimumDiscard { get { return 4; } }

        /// <summary>
        /// Parses any headers at the start of the packet, which is dependent on the file format.
        /// </summary>
        /// <param name="dltPacket">
        /// The DLT packet where offset zero is the start of the packet found, including the marker as returned by
        /// <see cref="ScanStartFrame"/>.
        /// </param>
        /// <param name="lineBuilder">The line builder.</param>
        /// <returns>
        /// Returns <see langword="true"/> always, as there is no prefix header that needs to be parsed.
        /// </returns>
        protected override bool ParsePrefixHeader(ReadOnlySpan<byte> dltPacket, IDltLineBuilder lineBuilder)
        {
            int seconds = BitOperations.To32ShiftLittleEndian(dltPacket[4..8]);
            int microsec = BitOperations.To32ShiftLittleEndian(dltPacket[8..12]);
            int ecuid = BitOperations.To32ShiftBigEndian(dltPacket[12..16]);
            lineBuilder.SetStorageHeaderEcuId(IdHashList.Instance.ParseId(ecuid));
            lineBuilder.SetTimeStamp(seconds, microsec);
            return true;
        }
    }
}
