﻿namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using System.Text;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT string.
    /// </summary>
    public sealed class StringArgEncoder : IArgEncoder
    {
        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the argument to.</param>
        /// <param name="verbose">If the argument encoding should include the type information.</param>
        /// <param name="msbf">If <see langword="true" /> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        public int Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg)
        {
            StringDltArg strArg = (StringDltArg)arg;
            int typeInfo = DltConstants.TypeInfo.TypeString +
                ((int)strArg.Coding << DltConstants.TypeInfo.CodingBitShift);

            int len = verbose ? 4 : 0;
            if (buffer.Length < len + 2) return -1;

            Encoder encoder;
            switch (strArg.Coding) {
            case StringEncodingType.Ascii:
                encoder = Encoding.ASCII.GetEncoder();
                break;
            case StringEncodingType.Utf8:
                encoder = Encoding.UTF8.GetEncoder();
                break;
            default:
                return -1;
            }

            int maxLen = Math.Min(buffer.Length, 65536);
            Span<byte> payload = buffer[len..maxLen];
            encoder.Convert(strArg.Data, payload[2..], true, out _, out int bu, out bool completed);
            if (!completed) return -1;
            BitOperations.Copy16Shift(bu, payload[0..], !msbf);

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + 2 + bu;
        }
    }
}
