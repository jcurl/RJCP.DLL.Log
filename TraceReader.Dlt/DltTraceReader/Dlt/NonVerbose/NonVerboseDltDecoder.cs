namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes non-verbose payloads.
    /// </summary>
    public class NonVerboseDltDecoder : INonVerboseDltDecoder
    {
        private readonly INonVerboseArgDecoder m_ArgDecoder;
        private readonly INonVerboseDltDecoder m_BinaryDecoder;
        private readonly HashSet<int> m_Logged = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseDltDecoder"/> class.
        /// </summary>
        /// <param name="map">
        /// The Frame Map to use that maps message identifiers to <see cref="IFrame"/> s and <see cref="IPdu"/> s.
        /// </param>
        public NonVerboseDltDecoder(IFrameMap map) : this(map, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseDltDecoder"/> class.
        /// </summary>
        /// <param name="map">
        /// The Frame Map to use that maps message identifiers to <see cref="IFrame"/> s and <see cref="IPdu"/> s.
        /// </param>
        /// <param name="argDecoder">The argument decoder that should be used.</param>
        public NonVerboseDltDecoder(IFrameMap map, INonVerboseArgDecoder argDecoder) : this(map, argDecoder, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseDltDecoder"/> class.
        /// </summary>
        /// <param name="map">
        /// The Frame Map to use that maps message identifiers to <see cref="IFrame"/> s and <see cref="IPdu"/> s.
        /// </param>
        /// <param name="argDecoder">The argument decoder that should be used.</param>
        /// <param name="fallbackDecoder">The fallback decoder to use in case a payload cannot be decoded.</param>
        public NonVerboseDltDecoder(IFrameMap map, INonVerboseArgDecoder argDecoder, INonVerboseDltDecoder fallbackDecoder)
        {
            FrameMap = map;
            m_ArgDecoder = argDecoder ?? new NonVerboseArgDecoder();
            if (map == null) {
                m_BinaryDecoder = fallbackDecoder;
            } else {
                m_BinaryDecoder = fallbackDecoder ?? new NonVerboseByteDecoder();
            }
        }

        /// <summary>
        /// Gets the frame map, which maps identifiers into frames consisting of arguments to construct a
        /// <see cref="DltTraceLine"/>.
        /// </summary>
        /// <value>The frame map.</value>
        public IFrameMap FrameMap { get; private set; }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer that should be decoded.</param>
        /// <param name="lineBuilder">
        /// The line builder providing information from the standard header, and where the decoded packets will be
        /// placed.
        /// </param>
        /// <returns>The length of all the decoded verbose arguments in the buffer.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            if (FrameMap == null)
                return m_BinaryDecoder.Decode(buffer, lineBuilder);

            try {
                if (buffer.Length < 4) {
                    lineBuilder.SetMessageId(0);
                    NonVerboseDltArg arg = new NonVerboseDltArg(Array.Empty<byte>());
                    lineBuilder.AddArgument(arg);
                    return buffer.Length;
                } else {
                    int messageId = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    lineBuilder.SetMessageId(messageId);

                    if (!FrameMap.TryGetFrame(messageId, lineBuilder.ApplicationId, lineBuilder.ContextId, lineBuilder.EcuId, out IFrame frame)) {
                        // Only log once per message.
                        if (!m_Logged.Contains(messageId)) {
                            m_Logged.Add(messageId);
                            if (Log.DltNonVerbose.ShouldTrace(TraceEventType.Information)) {
                                Log.DltNonVerbose.TraceEvent(TraceEventType.Information, "Missing frame identifier", messageId);
                            }
                        }
                        return m_BinaryDecoder.Decode(buffer, lineBuilder);
                    }

                    lineBuilder.SetDltType(frame.MessageType);
                    if (lineBuilder.ApplicationId == null || lineBuilder.ContextId == null) {
                        lineBuilder.SetApplicationId(frame.ApplicationId ?? string.Empty);
                        lineBuilder.SetContextId(frame.ContextId ?? string.Empty);
                    }
                    if (lineBuilder.EcuId == null) {
                        lineBuilder.SetEcuId(frame.EcuId ?? string.Empty);
                    }

                    buffer = buffer[4..];
                    int payloadLength = 4;
                    for (int i = 0; i < frame.Arguments.Count; i++) {
                        IPdu pdu = frame.Arguments[i];
                        int argLength = m_ArgDecoder.Decode(buffer, lineBuilder.BigEndian, pdu, out IDltArg argument);
                        if (argLength < 0) {
                            if (argument is DltArgError argError) {
                                lineBuilder.SetErrorMessage(
                                    "NonVerbose Message 0x{0:x} arg {1} of {2}, {3}",
                                    messageId, i + 1, frame.Arguments.Count, argError.Message);
                            } else {
                                lineBuilder.SetErrorMessage(
                                    "Verbose Message 0x{0:x} arg {1} of {2} decoding error",
                                    messageId, i + 1, frame.Arguments.Count);
                            }
                            return -1;
                        }

                        lineBuilder.AddArgument(argument);
                        buffer = buffer[argLength..];
                        payloadLength += argLength;
                    }
                    return payloadLength;
                }
            } catch (Exception ex) {
                Log.DltNonVerbose.TraceException(ex, nameof(Decode), "Exception while decoding");
                return -1;
            }
        }
    }
}
