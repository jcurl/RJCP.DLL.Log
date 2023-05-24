namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using Encoder;

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
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected override int EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            if (buffer.Length < 5) return -1;

            CustomConnectionInfoResponse controlArg = (CustomConnectionInfoResponse)arg;
            buffer[0] = unchecked((byte)controlArg.ConnectionState);
            DltTraceEncoder.WriteId(buffer[1..5], controlArg.ComInterface);
            return 5;
        }
    }
}
