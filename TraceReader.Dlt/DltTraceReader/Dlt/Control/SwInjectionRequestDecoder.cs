namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for the payload with <see cref="SwInjectionRequest"/>.
    /// </summary>
    public class SwInjectionRequestDecoder : IControlArgDecoder
    {
        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded.</returns>
        public Result<int> Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 8) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'SwInjectionRequest' with insufficient buffer length of {buffer.Length}"));
            }

            int length = BitOperations.To32Shift(buffer[4..8], !msbf);
            if (buffer.Length != 8 + length) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'SwInjectionRequest' with insufficient buffer length of {buffer.Length}"));
            }
            Result<int> decodeLengthResult = Decode(serviceId, length, buffer[8..], msbf, out service);
            if (!decodeLengthResult.TryGet(out int decodeLength)) return decodeLengthResult;
            return 8 + decodeLength;
        }

        /// <summary>
        /// Decodes the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="length">The length as provided in the original packet.</param>
        /// <param name="buffer">The buffer after skipping over the length field.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message that is created on exit of this function.</param>
        /// <returns>The number of bytes decoded.</returns>
        protected virtual Result<int> Decode(int serviceId, int length, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            service = new SwInjectionRequest(serviceId, buffer.ToArray());
            return buffer.Length;
        }
    }
}
