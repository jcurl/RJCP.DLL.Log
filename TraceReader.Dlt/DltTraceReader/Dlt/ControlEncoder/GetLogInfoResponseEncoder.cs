namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ControlArgs;
    using Encoder;
    using RJCP.Core;

    /// <summary>
    /// Encode a <see cref="GetLogInfoResponse"/>.
    /// </summary>
    /// <remarks>
    /// When encoding, if there is not enough buffer, the function returns -1, so you should encode a second
    /// time, but with a status of <see cref="GetLogInfoResponse.StatusOverflow"/>.
    /// </remarks>
    public sealed class GetLogInfoResponseEncoder : ControlArgResponseEncoder
    {
        // Not static as the `System.Text.Encoder` is not multithreading safe / pure.
        private readonly Encoder m_Utf8Enc = Encoding.UTF8.GetEncoder();

        /// <summary>
        /// Encodes the payload, immediately after the service identifier and the status.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        /// <remarks>
        /// When encoding, if there is not enough buffer, the result will contain an error, so you should encode a
        /// second time, but with a status of <see cref="GetLogInfoResponse.StatusOverflow"/>.
        /// </remarks>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            bool writeInfo = true;

            GetLogInfoResponse controlArg = (GetLogInfoResponse)arg;
            switch (controlArg.Status) {
            case ControlResponse.StatusOk:
            case ControlResponse.StatusNotSupported:
            case ControlResponse.StatusError:
            case GetLogInfoResponse.StatusOverflow:
                return 0;
            case GetLogInfoResponse.StatusNoMatch:
                writeInfo = false;
                break;
            case GetLogInfoResponse.StatusNoLogNoTrace:
            case GetLogInfoResponse.StatusWithLogNoTrace:
            case GetLogInfoResponse.StatusNoLogWithTrace:
            case GetLogInfoResponse.StatusWithLogWithTrace:
            case GetLogInfoResponse.StatusFullInfo:
                break;
            default:
                return Result.FromException<int>(new DltEncodeException($"'GetLogInfoResponseEncoder' unknown status code {controlArg.Status}"));
            }

            // LogInfoType
            //  uint16_t app_ids_count;
            //  AppIDsType[] app_ids;
            //
            // AppIDsType
            //  uint32_t appid;
            //  uint16_t ctx_ids_count;
            //  CtxIDsType[] ctx_ids;
            //  uint16_t len_description;    7
            //  char[]   app_description;    7
            //
            // CtxIDsType
            //  uint32_t ctxid;
            //  int8_t   log_level;          4, 6, 7
            //  int8_t   trace_status;       5, 6, 7
            //  uint16_t len_description;    7
            //  char[]   ctx_description;    7

            int pos = 0;
            if (writeInfo) {
                Result<int> result = WriteLogInfoType(buffer, msbf, arg.Status, controlArg.AppIds);
                if (!result.TryGet(out pos)) return result;
            }

            if (buffer.Length < pos + 4)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer"));
            DltTraceEncoder.WriteId(buffer[pos..], controlArg.ComInterface);
            return pos + 4;
        }

        private Result<int> WriteLogInfoType(Span<byte> buffer, bool msbf, int status, IList<AppId> apps)
        {
            if (buffer.Length < 2)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing Apps"));

            BitOperations.Copy16Shift(apps.Count, buffer, !msbf);

            int pos = 2;
            foreach (AppId app in apps) {
                Result<int> result = WriteAppIds(buffer[pos..], msbf, status, app);
                if (!result.TryGet(out int written)) return result;
                pos += written;
            }
            return pos;
        }

        private Result<int> WriteAppIds(Span<byte> buffer, bool msbf, int status, AppId app)
        {
            if (buffer.Length < 6)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing App"));

            DltTraceEncoder.WriteId(buffer, app.Name);
            BitOperations.Copy16Shift(app.ContextIds.Count, buffer[4..], !msbf);

            int pos = 6;
            foreach (ContextId ctx in app.ContextIds) {
                Result<int> result = WriteCtxIds(buffer[pos..], msbf, status, ctx);
                if (!result.TryGet(out int written)) return result;
                pos += written;
            }

            if (status == GetLogInfoResponse.StatusFullInfo) {
                Result<int> result = WriteDescription(buffer[pos..], msbf, app.Description);
                if (!result.TryGet(out int written)) return result;
                pos += written;
            }
            return pos;
        }

        private Result<int> WriteCtxIds(Span<byte> buffer, bool msbf, int status, ContextId ctx)
        {
            if (buffer.Length < 4)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing context"));

            DltTraceEncoder.WriteId(buffer, ctx.Name);
            int pos = 4;

            if (status == GetLogInfoResponse.StatusWithLogNoTrace ||
                status == GetLogInfoResponse.StatusWithLogWithTrace ||
                status == GetLogInfoResponse.StatusFullInfo) {
                if (buffer.Length < pos + 1)
                    return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing context"));
                buffer[pos] = unchecked((byte)ctx.LogLevel);
                pos++;
            }

            if (status == GetLogInfoResponse.StatusNoLogWithTrace ||
                status == GetLogInfoResponse.StatusWithLogWithTrace ||
                status == GetLogInfoResponse.StatusFullInfo) {
                if (buffer.Length < pos + 1)
                    return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing context"));
                buffer[pos] = unchecked((byte)ctx.TraceStatus);
                pos++;
            }

            if (status == GetLogInfoResponse.StatusFullInfo) {
                Result<int> result = WriteDescription(buffer[pos..], msbf, ctx.Description);
                if (!result.TryGet(out int written)) return result;
                pos += written;
            }
            return pos;
        }

        private Result<int> WriteDescription(Span<byte> buffer, bool msbf, string description)
        {
            if (description.Length == 0) {
                if (buffer.Length < 3)
                    return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing description"));
                BitOperations.Copy16Shift(1, buffer, !msbf);
                buffer[2] = 0;
                return 3;
            }

            if (buffer.Length < 4)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing description"));

            m_Utf8Enc.Convert(description, buffer[2..^1], true, out _, out int bu, out bool complete);
            if (!complete)
                return Result.FromException<int>(new DltEncodeException("'GetLogInfoResponseEncoder' insufficient buffer writing description"));

            buffer[bu + 2] = 0;
            BitOperations.Copy16Shift(bu + 1, buffer, !msbf);
            return bu + 3;
        }
    }
}
