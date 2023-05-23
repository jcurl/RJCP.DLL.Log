namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;

    /// <summary>
    /// Encode a <see cref="SetLogLevelRequest"/>.
    /// </summary>
    public sealed class SetLogLevelRequestEncoder : ControlArgEncoderBase
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected override int EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 13) return -1;

            SetLogLevelRequest controlArg = (SetLogLevelRequest)arg;
            DltTraceEncoder.WriteId(buffer[0..4], controlArg.ApplicationId);
            DltTraceEncoder.WriteId(buffer[4..8], controlArg.ContextId);
            buffer[8] = unchecked((byte)controlArg.LogLevel);
            DltTraceEncoder.WriteId(buffer[9..13], controlArg.ComInterface);
            return 13;
        }
    }
}
