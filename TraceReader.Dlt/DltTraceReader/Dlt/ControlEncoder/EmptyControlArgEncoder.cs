namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using Dlt.ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// A simple control encoder that only writes the service identifier.
    /// </summary>
    public sealed class EmptyControlArgEncoder : IControlArgEncoder
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
                return Result.FromException<int>(new DltEncodeException($"EmptyControlArgEncoder' insufficient buffer for service {arg.ServiceId:x}"));

            BitOperations.Copy32Shift(arg.ServiceId, buffer, !msbf);
            return 4;
        }
    }
}
