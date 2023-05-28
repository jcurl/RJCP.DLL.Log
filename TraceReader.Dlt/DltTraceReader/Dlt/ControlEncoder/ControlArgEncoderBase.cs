namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Base class containing common code to encode a control message.
    /// </summary>
    public abstract class ControlArgEncoderBase : IControlArgEncoder
    {
        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the control argument to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        public Result<int> Encode(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 4)
                return Result.FromException<int>(new DltEncodeException($"ControlArgEncoderBase Insufficient buffer for service {arg.ServiceId:x}"));
            BitOperations.Copy32Shift(arg.ServiceId, buffer, !msbf);

            Result<int> result = EncodePayload(buffer[4..], msbf, arg);
            if (!result.TryGet(out int written)) return result;
            return written + 4;
        }

        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        protected abstract Result<int> EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg);
    }
}
