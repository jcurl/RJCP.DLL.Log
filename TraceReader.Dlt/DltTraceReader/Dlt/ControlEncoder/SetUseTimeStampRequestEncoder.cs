namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Encode a <see cref="SetUseTimeStampRequest"/>.
    /// </summary>
    public sealed class SetUseTimeStampRequestEncoder : ControlArgEncoderBase
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
            if (buffer.Length < 1) return -1;

            SetUseTimeStampRequest controlArg = (SetUseTimeStampRequest)arg;
            buffer[0] = controlArg.Enabled ? (byte)1 : (byte)0;
            return 1;
        }
    }
}
