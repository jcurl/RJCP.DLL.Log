namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="CustomUnregisterContextResponse"/>.
    /// </summary>
    public sealed class CustomUnregisterContextResponseEncoder : ControlArgResponseEncoder
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier and the status.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            if (buffer.Length < 12)
                return Result.FromException<int>(new DltEncodeException("'CustomUnregisterContextResponseEncoder' insufficient buffer"));

            CustomUnregisterContextResponse controlArg = (CustomUnregisterContextResponse)arg;
            DltTraceEncoder.WriteId(buffer[0..4], controlArg.ApplicationId);
            DltTraceEncoder.WriteId(buffer[4..8], controlArg.ContextId);
            DltTraceEncoder.WriteId(buffer[8..12], controlArg.ComInterface);
            return 12;
        }
    }
}
