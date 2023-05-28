namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

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
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 5)
                return Result.FromException<int>(new DltEncodeException("'SetDefaultLogLevelRequestEncoder' insufficient buffer"));

            SetDefaultLogLevelRequest controlArg = (SetDefaultLogLevelRequest)arg;
            buffer[0] = unchecked((byte)controlArg.LogLevel);
            DltTraceEncoder.WriteId(buffer[1..5], controlArg.ComInterface);
            return 5;
        }
    }
}
