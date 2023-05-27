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
    public sealed class GetLogInfoResponseDecoder : IControlArgDecoder
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
            if (buffer.Length < 5) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetLogInfoResponse' with insufficient buffer length of {buffer.Length}"));
            }

            int status = buffer[4];
            switch (status) {
            case ControlResponse.StatusNotSupported:
            case ControlResponse.StatusError:
                service = new ControlErrorNotSupported(serviceId, status, "get_log_info");
                return 5;
            case GetLogInfoResponse.StatusOverflow:
                if (buffer.Length < 9) {
                    // Handle the case the output contains the COM or not for this. Specifications say it should not be
                    // sent.
                    service = new GetLogInfoResponse(status);
                    return 5;
                }
                break;
            }

            if (buffer.Length < 9) {
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetLogInfoResponse' with insufficient buffer length of {buffer.Length}"));
            }

            switch (status) {
            case ControlResponse.StatusOk:
            case GetLogInfoResponse.StatusOverflow:
            case GetLogInfoResponse.StatusNoMatch:
                int comId = BitOperations.To32ShiftBigEndian(buffer[5..9]);
                string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);
                service = new GetLogInfoResponse(status, comIdStr);
                return 9;
            case GetLogInfoResponse.StatusNoLogNoTrace:
            case GetLogInfoResponse.StatusWithLogNoTrace:
            case GetLogInfoResponse.StatusNoLogWithTrace:
            case GetLogInfoResponse.StatusWithLogWithTrace:
            case GetLogInfoResponse.StatusFullInfo:
                Result<int> payloadLengthResult = DecodeLogInfo(buffer[4..], msbf, status, out GetLogInfoResponse response);
                if (!payloadLengthResult.TryGet(out int payloadLength)) {
                    service = null;
                    return payloadLengthResult;
                }
                service = response;
                return 4 + payloadLength;
            default:
                service = null;
                return Result.FromException<int>(new DltDecodeException($"'GetLogInfoResponse' unknown status 0x{status:x}"));
            }
        }

        private Result<int> DecodeLogInfo(ReadOnlySpan<byte> buffer, bool msbf, int status, out GetLogInfoResponse response)
        {
            response = null;
            List<AppId> appIds = new List<AppId>();
            List<ContextId> ctxIds = new List<ContextId>();

            int appIdCount = BitOperations.To16Shift(buffer[1..], !msbf);
            int appIdOffset = 3;
            for (int i = 0; i < appIdCount; i++) {
                if (buffer.Length < appIdOffset + 6)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing application"));
                string appIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer[appIdOffset..]));

                int ctxIdCount = BitOperations.To16Shift(buffer[(appIdOffset + 4)..], !msbf);
                int ctxIdOffset = appIdOffset + 6;
                for (int j = 0; j < ctxIdCount; j++) {
                    Result<int> ctxIdLengthResult = DecodeContextId(buffer[ctxIdOffset..], msbf, status, out ContextId ctxId);
                    if (!ctxIdLengthResult.TryGet(out int ctxIdLength))
                        return ctxIdLengthResult;
                    ctxIdOffset += ctxIdLength;
                    ctxIds.Add(ctxId);
                }

                AppId appId;
                appIdOffset = ctxIdOffset;
                if (status != GetLogInfoResponse.StatusFullInfo) {
                    appId = new AppId(appIdName);
                } else {
                    if (buffer.Length < appIdOffset + 2)
                        return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing application"));
                    int appIdLen = BitOperations.To16Shift(buffer[appIdOffset..], !msbf);

                    if (buffer.Length < appIdOffset + 2 + appIdLen)
                        return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing application"));
                    string description = GetDescription(buffer[(appIdOffset + 2)..(appIdOffset + 2 + appIdLen)]);
                    appIdOffset += 2 + appIdLen;
                    appId = new AppId(appIdName, description);
                }

                foreach (ContextId ctxId in ctxIds)
                    appId.ContextIds.Add(ctxId);
                ctxIds.Clear();

                appIds.Add(appId);
            }

            if (buffer.Length < appIdOffset + 4)
                return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing application"));
            int comId = BitOperations.To32ShiftBigEndian(buffer[appIdOffset..]);
            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);

            response = new GetLogInfoResponse(status, comIdStr);
            foreach (AppId appId in appIds)
                response.AppIds.Add(appId);
            return appIdOffset + 4;
        }

        private Result<int> DecodeContextId(ReadOnlySpan<byte> buffer, bool msbf, int status, out ContextId ctxId)
        {
            ctxId = null;
            if (buffer.Length < 4)
                return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
            string ctxIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer));

            switch (status) {
            case GetLogInfoResponse.StatusNoLogNoTrace:
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, ContextId.StatusUndefined);
                return 4;
            case GetLogInfoResponse.StatusWithLogNoTrace:
                if (buffer.Length < 5)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), ContextId.StatusUndefined);
                return 5;
            case GetLogInfoResponse.StatusNoLogWithTrace:
                if (buffer.Length < 5)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, unchecked((sbyte)buffer[4]));
                return 5;
            case GetLogInfoResponse.StatusWithLogWithTrace:
                if (buffer.Length < 6)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), unchecked((sbyte)buffer[5]));
                return 6;
            case GetLogInfoResponse.StatusFullInfo:
                if (buffer.Length < 8)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
                int ctxIdLen = BitOperations.To16Shift(buffer[6..], !msbf);

                if (buffer.Length < 8 + ctxIdLen)
                    return Result.FromException<int>(new DltDecodeException("'GetLogInfoResponse' Insufficient length parsing context"));
                string description = GetDescription(buffer[8..(8 + ctxIdLen)]);
                ctxId = new ContextId(ctxIdName, GetLogLevel(buffer[4]), unchecked((sbyte)buffer[5]), description);
                return 8 + ctxIdLen;
            default:
                return Result.FromException<int>(new DltDecodeException($"Unrecognized status value {status} parsing context"));
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
