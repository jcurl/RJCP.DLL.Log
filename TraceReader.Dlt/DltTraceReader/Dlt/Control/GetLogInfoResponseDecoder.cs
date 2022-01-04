﻿namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Decoder for <see cref="GetLogInfoResponse"/>.
    /// </summary>
    public class GetLogInfoResponseDecoder : IControlArgDecoder
    {
        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, out IControlArg service)
        {
            int status = buffer[4];

            switch (status) {
            case GetLogInfoResponse.StatusOk:
            case GetLogInfoResponse.StatusNotSupported:
            case GetLogInfoResponse.StatusError:
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
                int payloadLength = DecodeLogInfo(buffer[4..], status, out GetLogInfoResponse response);
                service = response;
                return 4 + payloadLength;
            default:
                service = null;
                return -1;
            }
        }

        private int DecodeLogInfo(ReadOnlySpan<byte> buffer, int status, out GetLogInfoResponse response)
        {
            List<AppId> appIds = new List<AppId>();
            List<ContextId> ctxIds = new List<ContextId>();

            int appIdCount = BitOperations.To16ShiftLittleEndian(buffer[1..]);
            int appIdOffset = 3;
            for (int i = 0; i < appIdCount; i++) {
                string appIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer[appIdOffset..]));

                int ctxIdCount = BitOperations.To16ShiftLittleEndian(buffer[(appIdOffset + 4)..]);
                int ctxIdOffset = appIdOffset + 6;
                for (int j = 0; j < ctxIdCount; j++) {
                    int ctxIdLength = DecodeContextId(buffer[ctxIdOffset..], status, out ContextId ctxId);
                    ctxIdOffset += ctxIdLength;
                    ctxIds.Add(ctxId);
                }

                AppId appId;
                appIdOffset = ctxIdOffset;
                if (status != GetLogInfoResponse.StatusFullInfo) {
                    appId = new AppId(appIdName);
                } else {
                    int appIdLen = BitOperations.To16ShiftLittleEndian(buffer[appIdOffset..]);
                    string description = GetDescription(buffer[(appIdOffset + 2)..(appIdOffset + 2 + appIdLen)]);
                    appIdOffset += 2 + appIdLen;
                    appId = new AppId(appIdName, description);
                }

                foreach (ContextId ctxId in ctxIds)
                    appId.ContextIds.Add(ctxId);
                ctxIds.Clear();

                appIds.Add(appId);
            }

            int comId = BitOperations.To32ShiftBigEndian(buffer[(appIdOffset)..]);
            string comIdStr = comId == 0 ? string.Empty : IdHashList.Instance.ParseId(comId);

            response = new GetLogInfoResponse(status, comIdStr);
            foreach (AppId appId in appIds)
                response.AppIds.Add(appId);

            return appIdOffset + 4;
        }

        private int DecodeContextId(ReadOnlySpan<byte> buffer, int status, out ContextId ctxId)
        {
            string ctxIdName = IdHashList.Instance.ParseId(BitOperations.To32ShiftBigEndian(buffer));

            switch (status) {
            case GetLogInfoResponse.StatusNoLogNoTrace:
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, ContextId.StatusUndefined);
                return 4;
            case GetLogInfoResponse.StatusWithLogNoTrace:
                ctxId = new ContextId(ctxIdName, (LogLevel)(sbyte)buffer[4], ContextId.StatusUndefined);
                return 5;
            case GetLogInfoResponse.StatusNoLogWithTrace:
                ctxId = new ContextId(ctxIdName, LogLevel.Undefined, (sbyte)buffer[4]);
                return 5;
            case GetLogInfoResponse.StatusWithLogWithTrace:
                ctxId = new ContextId(ctxIdName, (LogLevel)(sbyte)buffer[4], (sbyte)buffer[5]);
                return 6;
            case GetLogInfoResponse.StatusFullInfo:
                int ctxIdLen = BitOperations.To16ShiftLittleEndian(buffer[6..]);
                string description = GetDescription(buffer[8..(8 + ctxIdLen)]);
                ctxId = new ContextId(ctxIdName, (LogLevel)(sbyte)buffer[4], (sbyte)buffer[5], description);
                return 8 + ctxIdLen;
            default:
                string msg = string.Format("Unrecognized status value {0}", status);
                throw new InvalidOperationException(msg);
            }
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