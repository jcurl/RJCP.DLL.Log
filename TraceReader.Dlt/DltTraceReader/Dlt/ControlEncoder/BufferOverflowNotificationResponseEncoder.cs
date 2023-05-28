namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="BufferOverflowNotificationResponse"/>.
    /// </summary>
    public sealed class BufferOverflowNotificationResponseEncoder : ControlArgResponseEncoder
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
            if (buffer.Length < 4)
                return Result.FromException<int>(new DltEncodeException("'BufferOverflowNotificationResponseEncoder' insufficient buffer"));

            BufferOverflowNotificationResponse controlArg = (BufferOverflowNotificationResponse)arg;
            BitOperations.Copy32Shift(controlArg.Counter, buffer, !msbf);
            return 4;
        }
    }
}
