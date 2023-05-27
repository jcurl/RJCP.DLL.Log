namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="CustomUnregisterContextResponse"/>.
    /// </summary>
    public sealed class CustomUnregisterContextResponseDecoder : IControlArgDecoder
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
            if (buffer.Length < 17) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'CustomUnregisterContextResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            int appId = BitOperations.To32ShiftBigEndian(buffer[5..9]);
            int ctxId = BitOperations.To32ShiftBigEndian(buffer[9..13]);
            int comId = BitOperations.To32ShiftBigEndian(buffer[13..17]);

            string appIdStr = appId == 0 ? string.Empty : IdHashList.Instance.ParseId(appId);
            string ctxIdStr = ctxId == 0 ? string.Empty : IdHashList.Instance.ParseId(ctxId);
            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);
            service = new CustomUnregisterContextResponse(status, appIdStr, ctxIdStr, comIdStr);
            return 17;
        }
    }
}
