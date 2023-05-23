namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;

    /// <summary>
    /// Encode a <see cref="SetDefaultLogLevelRequest"/>.
    /// </summary>
    public sealed class SetDefaultLogLevelRequestEncoder : ControlArgEncoderBase
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
            if (buffer.Length < 5) return -1;

            SetDefaultLogLevelRequest controlArg = (SetDefaultLogLevelRequest)arg;
            buffer[0] = unchecked((byte)controlArg.LogLevel);
            DltTraceEncoder.WriteId(buffer[1..5], controlArg.ComInterface);
            return 5;
        }
    }
}
