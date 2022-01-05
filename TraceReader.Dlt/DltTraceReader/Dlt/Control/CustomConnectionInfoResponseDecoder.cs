namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="CustomConnectionInfoResponse"/>.
    /// </summary>
    public class CustomConnectionInfoResponseDecoder : IControlArgDecoder
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
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            int status = buffer[4];
            int state = buffer[5];
            int comId = BitOperations.To32ShiftBigEndian(buffer[6..10]);

            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);
            service = new CustomConnectionInfoResponse(status, state, comIdStr);
            return 10;
        }
    }
}
