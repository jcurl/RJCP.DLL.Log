namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="CustomConnectionInfoResponse"/>.
    /// </summary>
    public sealed class CustomConnectionInfoResponseEncoder : ControlArgResponseEncoder
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
            if (buffer.Length < 5)
                return Result.FromException<int>(new DltEncodeException("'CustomConnectionInfoResponseEncoder' insufficient buffer"));

            CustomConnectionInfoResponse controlArg = (CustomConnectionInfoResponse)arg;
            buffer[0] = unchecked((byte)controlArg.ConnectionState);
            DltTraceEncoder.WriteId(buffer[1..5], controlArg.ComInterface);
            return 5;
        }
    }
}
