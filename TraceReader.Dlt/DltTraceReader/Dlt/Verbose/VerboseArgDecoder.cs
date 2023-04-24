namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;

    /// <summary>
    /// Decoder that knows how to decode verbose arguments.
    /// </summary>
    /// <remarks>
    /// This class inspects the contents of the verbose argument and calls the appropriate specialized decoder to decode
    /// the argument. This decoder can be used for decoding any kind of verbose argument.
    /// </remarks>
    public sealed class VerboseArgDecoder : IVerboseArgDecoder
    {
        private const int TypeInfoMask = 0x67F0;
        private const int BoolType = 0x0010;
        private const int SignedIntegerType = 0x0020;
        private const int UnsignedIntegerType = 0x0040;
        private const int FloatType = 0x0080;
        private const int StringType = 0x0200;
        private const int RawType = 0x0400;

        private readonly BoolArgDecoder m_BoolArgDecoder = new BoolArgDecoder();
        private readonly SignedIntArgDecoder m_SignedIntArgDecoder = new SignedIntArgDecoder();
        private readonly UnsignedIntArgDecoder m_UnsignedIntArgDecoder = new UnsignedIntArgDecoder();
        private readonly FloatArgDecoder m_FloatArgDecoder = new FloatArgDecoder();
        private readonly StringArgDecoder m_StringArgDecoder = new StringArgDecoder();
        private readonly RawArgDecoder m_RawArgDecoder = new RawArgDecoder();

        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            switch (typeInfo & TypeInfoMask) {
            case BoolType:
                return m_BoolArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            case SignedIntegerType:
                return m_SignedIntArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            case UnsignedIntegerType:
                return m_UnsignedIntArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            case FloatType:
                return m_FloatArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            case StringType:
                return m_StringArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            case RawType:
                return m_RawArgDecoder.Decode(typeInfo, buffer, msbf, out arg);
            default:
                return DltArgError.Get("unknown type info", out arg);
            }
        }
    }
}
