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
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        /// <remarks>
        /// When encoding, if there is not enough buffer, the function returns -1, so you should encode a second
        /// time, but with a status of <see cref="GetLogInfoResponse.StatusOverflow"/>.
        /// </remarks>
        protected override int EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
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
                return -1;
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
                pos = WriteLogInfoType(buffer, msbf, arg.Status, controlArg.AppIds);
                if (pos == -1) return -1;
            }

            if (buffer.Length < pos + 4) return -1;
            DltTraceEncoder.WriteId(buffer[pos..], controlArg.ComInterface);
            return pos + 4;
        }

        private int WriteLogInfoType(Span<byte> buffer, bool msbf, int status, IList<AppId> apps)
        {
            if (buffer.Length < 2) return -1;

            BitOperations.Copy16Shift(apps.Count, buffer, !msbf);

            int pos = 2;
            foreach (AppId app in apps) {
                int written = WriteAppIds(buffer[pos..], msbf, status, app);
                if (written == -1) return -1;
                pos += written;
            }
            return pos;
        }

        private int WriteAppIds(Span<byte> buffer, bool msbf, int status, AppId app)
        {
            if (buffer.Length < 6) return -1;

            DltTraceEncoder.WriteId(buffer, app.Name);
            BitOperations.Copy16Shift(app.ContextIds.Count, buffer[4..], !msbf);

            int pos = 6;
            foreach (ContextId ctx in app.ContextIds) {
                int written = WriteCtxIds(buffer[pos..], msbf, status, ctx);
                if (written == -1) return -1;
                pos += written;
            }

            if (status == GetLogInfoResponse.StatusFullInfo) {
                int written = WriteDescription(buffer[pos..], msbf, app.Description);
                if (written == -1) return -1;
                pos += written;
            }
            return pos;
        }

        private int WriteCtxIds(Span<byte> buffer, bool msbf, int status, ContextId ctx)
        {
            if (buffer.Length < 4) return -1;

            DltTraceEncoder.WriteId(buffer, ctx.Name);
            int pos = 4;

            if (status == GetLogInfoResponse.StatusWithLogNoTrace ||
                status == GetLogInfoResponse.StatusWithLogWithTrace ||
                status == GetLogInfoResponse.StatusFullInfo) {
                if (buffer.Length < pos + 1) return -1;
                buffer[pos] = unchecked((byte)ctx.LogLevel);
                pos++;
            }

            if (status == GetLogInfoResponse.StatusNoLogWithTrace ||
                status == GetLogInfoResponse.StatusWithLogWithTrace ||
                status == GetLogInfoResponse.StatusFullInfo) {
                if (buffer.Length < pos + 1) return -1;
                buffer[pos] = unchecked((byte)ctx.TraceStatus);
                pos++;
            }

            if (status == GetLogInfoResponse.StatusFullInfo) {
                int written = WriteDescription(buffer[pos..], msbf, ctx.Description);
                if (written == -1) return -1;
                pos += written;
            }
            return pos;
        }

        private int WriteDescription(Span<byte> buffer, bool msbf, string description)
        {
            if (description.Length == 0) {
                if (buffer.Length < 3) return -1;
                BitOperations.Copy16Shift(1, buffer, !msbf);
                buffer[2] = 0;
                return 3;
            }

            if (buffer.Length < 4) return -1;
            m_Utf8Enc.Convert(description, buffer[2..^1], true, out _, out int bu, out bool complete);
            if (!complete) return -1;
            buffer[bu + 2] = 0;
            BitOperations.Copy16Shift(bu + 1, buffer, !msbf);
            return bu + 3;
        }
    }
}
