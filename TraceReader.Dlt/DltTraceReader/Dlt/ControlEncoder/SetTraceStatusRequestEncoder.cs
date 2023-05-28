namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="SetTraceStatusRequest"/>.
    /// </summary>
    public sealed class SetTraceStatusRequestEncoder : ControlArgEncoderBase
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 13)
                return Result.FromException<int>(new DltEncodeException("'SetTraceStatusRequestEncoder' insufficient buffer"));

            SetTraceStatusRequest controlArg = (SetTraceStatusRequest)arg;
            DltTraceEncoder.WriteId(buffer[0..4], controlArg.ApplicationId);
            DltTraceEncoder.WriteId(buffer[4..8], controlArg.ContextId);
            buffer[8] = unchecked((byte)controlArg.TraceStatus);
            DltTraceEncoder.WriteId(buffer[9..13], controlArg.ComInterface);
            return 13;
        }
    }
}
