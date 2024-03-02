namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;
    using System.Text;
    using RJCP.Core;

    /// <summary>
    /// Represents arbitrary data for a software injection request.
    /// </summary>
    public class SwInjectionRequest : ControlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwInjectionRequest"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="payload">The payload bytes.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serviceId"/> is outside valid range of 0xFFF..0xFFFFFFFF.
        /// </exception>
        public SwInjectionRequest(int serviceId, byte[] payload)
        {
            ThrowHelper.ThrowIfBetween(serviceId, 0, 0xFFE);
            ServiceId = serviceId;
            Payload = payload ?? Array.Empty<byte>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwInjectionRequest"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="payLoad">
        /// The pay load as a string, which will be converted to bytes from UTF8 and appended with a NUL terminator.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serviceId"/> is outside valid range of 0xFFF..0xFFFFFFFF.
        /// </exception>
        public SwInjectionRequest(int serviceId, string payLoad)
            : this(serviceId, payLoad, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwInjectionRequest"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="payLoad">
        /// The pay load as a string, which will be converted to bytes from UTF8 and appended with a NUL terminator if
        /// <paramref name="appendNul"/> is <see langword="true"/>.
        /// </param>
        /// <param name="appendNul">
        /// If set to <see langword="true"/> then the string is appended with an ASCII NUL (0x00).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serviceId"/> is outside valid range of 0xFFF..0xFFFFFFFF.
        /// </exception>
        public SwInjectionRequest(int serviceId, string payLoad, bool appendNul)
            : this(serviceId, GetUtf8Bytes(payLoad, appendNul)) { }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId { get; }

        /// <summary>
        /// Gets the payload, without the length field. The array size is the length.
        /// </summary>
        /// <value>The payload.</value>
        public byte[] Payload { get; }

        private static byte[] GetUtf8Bytes(string payLoad, bool appendNul)
        {
            if (string.IsNullOrEmpty(payLoad))
                return appendNul ? new byte[] { 0x00 } : Array.Empty<byte>();

            byte[] dataBytes = Encoding.UTF8.GetBytes(payLoad);
            if (!appendNul) return dataBytes;

            byte[] buffer = new byte[dataBytes.Length + 1];
            Buffer.BlockCopy(dataBytes, 0, buffer, 0, dataBytes.Length);
            buffer[^1] = 0x00;

            return buffer;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            Span<byte> dataLength = stackalloc byte[4];

            StringBuilder strBuilder = new("[] ", Payload.Length * 3 + 15);
            BitOperations.Copy32ShiftLittleEndian(Payload.Length, dataLength);
            Args.HexConvert.ConvertToHex(strBuilder, dataLength);
            if (Payload.Length > 0) {
                strBuilder.Append(' ');
                Args.HexConvert.ConvertToHex(strBuilder, Payload);
            }

            return strBuilder.ToString();
        }
    }
}
