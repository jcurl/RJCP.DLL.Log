namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes non-verbose payloads.
    /// </summary>
    public class NonVerboseDltDecoder : INonVerboseDltDecoder
    {
        private readonly INonVerboseArgDecoder m_ArgDecoder;

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
            Fallback = fallbackDecoder ?? new NonVerboseByteDecoder();
            m_ArgDecoder = argDecoder ?? new NonVerboseArgDecoder();
        }

        /// <summary>
        /// Gets the frame map, which maps identifiers into frames consisting of arguments to construct a
        /// <see cref="DltTraceLine"/>.
        /// </summary>
        /// <value>The frame map.</value>
        public IFrameMap FrameMap { get; }

        /// <summary>
        /// If not <see langword="null"/>, use the fallback decoder in case of decoder errors.
        /// </summary>
        /// <value>The fallback decoder.</value>
        public INonVerboseDltDecoder Fallback { get; }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer that should be decoded.</param>
        /// <param name="lineBuilder">
        /// The line builder providing information from the standard header, and where the decoded packets will be
        /// placed.
        /// </param>
        /// <returns>The length of all the decoded verbose arguments in the buffer.</returns>
        public Result<int> Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            if (FrameMap is null)
                return Fallback.Decode(buffer, lineBuilder);

            try {
                if (buffer.Length < 4) {
                    lineBuilder.SetMessageId(0);
                    NonVerboseDltArg arg = new NonVerboseDltArg(Array.Empty<byte>());
                    lineBuilder.AddArgument(arg);
                    lineBuilder.SetErrorMessage("Buffer too small to contain the message identifier ({0} bytes)", buffer.Length);
                    return buffer.Length;
                } else {
                    int messageId = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    if (!FrameMap.TryGetFrame(messageId,
                      lineBuilder.Features.ApplicationId ? lineBuilder.ApplicationId : null,
                      lineBuilder.Features.ContextId ? lineBuilder.ContextId : null,
                      lineBuilder.Features.EcuId ? lineBuilder.EcuId : null,
                      out IFrame frame)) {
                        // The lineBuilder.SetErrorMessage is not set, so nothing is traced.
                        return Result.FromException<int>(new DltDecodeException($"Message {messageId} not mapped"));
                    }

                    lineBuilder.SetMessageId(messageId);
                    lineBuilder.SetDltType(frame.MessageType);
                    if (!lineBuilder.Features.ApplicationId || lineBuilder.Features.ContextId) {
                        lineBuilder.SetApplicationId(frame.ApplicationId);
                        lineBuilder.SetContextId(frame.ContextId);
                    }
                    if (!lineBuilder.Features.EcuId) {
                        lineBuilder.SetEcuId(frame.EcuId);
                    }

                    Result<int> payloadLengthResult = DecodePdus(buffer[4..], messageId, frame, lineBuilder);
                    if (!payloadLengthResult.TryGet(out int payloadLength)) {
                        lineBuilder.ResetArguments();
                        return payloadLengthResult;
                    }
                    return payloadLength + 4;
                }
            } catch (Exception ex) {
                Log.DltNonVerbose.TraceException(ex, nameof(Decode), "Exception while decoding");
                return Result.FromException<int>(ex);
            }
        }

        private Result<int> DecodePdus(ReadOnlySpan<byte> buffer, int messageId, IFrame frame, IDltLineBuilder lineBuilder)
        {
            int payloadLength = 0;
            for (int i = 0; i < frame.Arguments.Count; i++) {
                IPdu pdu = frame.Arguments[i];
                Result<int> argLengthResult = m_ArgDecoder.Decode(buffer, lineBuilder.BigEndian, pdu, out IDltArg argument);
                if (!argLengthResult.TryGet(out int argLength)) {
                    string message;
                    string intMessage = argLengthResult.Error.Message;
                    if (!string.IsNullOrEmpty(intMessage)) {
                        message = string.Format("Message ECU={0}, App={1}, Ctx={2}, Id={3} (0x{3:x}) arg {4} of {5}, {6}",
                            lineBuilder.EcuId, lineBuilder.ApplicationId, lineBuilder.ContextId,
                            messageId, i + 1, frame.Arguments.Count, intMessage);
                    } else {
                        message = string.Format("Message ECU={0}, App={1}, Ctx={2}, Id={3} (0x{3:x}) pdu {4} of {5} decoding error",
                            lineBuilder.EcuId, lineBuilder.ApplicationId, lineBuilder.ContextId,
                            messageId, i + 1, frame.Arguments.Count);
                    }
                    lineBuilder.SetErrorMessage(message);
                    return Result.FromException<int>(new DltDecodeException(message, argLengthResult.Error));
                }

                lineBuilder.AddArgument(argument);
                buffer = buffer[argLength..];
                payloadLength += argLength;
            }
            return payloadLength;
        }
    }
}
