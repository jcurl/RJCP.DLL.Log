namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Args;
    using RJCP.Core;
    using Text;

    /// <summary>
    /// Decode a verbose payload that contains a string.
    /// </summary>
    public class StringArgDecoder : IVerboseArgDecoder
    {
        private readonly Decoder m_Utf8Decoder = Encoding.UTF8.GetDecoder();
        private readonly char[] m_CharResult = new char[ushort.MaxValue];

        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian. The length of the string is expected to be coding with this endianness.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            const int DataOffset = DltConstants.TypeInfo.TypeInfoSize + 2;

            arg = null;
            if ((typeInfo & DltConstants.TypeInfo.VariableInfo) != 0) {
                Log.Dlt.TraceEvent(TraceEventType.Information, "String argument with unsupported type info of 0x{0:x}", typeInfo);
                return -1;
            }

            StringEncodingType coding =
                (StringEncodingType)((typeInfo & DltConstants.TypeInfo.CodingMask) >> DltConstants.TypeInfo.CodingBitShift);

            if (buffer.Length < DataOffset) {
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "String argument with insufficient buffer length of {0} (minimum {1})", buffer.Length, DataOffset);
                return -1;
            }

            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(
                buffer[DltConstants.TypeInfo.TypeInfoSize..DataOffset], !msbf));

            if (buffer.Length < DataOffset + payloadLength) {
                Log.Dlt.TraceEvent(TraceEventType.Warning,
                    "String argument with insufficient buffer length of {0} (expected {1})",
                    buffer.Length, DataOffset + payloadLength);
                return -1;
            }

            int strLength = payloadLength;
            if (strLength > 0 && buffer[DataOffset + payloadLength - 1] == '\0')
                strLength--;

            string data;
            if (strLength == 0) {
                data = string.Empty;
            } else {

                int cu;
                switch (coding) {
                case StringEncodingType.Ascii:
                    cu = Iso8859_15.Convert(buffer[DataOffset..(DataOffset + strLength)], m_CharResult);
                    break;
                default:
                    m_Utf8Decoder.Convert(buffer[DataOffset..(DataOffset + strLength)], m_CharResult.AsSpan(), true, out _, out cu, out _);
                    break;
                }
                data = new string(m_CharResult, 0, cu);
            }
            arg = new StringDltArg(data, coding);
            return DataOffset + payloadLength;
        }
    }
}
