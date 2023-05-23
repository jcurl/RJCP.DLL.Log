namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encode a control response.
    /// </summary>
    /// <remarks>
    /// Override this class to encode further payload information. The status will be encoded by this class.
    /// </remarks>
    public class ControlArgResponseEncoder : IControlArgEncoder
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
            if (buffer.Length < 5) return -1;
            BitOperations.Copy32Shift(arg.ServiceId, buffer, !msbf);

            ControlResponse response = (ControlResponse)arg;
            buffer[4] = unchecked((byte)response.Status);

            int written = EncodePayload(buffer[5..], msbf, response);
            if (written == -1) return -1;
            return written + 5;
        }

        /// <summary>
        /// Encodes the payload, immediately after the service identifier and the status.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected virtual int EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            return 0;
        }
    }
}
