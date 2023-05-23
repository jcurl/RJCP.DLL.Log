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
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        public int Encode(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            if (buffer.Length < 4) return -1;
            BitOperations.Copy32Shift(arg.ServiceId, buffer, !msbf);

            int written = EncodePayload(buffer[4..], msbf, arg);
            if (written == -1) return -1;
            return written + 4;
        }

        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected abstract int EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg);
    }
}
