﻿namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Dlt;
    using Dlt.Control;
    using Dlt.NonVerbose;
    using Dlt.Verbose;
    using RJCP.Core;

    /// <summary>
    /// The main decoder for AutoSAR DLT data based on Version 1 of the protocol.
    /// </summary>
    public abstract partial class DltTraceDecoderBase : ITraceDecoder<DltTraceLineBase>
    {
        private readonly LineCache m_Cache = new();
        private readonly PosMap m_PosMap = new();
        private readonly IVerboseDltDecoder m_VerboseDecoder;
        private readonly INonVerboseDltDecoder m_NonVerboseDecoder;
        private readonly IControlDltDecoder m_ControlDecoder;
        private readonly IDltLineBuilder m_DltLineBuilder;

        private bool m_ValidHeaderFound;
        private int m_ExpectedLength;

        /// <summary>
        /// Gets the verbose decoder that should be used when instantiating this class.
        /// </summary>
        /// <returns>A <see cref="IVerboseDltDecoder"/>.</returns>
        protected static IVerboseDltDecoder GetVerboseDecoder()
        {
            return new VerboseDltDecoder(new VerboseArgDecoder());
        }

        /// <summary>
        /// Gets the non-verbose decoder that should be used when instantiating this class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        /// <returns>A <see cref="INonVerboseDltDecoder"/>.</returns>
        protected static INonVerboseDltDecoder GetNonVerboseDecoder(IFrameMap map)
        {
            if (map is null) return new NonVerboseByteDecoder();
            return new NonVerboseDltDecoder(map);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderBase"/> class.
        /// </summary>
        /// <remarks>
        /// Instantiate a trace decoder using recommended default decoders. This class must still be extended on how to
        /// find the start of a frame, and the size of data that can be skipped when searching for a frame.
        /// </remarks>
        protected DltTraceDecoderBase()
            : this(GetVerboseDecoder(), new NonVerboseByteDecoder(), new ControlDltDecoder(), new DltLineBuilder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderBase"/> class.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> used to decode non-verbose payloads.</param>
        /// <remarks>
        /// Instantiate a trace decoder using recommended default decoders. This class must still be extended on how to
        /// find the start of a frame, and the size of data that can be skipped when searching for a frame.
        /// </remarks>
        protected DltTraceDecoderBase(IFrameMap map)
            : this(GetVerboseDecoder(), GetNonVerboseDecoder(map), new ControlDltDecoder(), new DltLineBuilder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceDecoderBase"/> class.
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
        protected DltTraceDecoderBase(IVerboseDltDecoder verboseDecoder, INonVerboseDltDecoder nonVerboseDecoder, IControlDltDecoder controlDecoder, IDltLineBuilder lineBuilder)
        {
            ArgumentNullException.ThrowIfNull(verboseDecoder);
            ArgumentNullException.ThrowIfNull(nonVerboseDecoder);
            ArgumentNullException.ThrowIfNull(controlDecoder);
            ArgumentNullException.ThrowIfNull(lineBuilder);

            m_VerboseDecoder = verboseDecoder;
            m_NonVerboseDecoder = nonVerboseDecoder;
            m_ControlDecoder = controlDecoder;
            m_DltLineBuilder = lineBuilder;
        }

        private readonly List<DltTraceLineBase> m_Lines = new();

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// The <see cref="Decode(ReadOnlySpan{byte}, long)"/> method shall accept any number of bytes for decoding. It
        /// should also consume all data that is received, so that data which is not processed is buffered locally by
        /// the decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            return Decode(buffer, position, false);
        }

        /// <summary>
        /// Decodes data from the buffer, and any data that couldn't be decoded is flushed.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <param name="flush">If the conversion should be completed.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// The <see cref="Decode(ReadOnlySpan{byte}, long, bool)"/> method shall accept any number of bytes for decoding. It
        /// should also consume all data that is received, so that data which is not processed is buffered locally by
        /// the decoder.
        /// <para>
        /// If the <paramref name="flush"/> is <see langword="true"/>, then any data remaining in the buffer is also
        /// decoded, as if this is the last set of data in the stream. There will be no data remaining in the cache
        /// after this call.
        /// </para>
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position, bool flush)
        {
            m_Lines.Clear();
            if (!flush) return DecodeStream(buffer, position, false);

            // First decode using any data that might still be in the cache.
            DecodeStream(buffer, position, false);

            // Now we flush data that remains in the cache.
            if (!m_Cache.IsCached) {
                if (m_DltLineBuilder.SkippedBytes != 0)
                    m_Lines.Add(m_DltLineBuilder.GetSkippedResult());

                return m_Lines;
            }

            ReadOnlySpan<byte> cache = m_Cache.SetFlush();
            IEnumerable<DltTraceLineBase> lines = DecodeStream(cache, 0, true);
            m_Cache.Unlock();
            return lines;
        }

        /// <summary>
        /// Gets the trace level based on previous errors.
        /// </summary>
        /// <param name="baseLevel">The base level for the first event that is logged.</param>
        /// <returns>The trace level that should be used.</returns>
        /// <remarks>
        /// Often after one single error, we will get a lot of follow up errors. To reduce the log levels to help make
        /// it easier to debug input streams, log the level in case no known previous errors have occurred, or log it as
        /// verbose (which should normally not be enabled).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TraceEventType TraceLevel(TraceEventType baseLevel)
        {
            return m_DltLineBuilder.SkippedBytes == 0 ? baseLevel : TraceEventType.Verbose;
        }

        private IEnumerable<DltTraceLineBase> DecodeStream(ReadOnlySpan<byte> buffer, long position, bool flush)
        {
            // When flushing, there should be no access to the m_Cache variable. It is expected that the user
            // provides the buffer from m_Cache when flushing.

            ReadOnlySpan<byte> decodeBuffer = buffer;

            int bytes = buffer.Length;
            if (!flush) {
                m_PosMap.Append(position, bytes);
                bytes += m_Cache.CacheLength;
                m_Cache.Write();
            }

            while (bytes > 0) {
                ReadOnlySpan<byte> dltPacket = decodeBuffer;
                if (!m_ValidHeaderFound) {
                    if (bytes < StandardHeaderOffset + 4) {
                        if (flush) {
                            m_DltLineBuilder.AddSkippedBytes(bytes, m_PosMap.Position, "End of stream");
                            m_PosMap.Consume(bytes);
                            AddSkippedLine();
                            return m_Lines;
                        }
                        AppendFinal(decodeBuffer);
                        return m_Lines;
                    }

                    // Search for the initial marker defining the start of the DLT frame. If data is already cached, we
                    // append and search there.
                    bool found;
                    if (m_Cache.IsCached) {
                        // Put the smallest amount of data into the cache, that if we find the start marker, and it
                        // happens to be at the start, we already have the packet.
                        decodeBuffer = CacheMinimumPacket(decodeBuffer, out bool success);
                        if (!success) return m_Lines;

                        found = ScanStartFrame(m_Cache.GetCache(), out int skip);
                        if (skip > 0) {
                            m_DltLineBuilder.AddSkippedBytes(skip, m_PosMap.Position, "Searching for next packet");
                            m_PosMap.Consume(skip);
                            bytes -= m_Cache.Consume(skip);
                            if (m_Cache.CacheWriteOffset >= 0) {
                                // We've consumed enough data from the cache, that, even though there might still be
                                // data in the cache, it's a subset of the original buffer, and we should use that
                                // instead. If we continue to use the cache, we'll end up copying most of what's in the
                                // buffer into the cache just because the data didn't align properly to start with.
                                decodeBuffer = buffer[m_Cache.CacheWriteOffset..];
                                m_Cache.Clear();

                                // The variable m_Cache.CacheWriteOffset is not reset, and remains as it was. However,
                                // as the m_Cache has been reset (it is now empty), we'll never get back here from
                                // within the loop. Data is added to the cache only shortly before this decode function
                                // is exited. The `continue` saves us having to check if we have enough data, and we'll
                                // check at the beginning of the loop, and if there isn't enough remaining, we exit.
                                continue;
                            } else {
                                decodeBuffer = CacheMinimumPacket(decodeBuffer, out success);
                                if (!success) return m_Lines;
                            }
                        }
                    } else {
                        found = ScanStartFrame(decodeBuffer, out int skip);
                        if (skip > 0) {
                            m_DltLineBuilder.AddSkippedBytes(skip, m_PosMap.Position, "Searching for next packet");
                            bytes -= skip;
                            m_PosMap.Consume(skip);
                            decodeBuffer = decodeBuffer[skip..];

                            // We need the standard header before we know what is happening.
                            if (bytes < StandardHeaderOffset + 4) {
                                if (flush) {
                                    m_DltLineBuilder.AddSkippedBytes(bytes, m_PosMap.Position, "End of stream");
                                    AddSkippedLine();
                                    return m_Lines;
                                }
                                AppendFinal(decodeBuffer);
                                return m_Lines;
                            }
                        }
                    }
                    if (!found) continue;

                    // Get the start of the DLT packet. The offset zero is the beginning of the DLT packet.
                    if (m_Cache.IsCached) {
                        dltPacket = m_Cache.GetCache();
                    } else {
                        dltPacket = decodeBuffer;
                    }

                    if (!IsValidPacket(dltPacket[StandardHeaderOffset..], out m_ExpectedLength)) {
                        bytes -= MinimumDiscard;
                        decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer, flush, "Invalid packet standard header");
                        continue;
                    }

                    m_ValidHeaderFound = true;
                }

                // If we don't have enough data, parsing doesn't make sense. Cache it and exit, waiting for more data.
                if (bytes < StandardHeaderOffset + m_ExpectedLength) {
                    // Cache all data until we have the complete packet
                    if (flush) {
                        // In case we don't have enough data, we assume that we might have corruption that the length
                        // could be incorrect, and hence look for the next packet. This may result in additional packets
                        // at the end of the stream if data within a packet could be interpreted as a new DLT packet.
                        bytes -= MinimumDiscard;
                        decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer, true, "Flushing packets at end of stream");
                        m_ValidHeaderFound = false;
                        continue;
                    }
                    AppendFinal(decodeBuffer);
                    return m_Lines;
                }

                // Append all remaining data to the cache, so the complete packet is continuous before parsing. If the
                // data wasn't cached, then dltPacket already points to the start of the DLT packet.
                if (m_Cache.IsCached) {
                    int restLength = StandardHeaderOffset + m_ExpectedLength - m_Cache.CacheLength;
                    if (restLength > 0) {
                        m_Cache.Append(decodeBuffer[0..restLength]);
                        decodeBuffer = decodeBuffer[restLength..];
                    }
                    dltPacket = m_Cache.GetCache();
                }

                if (!ParsePrefixHeader(dltPacket, m_DltLineBuilder) ||
                    !ParsePacket(dltPacket[StandardHeaderOffset..(StandardHeaderOffset + m_ExpectedLength)])) {
                    // We need to keep the position, as a reset will set it to zero. We don't know if the next packet
                    // will be valid, or more skipped data.
                    m_DltLineBuilder.Reset();
                    bytes -= MinimumDiscard;
                    decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer, flush, "Invalid packet");
                    m_ValidHeaderFound = false;
                    continue;
                }

                AddSkippedLine();
                m_DltLineBuilder.SetPosition(m_PosMap.Position);

                int packetLen = StandardHeaderOffset + m_ExpectedLength;
                DltTraceLineBase line = m_DltLineBuilder.GetResult();
                if (CheckLine(line, dltPacket[..packetLen]))
                    m_Lines.Add(line);
                m_DltLineBuilder.Reset();

                bytes -= packetLen;
                decodeBuffer = Consume(packetLen, decodeBuffer, flush);
                m_ValidHeaderFound = false;
            }

            if (flush) AddSkippedLine();
            return m_Lines;
        }

        private void AppendFinal(ReadOnlySpan<byte> decodeBuffer)
        {
            m_Cache.Append(decodeBuffer);

#if DEBUG
            // This method appends all remaining data into the cache. It is expected that the PosMap contains the same
            // length of data as the cache.
            if (m_PosMap.Length != m_Cache.CacheLength) {
                string message = string.Format("Internal error, position map length {0} doesn't match cached length {1}",
                    m_PosMap.Length, m_Cache.CacheLength);
                throw new InvalidOperationException(message);
            }
#endif
        }

        private ReadOnlySpan<byte> CacheMinimumPacket(ReadOnlySpan<byte> decodeBuffer, out bool success)
        {
            int minPacketSize = StandardHeaderOffset + 4;
            int cached = minPacketSize - m_Cache.CacheLength;
            if (cached <= 0) {
                // There is already sufficient data in the packet cache.
                success = true;
                return decodeBuffer;
            }

            if (decodeBuffer.Length < cached) {
                // The input buffer does not have enough data, nor does our cache, so append everything, and indicate
                // that it's incomplete but setting `success` to `false`.
                success = false;
                m_Cache.Append(decodeBuffer);
                return ReadOnlySpan<byte>.Empty;
            }

            success = true;
            m_Cache.Append(decodeBuffer[0..cached]);
            return decodeBuffer[cached..];
        }

        private ReadOnlySpan<byte> SkipBytes(int skip, ReadOnlySpan<byte> decodeBuffer, bool flush, string reason)
        {
            if (skip == 0) return decodeBuffer;
            m_DltLineBuilder.AddSkippedBytes(skip, m_PosMap.Position, reason);

            return Consume(skip, decodeBuffer, flush);
        }

        private ReadOnlySpan<byte> Consume(int bytes, ReadOnlySpan<byte> decodeBuffer, bool flush)
        {
            if (bytes == 0) return decodeBuffer;

            m_PosMap.Consume(bytes);

            if (!flush) {
                bytes -= m_Cache.Consume(bytes);
                if (bytes > 0) return decodeBuffer[bytes..];
                return decodeBuffer;
            }

            return decodeBuffer[bytes..];
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

        private bool IsValidPacket(ReadOnlySpan<byte> standardHeader, out int length)
        {
            int headerType = standardHeader[0];

            int version = headerType & DltConstants.HeaderType.VersionIdentifierMask;
            if (version != DltConstants.HeaderType.Version1) {
                length = 0;
                Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Packet offset 0x{0:x} with version {1} found, expected {2}",
                    m_PosMap.Position, version >> DltConstants.HeaderType.VersionBitShift,
                    DltConstants.HeaderType.Version1 >> DltConstants.HeaderType.VersionBitShift);
                return false;
            }

            int minLength = 4;
            if ((headerType & DltConstants.HeaderType.WithEcuId) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.WithSessionId) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.WithTimeStamp) != 0) minLength += 4;
            if ((headerType & DltConstants.HeaderType.UseExtendedHeader) != 0) minLength += 10;

            length = unchecked((ushort)BitOperations.To16ShiftBigEndian(standardHeader[2..4]));
            if (length < minLength) {
                Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning),
                    "Packet offset 0x{0:x} found with length {1}, expected minimum {2}",
                    m_PosMap.Position, length, minLength);
                return false;
            }
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

            bool isVerbose = false;
            if ((headerType & DltConstants.HeaderType.UseExtendedHeader) != 0) {
                int messageInfo = standardHeader[offset];

                isVerbose = (messageInfo & DltConstants.MessageInfo.Verbose) != 0;
                bool isControl = (messageInfo & DltConstants.MessageInfo.MessageTypeMaskMstp) == DltConstants.MessageInfo.MessageTypeControl;
                m_DltLineBuilder.SetIsVerbose(isVerbose);

                if (isVerbose || isControl) {
                    DltType messageType = GetMessageType(messageInfo);
                    if (messageType == DltType.UNKNOWN) {
                        Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Packet at offset 0x{0:x} has invalid message info {1:x2}",
                            m_PosMap.Position, messageInfo);
                        return false;
                    }
                    m_DltLineBuilder.SetDltType(messageType);

                    byte noar = standardHeader[offset + 1];
                    m_DltLineBuilder.SetNumberOfArgs(noar);
                }

                int appid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 2)..(offset + 6)]);
                m_DltLineBuilder.SetApplicationId(IdHashList.Instance.ParseId(appid));

                int ctxid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 6)..(offset + 10)]);
                m_DltLineBuilder.SetContextId(IdHashList.Instance.ParseId(ctxid));

                offset += 10;

                // A control message can only be present when there's an extended header.
                if (isControl) {
                    Result<int> result = m_ControlDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                    if (!result.TryGet(out int controlLength)) {
                        string error = m_DltLineBuilder.ResetErrorMessage();
                        if (string.IsNullOrEmpty(error)) {
                            Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Control packet at offset 0x{0:x} cannot be decoded",
                                m_PosMap.Position);
                        } else {
                            Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Control packet at offset 0x{0:x} cannot be decoded, {1}",
                                m_PosMap.Position, error);
                        }
                        return false;
                    }
                    if (m_ExpectedLength < offset + controlLength || m_ExpectedLength > offset + controlLength + 32) {
                        Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Control packet at offset 0x{0:x} expected payload length {1}, decoded {2}",
                            m_PosMap.Position, m_ExpectedLength - offset, controlLength);
                        return false;
                    }
                    return true;
                }
            }

            if (isVerbose) {
                // Observed as a common error in DLT logs. The provider of DLT logs specifies a NOAR that is non-zero,
                // but we see that the length of the packet doesn't allow for arguments. The Covesa DLT viewer would
                // treat this as a valid packet and ignore the NOAR. WireShark can decode the packet, but shows a
                // decoding error. If we don't do this, we might end up losing more data than expected (trying to sync
                // with the next packet could lose many more as it tries to find the next header based on heuristics).
                if (offset == standardHeader.Length) {
                    if (m_DltLineBuilder.NumberOfArgs > 0) {
                        Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Verbose packet at offset 0x{0:x} cannot be decoded. NOAR > 0 and no arguments available.",
                            m_PosMap.Position);
                    }

                    // Indicate, despite the error, that we were able to decode the packet. Else it would skip a single
                    // byte and start trying to decode using heuristics which is unrelaiable.
                    return true;
                }

                Result<int> result = m_VerboseDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                if (!result.TryGet(out int payloadLength)) {
                    string error = m_DltLineBuilder.ResetErrorMessage();
                    if (string.IsNullOrEmpty(error)) {
                        Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Verbose packet at offset 0x{0:x} cannot be decoded",
                            m_PosMap.Position);
                    } else {
                        Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Verbose packet at offset 0x{0:x} cannot be decoded, {1}",
                            m_PosMap.Position, error);
                    }
                    return false;
                }
                if (m_ExpectedLength != offset + payloadLength) {
                    Log.Dlt.TraceEvent(TraceLevel(TraceEventType.Warning), "Verbose packet at offset 0x{0:x} expected payload length {1}, decoded {2}",
                        m_PosMap.Position, m_ExpectedLength - offset, payloadLength);
                    return false;
                }
            } else {
                Result<int> result = m_NonVerboseDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                if (!result.TryGet(out int payloadLength) || m_DltLineBuilder.HasErrorMessage()) {
                    string error = m_DltLineBuilder.ResetErrorMessage();
                    if (result.HasValue) {
                        if (!string.IsNullOrEmpty(error)) {
                            Log.DltNonVerbose.TraceEvent(TraceEventType.Verbose,
                                "Non-verbose packet at offset 0x{0:x}, {1}",
                                m_PosMap.Position, error);
                        }
                        return true;
                    }
                    bool decoded = ParseNonVerboseFallback(standardHeader[offset..]);
                    if (decoded) {
                        Log.DltNonVerbose.TraceEvent(TraceEventType.Verbose,
                            "Non-verbose packet at offset 0x{0:x}, using fallback, {1}",
                            m_PosMap.Position, error);
                        return true;
                    }
                    Log.DltNonVerbose.TraceEvent(TraceLevel(TraceEventType.Warning),
                        "Non-verbose packet at offset 0x{0:x}, {1}",
                        m_PosMap.Position, error);
                    return false;
                }
                if (m_ExpectedLength != offset + payloadLength) {
                    Log.DltNonVerbose.TraceEvent(TraceLevel(TraceEventType.Information),
                        "Non-verbose packet at offset 0x{0:x} expected payload length {1}, decoded {2}, using fallback",
                        m_PosMap.Position, m_ExpectedLength - offset, payloadLength);
                    m_DltLineBuilder.ResetArguments();
                    return ParseNonVerboseFallback(standardHeader[offset..]);
                }
            }

            return true;
        }

        private bool ParseNonVerboseFallback(ReadOnlySpan<byte> buffer)
        {
            if (m_NonVerboseDecoder.Fallback is null) return false;

            Result<int> payloadLengthResult = m_NonVerboseDecoder.Fallback.Decode(buffer, m_DltLineBuilder);
            return payloadLengthResult.HasValue;
        }

        private static DltType GetMessageType(int messageInfo)
        {
            int mtin = messageInfo & DltConstants.MessageInfo.MessageTypeMaskMtin;
            if (mtin == 0) return DltType.UNKNOWN;

            int mstp = messageInfo & DltConstants.MessageInfo.MessageTypeMaskMstp;
            switch (mstp) {
            case DltConstants.MessageInfo.MessageTypeLog:
            case DltConstants.MessageInfo.MessageTypeAppTrace:
            case DltConstants.MessageInfo.MessageTypeNwTrace:
                break;
            case DltConstants.MessageInfo.MessageTypeControl:
                if (mtin > (3 << DltConstants.MessageInfo.MessageTypeMaskMtinShift))
                    return DltType.UNKNOWN;
                break;
            default:
                return DltType.UNKNOWN;
            }

            return (DltType)(messageInfo & DltConstants.MessageInfo.MessageTypeInfoMask);
        }

        /// <summary>
        /// Checks the line before adding to the list of data that can be parsed.
        /// </summary>
        /// <param name="line">The line that should be checked.</param>
        /// <param name="packet">The raw packet data.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the line should be added, <see langword="false"/> otherwise.
        /// </returns>
        protected virtual bool CheckLine(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            return true;
        }

        private void AddSkippedLine()
        {
            if (m_DltLineBuilder.SkippedBytes > 0) {
                DltTraceLineBase line = m_DltLineBuilder.GetSkippedResult();
                if (CheckSkippedLine(line))
                    m_Lines.Add(line);
                Log.Dlt.TraceEvent(TraceEventType.Information, $"{line.Text} (offset {line.Position})");
            }
        }

        /// <summary>
        /// Checks the skipped line before adding to the list of data that can be parsed.
        /// </summary>
        /// <param name="line">The skipped line that should be checked.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the line should be added, <see langword="false"/> otherwise.
        /// </returns>
        protected virtual bool CheckSkippedLine(DltTraceLineBase line)
        {
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
            if (!m_Cache.IsCached) {
                if (m_DltLineBuilder.SkippedBytes == 0)
                    return Array.Empty<DltTraceLineBase>();

                return new DltTraceLineBase[] { m_DltLineBuilder.GetSkippedResult() };
            }

            m_Lines.Clear();
            ReadOnlySpan<byte> buffer = m_Cache.SetFlush();
            return DecodeStream(buffer, 0, true);
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
