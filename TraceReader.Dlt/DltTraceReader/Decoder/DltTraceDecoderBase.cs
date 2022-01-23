namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        private readonly LineCache m_Cache = new LineCache();
        private readonly IVerboseDltDecoder m_VerboseDecoder;
        private readonly INonVerboseDltDecoder m_NonVerboseDecoder;
        private readonly IControlDltDecoder m_ControlDecoder;
        private readonly IDltLineBuilder m_DltLineBuilder;

        private bool m_ValidHeaderFound = false;
        private int m_ExpectedLength;

        /// <summary>
        /// Gets the verbose decoder that should be used when instantiating this class.
        /// </summary>
        /// <returns></returns>
        protected static IVerboseDltDecoder GetVerboseDecoder()
        {
            return new VerboseDltDecoder(new VerboseArgDecoder());
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
            if (verboseDecoder == null) throw new ArgumentNullException(nameof(verboseDecoder));
            if (nonVerboseDecoder == null) throw new ArgumentNullException(nameof(nonVerboseDecoder));
            if (controlDecoder == null) throw new ArgumentNullException(nameof(controlDecoder));
            if (lineBuilder == null) throw new ArgumentNullException(nameof(lineBuilder));

            m_VerboseDecoder = verboseDecoder;
            m_NonVerboseDecoder = nonVerboseDecoder;
            m_ControlDecoder = controlDecoder;
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

        private IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position, bool flush)
        {
            // When flushing, there should be no access to the m_Cache variable. It is expected that the user
            // provides the buffer from m_Cache when flushing.

            ReadOnlySpan<byte> decodeBuffer = buffer;
            m_Lines.Clear();

            int bytes = buffer.Length;
            if (!flush) {
                bytes += m_Cache.CacheLength;
                m_Cache.Write();
            }

            while (bytes > 0) {
                ReadOnlySpan<byte> dltPacket = decodeBuffer;
                if (!m_ValidHeaderFound) {
                    if (bytes < StandardHeaderOffset + 4) {
                        if (flush) {
                            m_DltLineBuilder.AddSkippedBytes(bytes, "End of stream");
                            if (m_DltLineBuilder.SkippedBytes > 0)
                                m_Lines.Add(m_DltLineBuilder.GetSkippedResult());
                            return m_Lines;
                        }
                        m_Cache.Append(decodeBuffer);
                        return m_Lines;
                    }

                    // Search for the initial marker defining the start of the DLT frame. If data is already cached, we
                    // append and search there.
                    bool found;
                    if (!flush && m_Cache.IsCached) {
                        // Put the smallest amount of data into the cache, that if we find the start marker, and it
                        // happens to be at the start, we already have the packet.
                        decodeBuffer = CacheMinimumPacket(decodeBuffer, out _);
                        found = ScanStartFrame(m_Cache.GetCache(), out int skip);
                        if (skip > 0) {
                            bytes -= m_Cache.Consume(skip);
                            m_DltLineBuilder.AddSkippedBytes(skip, "Searching for next packet");
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
                                decodeBuffer = CacheMinimumPacket(decodeBuffer, out bool success);
                                if (!success) return m_Lines;
                            }
                        }
                    } else {
                        found = ScanStartFrame(decodeBuffer, out int skip);
                        if (skip > 0) {
                            bytes -= skip;
                            m_DltLineBuilder.AddSkippedBytes(skip, "Searching for next packet");
                            decodeBuffer = decodeBuffer[skip..];

                            // We need the standard header before we know what is happening.
                            if (bytes < StandardHeaderOffset + 4) {
                                if (flush) {
                                    m_DltLineBuilder.AddSkippedBytes(bytes, "End of stream");
                                    if (m_DltLineBuilder.SkippedBytes > 0)
                                        m_Lines.Add(m_DltLineBuilder.GetSkippedResult());
                                    return m_Lines;
                                }
                                m_Cache.Append(decodeBuffer);
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
                        decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer, flush, "Incomplete packet at end of stream");
                        continue;
                    }
                    m_Cache.Append(decodeBuffer);
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
                    m_DltLineBuilder.Reset();
                    bytes -= MinimumDiscard;
                    decodeBuffer = SkipBytes(MinimumDiscard, decodeBuffer, flush, "Invalid packet");
                    m_ValidHeaderFound = false;
                    continue;
                }

                if (m_DltLineBuilder.SkippedBytes > 0) {
                    m_Lines.Add(m_DltLineBuilder.GetSkippedResult());
                }
                m_Lines.Add(m_DltLineBuilder.GetResult());
                m_DltLineBuilder.Reset();

                bytes -= StandardHeaderOffset + m_ExpectedLength;
                decodeBuffer = Consume(StandardHeaderOffset + m_ExpectedLength, decodeBuffer);
                m_ValidHeaderFound = false;
            }

            if (flush && m_DltLineBuilder.SkippedBytes > 0)
                m_Lines.Add(m_DltLineBuilder.GetSkippedResult());
            return m_Lines;
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
            m_DltLineBuilder.AddSkippedBytes(skip, reason);

            if (flush) return decodeBuffer[skip..];
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

            int version = headerType & DltConstants.HeaderType.VersionIdentifierMask;
            if (version != DltConstants.HeaderType.Version1) {
                length = 0;
                Log.Dlt.TraceEvent(TraceEventType.Warning, "Packet version {0} found, expected {1}",
                    version >> DltConstants.HeaderType.VersionBitShift,
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
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "Packet with length {0} found, expected minimum {1}", length, minLength);
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

            if ((headerType & DltConstants.HeaderType.UseExtendedHeader) != 0) {
                int messageInfo = standardHeader[offset];

                m_DltLineBuilder.SetIsVerbose((messageInfo & DltConstants.MessageInfo.Verbose) != 0);

                int messageType = messageInfo & DltConstants.MessageInfo.MessageTypeInfoMask;
                m_DltLineBuilder.SetDltType((DltType)messageType);

                byte noar = standardHeader[offset + 1];
                m_DltLineBuilder.SetNumberOfArgs(noar);

                int appid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 2)..(offset + 6)]);
                m_DltLineBuilder.SetApplicationId(IdHashList.Instance.ParseId(appid));

                int ctxid = BitOperations.To32ShiftBigEndian(standardHeader[(offset + 6)..(offset + 10)]);
                m_DltLineBuilder.SetContextId(IdHashList.Instance.ParseId(ctxid));

                offset += 10;

                // A control message can only be present when there's an extended header.
                if ((messageType & DltConstants.MessageInfo.MessageTypeMask) == DltConstants.MessageInfo.MessageTypeControl) {
                    int controlLength = m_ControlDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                    if (controlLength == -1) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "Control payload cannot be decoded");
                        return false;
                    }
                    if (m_ExpectedLength < offset + controlLength || m_ExpectedLength > offset + controlLength + 32) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "Control payload length {0} found, expected {1}",
                            controlLength, m_ExpectedLength - offset);
                        return false;
                    }
                    return true;
                }
            }

            if (m_DltLineBuilder.IsVerbose) {
                int payloadLength = m_VerboseDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                if (payloadLength == -1) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Verbose payload cannot be decoded");
                    return false;
                }
                if (m_ExpectedLength != offset + payloadLength) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Verbose payload length {0} found, expected {1}",
                        payloadLength, m_ExpectedLength - offset);
                    return false;
                }
            } else {
                int payloadLength = m_NonVerboseDecoder.Decode(standardHeader[offset..], m_DltLineBuilder);
                if (payloadLength == -1) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Non-verbose payload cannot be decoded");
                    return false;
                }
                if (m_ExpectedLength != offset + payloadLength) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Non-verbose payload length {0} found, expected {1}",
                        payloadLength, m_ExpectedLength - offset);
                    return false;
                }
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
            if (!m_Cache.IsCached) {
                if (m_DltLineBuilder.SkippedBytes == 0)
                    return Array.Empty<DltTraceLineBase>();
                m_Lines.Add(m_DltLineBuilder.GetSkippedResult());
                return m_Lines;
            }

            ReadOnlySpan<byte> buffer = m_Cache.GetCache();
            return Decode(buffer, 0, true);
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
