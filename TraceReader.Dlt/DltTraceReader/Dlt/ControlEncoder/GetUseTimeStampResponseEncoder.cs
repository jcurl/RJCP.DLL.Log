namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="GetUseTimeStampResponse"/>.
    /// </summary>
    public sealed class GetUseTimeStampResponseEncoder : ControlArgResponseEncoder
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
            if (arg.Status != ControlResponse.StatusOk) return 0;
            if (buffer.Length < 1)
                return Result.FromException<int>(new DltEncodeException("'GetUseTimeStampResponseEncoder' insufficient buffer"));

            GetUseTimeStampResponse controlArg = (GetUseTimeStampResponse)arg;
            buffer[0] = controlArg.Enabled ? (byte)1 : (byte)0;
            return 1;
        }
    }
}
