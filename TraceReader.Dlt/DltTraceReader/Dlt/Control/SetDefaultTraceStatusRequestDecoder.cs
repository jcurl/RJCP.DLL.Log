namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decodes a payload to retrieve a <see cref="SetDefaultTraceStatusRequest"/>.
    /// </summary>
    public class SetDefaultTraceStatusRequestDecoder : ControlArgDecoderBase
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
        public override int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            if (buffer.Length < 9)
                return DecodeError(serviceId, DltType.CONTROL_REQUEST,
                    "'SetDefaultTraceStatusRequest' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            int logLevel = buffer[4];
            int comId = BitOperations.To32ShiftBigEndian(buffer[5..9]);

            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);
            service = new SetDefaultTraceStatusRequest(logLevel != 0, comIdStr);
            return 9;
        }
    }
}
