namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="GetTraceStatusRequest"/>.
    /// </summary>
    public sealed class GetTraceStatusRequestEncoder : ControlArgEncoderBase
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the bufferr.</returns>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 8)
                return Result.FromException<int>(new DltEncodeException("'GetTraceStatusRequestEncoder' insufficient buffer"));

            GetTraceStatusRequest controlArg = (GetTraceStatusRequest)arg;
            DltTraceEncoder.WriteId(buffer[0..4], controlArg.ApplicationId);
            DltTraceEncoder.WriteId(buffer[4..8], controlArg.ContextId);
            return 8;
        }
    }
}
