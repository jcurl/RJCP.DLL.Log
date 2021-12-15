﻿namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using Dlt;
    using RJCP.Core;

    /// <summary>
    /// The main decoder for AutoSAR DLT data based on Version 1 of the protocol.
    /// </summary>
    public abstract partial class DltTraceDecoderBase : ITraceDecoder<DltTraceLineBase>
    {
        private readonly LineCache m_Cache = new LineCache();
        private readonly IDltLineBuilder m_DltLineBuilder;
        private long m_SkippedBytes;

        private bool m_ValidHeaderFound = false;
        private int m_ExpectedLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderBase"/> class.
        /// </summary>
        /// <param name="lineBuilder">The line builder responsible for constructing each DLT line.</param>
        /// <exception cref="ArgumentNullException"><paramref name="lineBuilder"/> is <see langword="null"/>.</exception>
        protected DltTraceDecoderBase(IDltLineBuilder lineBuilder)
        {
            if (lineBuilder == null) throw new ArgumentNullException(nameof(lineBuilder));
            m_DltLineBuilder = lineBuilder;
        }

        private readonly List<DltTraceLineBase> m_Lines = new List<DltTraceLineBase>();

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// The <see cref="Decode"/> method shall accept any number of bytes for decoding. It should also consume all
        /// data that is received, so that data which is not processed is buffered locally by the decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            ReadOnlySpan<byte> decodeBuffer = buffer;
            m_Lines.Clear();

            int bytes = m_Cache.Length + buffer.Length;
            while (bytes > 0) {
                ReadOnlySpan<byte> dltPacket = decodeBuffer;
                if (!m_ValidHeaderFound) {
                    if (bytes < StandardHeaderOffset + 4) {
                        m_SkippedBytes += m_Cache.Append(decodeBuffer);
                        return m_Lines;
                    }

                    // Search for the initial marker defining the start of the DLT frame. If data is already cached, we
                    // append and search there.
                    bool found;
                    if (m_Cache.Length != 0) {
                        // TODO: We could optimize this logic by realising there's no need to add cache if all the data
                        // we need is in the original input `buffer`. Then we'd just set the `decodeBuffer` to the
                        // equivalent position in `buffer`, reset the cache, and start again. This could reduce a lot of
                        // copy operations between buffer and the cache in some cases.
                        decodeBuffer = CacheMinimumPacket(decodeBuffer, out _);
                        found = ScanStartFrame(m_Cache.GetCache(), out int skip);
                        if (skip > 0) {
                            bytes -= m_Cache.Consume(bytes);
                            m_SkippedBytes += skip;
                            decodeBuffer = CacheMinimumPacket(decodeBuffer, out bool isCached);
                            if (!isCached) return m_Lines;
                        }
                    } else {
                        found = ScanStartFrame(decodeBuffer, out int skip);
                        if (skip > 0) {
                            bytes -= skip;
                            m_SkippedBytes += skip;
                            decodeBuffer = decodeBuffer[skip..];

                            // We need the standard header before we know what is happening.
                            if (bytes < StandardHeaderOffset + 4) {
                                m_SkippedBytes += m_Cache.Append(decodeBuffer);
                                return m_Lines;
                            }
                        }
                    }
                    if (!found) continue;

                    // Get the start of the DLT packet. The offset zero is the beginning of the DLT packet.
                    if (m_Cache.Length == 0) {
                        dltPacket = decodeBuffer;
                    } else {
                        dltPacket = m_Cache.GetCache();
                    }

                    if (!IsValidPacket(dltPacket[StandardHeaderOffset..], out m_ExpectedLength)) {
                        bytes -= MinimumDiscard;
                        decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer);
                        continue;
                    }

                    m_ValidHeaderFound = true;
                }

                // If we don't have enough data, parsing doesn't make sense. Cache it and exit, waiting for more data.
                if (bytes < StandardHeaderOffset + m_ExpectedLength) {
                    // Cache all data until we have the complete packet
                    m_SkippedBytes += m_Cache.Append(decodeBuffer);
                    return m_Lines;
                }

                // Append all remaining data to the cache, so the complete packet is continuous before parsing. If the
                // data wasn't cached, then dltPacket already points to the start of the DLT packet.
                if (m_Cache.Length != 0) {
                    int restLength = StandardHeaderOffset + m_ExpectedLength - m_Cache.Length;
                    m_SkippedBytes += m_Cache.Append(decodeBuffer[0..restLength]);
                    decodeBuffer = decodeBuffer[restLength..];
                    dltPacket = m_Cache.GetCache();
                }

                if (!ParsePrefixHeader(dltPacket, m_DltLineBuilder) ||
                    !ParsePacket(dltPacket[StandardHeaderOffset..])) {
                    bytes -= MinimumDiscard;
                    decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer);
                    m_ValidHeaderFound = false;
                    continue;
                }

                if (m_SkippedBytes > 0) {
                    m_Lines.Add(m_DltLineBuilder.GetSkippedResult(null, m_SkippedBytes));
                    m_SkippedBytes = 0;
                }
                m_Lines.Add(m_DltLineBuilder.GetResult());
                m_DltLineBuilder.Reset();

                bytes -= StandardHeaderOffset + m_ExpectedLength;
                decodeBuffer = Consume(StandardHeaderOffset + m_ExpectedLength, decodeBuffer);
                m_ValidHeaderFound = false;
            }

            return m_Lines;
        }

        private ReadOnlySpan<byte> CacheMinimumPacket(ReadOnlySpan<byte> decodeBuffer, out bool isCached)
        {
            int minLen = StandardHeaderOffset + 4 - m_Cache.Length;
            if (minLen > 0) {
                if (decodeBuffer.Length >= minLen) {
                    m_SkippedBytes += m_Cache.Append(decodeBuffer[0..minLen]);
                    isCached = true;
                    return decodeBuffer[minLen..];
                }

                m_SkippedBytes += m_Cache.Append(decodeBuffer);
                isCached = false;
                return ReadOnlySpan<byte>.Empty;
            }

            isCached = true;
            return decodeBuffer;
        }

        private ReadOnlySpan<byte> SkipBytes(int skip, ReadOnlySpan<byte> decodeBuffer)
        {
            m_SkippedBytes += skip;
            return Consume(skip, decodeBuffer);
        }

        private ReadOnlySpan<byte> Consume(int bytes, ReadOnlySpan<byte> decodeBuffer)
        {
            if (bytes == 0) return decodeBuffer;
            bytes -= m_Cache.Consume(bytes);
            if (bytes > 0) return decodeBuffer[bytes..];
            return decodeBuffer;
        }

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
        protected abstract bool ScanStartFrame(ReadOnlySpan<byte> buffer, out int skip);

        /// <summary>
        /// Gets the offset to where the standard header starts once the start of a packet is found.
        /// </summary>
        /// <value>The offset to where the standard header starts, as defined by <see cref="ScanStartFrame"/>.</value>
        protected abstract int StandardHeaderOffset { get; }

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
        protected abstract int MinimumDiscard { get; }

        private static bool IsValidPacket(ReadOnlySpan<byte> standardHeader, out int length)
        {
            int headerType = standardHeader[0];

            if ((headerType & DltConstants.HeaderType.VersionIdentifierMask) != DltConstants.HeaderType.Version1) {
                length = 0;
                return false;
            }

            int minLength = 4;
            if ((headerType & DltConstants.HeaderType.WithEcuId) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.WithSessionId) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.WithTimeStamp) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.UseExtendedHeader) != 0) minLength += 10;

            length = BitOperations.To16ShiftBigEndian(standardHeader[2..4]);
            if (length < minLength) return false;
            return true;
        }

        /// <summary>
        /// Parses any headers at the start of the packet, which is dependent on the file format.
        /// </summary>
        /// <param name="dltPacket">
        /// The DLT packet where offset zero is the start of the packet found, including the marker as returned by
        /// <see cref="ScanStartFrame"/>.
        /// </param>
        /// <param name="lineBuilder">The line builder.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the prefix header was successfully parsed, else <see langword="false"/> if
        /// the packet is considered invalid and should be discarded.
        /// </returns>
        protected abstract bool ParsePrefixHeader(ReadOnlySpan<byte> dltPacket, IDltLineBuilder lineBuilder);

        private bool ParsePacket(ReadOnlySpan<byte> standardHeader)
        {
            int headerType = standardHeader[0];
            m_DltLineBuilder.SetBigEndian((headerType & DltConstants.HeaderType.MostSignificantByte) != 0);
            m_DltLineBuilder.SetCount(standardHeader[1]);

            int offset = 4;
            if ((headerType & DltConstants.HeaderType.WithEcuId) != 0) {
                int ecuid = BitOperations.To32ShiftBigEndian(standardHeader[offset..(offset + 4)]);
                m_DltLineBuilder.SetEcuId(IdHashList.Instance.ParseId(ecuid));
                offset += 4;
            }

            if ((headerType & DltConstants.HeaderType.WithSessionId) != 0) {
                int sessionId = BitOperations.To32ShiftBigEndian(standardHeader[offset..(offset + 4)]);
                m_DltLineBuilder.SetSessionId(sessionId);
                offset += 4;
            }

            if ((headerType & DltConstants.HeaderType.WithTimeStamp) != 0) {
                long ticks = unchecked((uint)BitOperations.To32ShiftBigEndian(standardHeader[offset..(offset + 4)])) *
                    DltConstants.DeviceTimeResolution;
                m_DltLineBuilder.SetDeviceTimeStamp(ticks);
                offset += 4;
            }

            if ((headerType & DltConstants.HeaderType.UseExtendedHeader) != 0) {
                int messageInfo = standardHeader[offset];

                m_DltLineBuilder.SetIsVerbose((messageInfo & DltConstants.MessageInfo.Verbose) != 0);

                DltType messageType = (DltType)(messageInfo & DltConstants.MessageInfo.MessageTypeMask);
                m_DltLineBuilder.SetDltType(messageType);

                int appid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 2)..(offset + 6)]);
                m_DltLineBuilder.SetApplicationId(IdHashList.Instance.ParseId(appid));

                int ctxid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 6)..(offset + 10)]);
                m_DltLineBuilder.SetContextId(IdHashList.Instance.ParseId(ctxid));

                // TODO: Set the number of arguments (NOAR) when we start parsing the arguments for verbose and
                // non-verbose messages.
            }

            return true;
        }

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Flush()
        {
            // TODO: We should parse through the cached buffer, as if it is the input buffer, but with the knowledge
            // that we're flushing, so that instead of caching a second time, it is added to the skipped data.

            return Array.Empty<DltTraceLineBase>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            /* Nothing to dispose in the base class */
        }
    }
}
