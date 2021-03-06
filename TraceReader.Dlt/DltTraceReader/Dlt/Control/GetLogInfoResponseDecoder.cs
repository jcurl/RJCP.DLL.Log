namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="GetLogInfoResponse"/>.
    /// </summary>
    public class GetLogInfoResponseDecoder : ControlArgDecoderBase
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
            if (buffer.Length < 5)
                return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                    "'GetLogInfoResponse' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            int status = buffer[4];
            switch (status) {
            case ControlResponse.StatusNotSupported:
            case ControlResponse.StatusError:
                service = new ControlErrorNotSupported(serviceId, status, "get_log_info");
                return 5;
            }

            if (buffer.Length < 9)
                return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                    "'GetLogInfoResponse' with insufficient buffer length of {0}", buffer.Length,
                    out service);

            switch (status) {
            case ControlResponse.StatusOk:
            case GetLogInfoResponse.StatusNoMatch:
            case GetLogInfoResponse.StatusOverflow:
                int comId = BitOperations.To32ShiftBigEndian(buffer[5..9]);
                string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);
                service = new GetLogInfoResponse(status, comIdStr);
                return 9;
            case GetLogInfoResponse.StatusNoLogNoTrace:
            case GetLogInfoResponse.StatusWithLogNoTrace:
            case GetLogInfoResponse.StatusNoLogWithTrace:
            case GetLogInfoResponse.StatusWithLogWithTrace:
            case GetLogInfoResponse.StatusFullInfo:
                int payloadLength = DecodeLogInfo(buffer[4..], msbf, status, out GetLogInfoResponse response);
                if (payloadLength == -1)
                    return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                        "'GetLogInfoResponse' with insufficient buffer length of {0}", buffer.Length,
                        out service);
                service = response;
                return 4 + payloadLength;
            default:
                return DecodeError(serviceId, DltType.CONTROL_RESPONSE,
                    "'GetLogInfoResponse' unknown status 0x{0:x}", status, out service);
            }
        }

        private int DecodeLogInfo(ReadOnlySpan<byte> buffer, bool msbf, int status, out GetLogInfoResponse response)
        {
            response = null;
            List<AppId> appIds = new List<AppId>();
            List<ContextId> ctxIds = new List<ContextId>();

            int appIdCount = BitOperations.To16Shift(buffer[1..], !msbf);
            int appIdOffset = 3;
            for (int i = 0; i < appIdCount; i++) {
                if (buffer.Length < appIdOffset + 6) return -1;
                string appIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer[appIdOffset..]));

                int ctxIdCount = BitOperations.To16Shift(buffer[(appIdOffset + 4)..], !msbf);
                int ctxIdOffset = appIdOffset + 6;
                for (int j = 0; j < ctxIdCount; j++) {
                    int ctxIdLength = DecodeContextId(buffer[ctxIdOffset..], msbf, status, out ContextId ctxId);
                    if (ctxIdLength == -1) return -1;
                    ctxIdOffset += ctxIdLength;
                    ctxIds.Add(ctxId);
                }

                AppId appId;
                appIdOffset = ctxIdOffset;
                if (status != GetLogInfoResponse.StatusFullInfo) {
                    appId = new AppId(appIdName);
                } else {
                    if (buffer.Length < appIdOffset + 2) return -1;
                    int appIdLen = BitOperations.To16Shift(buffer[appIdOffset..], !msbf);

                    if (buffer.Length < appIdOffset + 2 + appIdLen) return -1;
                    string description = GetDescription(buffer[(appIdOffset + 2)..(appIdOffset + 2 + appIdLen)]);
                    appIdOffset += 2 + appIdLen;
                    appId = new AppId(appIdName, description);
                }

                foreach (ContextId ctxId in ctxIds)
                    appId.ContextIds.Add(ctxId);
                ctxIds.Clear();

                appIds.Add(appId);
            }

            if (buffer.Length < appIdOffset + 4) return -1;
            int comId = BitOperations.To32ShiftBigEndian(buffer[appIdOffset..]);
            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);

            response = new GetLogInfoResponse(status, comIdStr);
            foreach (AppId appId in appIds)
                response.AppIds.Add(appId);
            return appIdOffset + 4;
        }

        private int DecodeContextId(ReadOnlySpan<byte> buffer, bool msbf, int status, out ContextId ctxId)
        {
            ctxId = null;
            if (buffer.Length < 4) return -1;
            string ctxIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer));

            switch (status) {
            case GetLogInfoResponse.StatusNoLogNoTrace:
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, ContextId.StatusUndefined);
                return 4;
            case GetLogInfoResponse.StatusWithLogNoTrace:
                if (buffer.Length < 5) return -1;
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), ContextId.StatusUndefined);
                return 5;
            case GetLogInfoResponse.StatusNoLogWithTrace:
                if (buffer.Length < 5) return -1;
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, unchecked((sbyte)buffer[4]));
                return 5;
            case GetLogInfoResponse.StatusWithLogWithTrace:
                if (buffer.Length < 6) return -1;
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), unchecked((sbyte)buffer[5]));
                return 6;
            case GetLogInfoResponse.StatusFullInfo:
                if (buffer.Length < 8) return -1;
                int ctxIdLen = BitOperations.To16Shift(buffer[6..], !msbf);

                if (buffer.Length < 8 + ctxIdLen) return -1;
                string description = GetDescription(buffer[8..(8 + ctxIdLen)]);
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), unchecked((sbyte)buffer[5]), description);
                return 8 + ctxIdLen;
            default:
                string msg = string.Format("Unrecognized status value {0}", status);
                throw new InvalidOperationException(msg);
            }
        }

        private static LogLevel GetLogLevel(byte value)
        {
            return (LogLevel)unchecked((sbyte)value);
        }

        private readonly Decoder m_Utf8Decoder = Encoding.UTF8.GetDecoder();
        private readonly char[] m_CharResult = new char[ushort.MaxValue];

        private string GetDescription(ReadOnlySpan<byte> buffer)
        {
            m_Utf8Decoder.Convert(buffer, m_CharResult.AsSpan(), true, out _, out int cu, out _);
            return new string(m_CharResult, 0, cu);
        }
    }
}
