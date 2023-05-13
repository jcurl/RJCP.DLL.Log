namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;

    /// <summary>
    /// Top level class for serializing and encoding an argument to a buffer.
    /// </summary>
    public class DltArgEncoder : IArgEncoder
    {
        private readonly BinaryIntArgEncoder m_BinaryEncoder = new BinaryIntArgEncoder();
        private readonly HexIntArgEncoder m_HexEncoder = new HexIntArgEncoder();
        private readonly SignedIntArgEncoder m_SignedIntEncoder = new SignedIntArgEncoder();
        private readonly UnsignedIntArgEncoder m_UnsignedIntEncoder = new UnsignedIntArgEncoder();
        private readonly BoolArgEncoder m_BoolEncoder = new BoolArgEncoder();
        private readonly Float32ArgEncoder m_Float32Encoder = new Float32ArgEncoder();
        private readonly Float64ArgEncoder m_Float64Encoder = new Float64ArgEncoder();
        private readonly StringArgEncoder m_StringEncoder = new StringArgEncoder();
        private readonly RawArgEncoder m_RawEncoder = new RawArgEncoder();
        private readonly NonVerboseArgEncoder m_NonVerboseEncoder = new NonVerboseArgEncoder();
        private readonly UnknownVerboseArgEncoder m_UnknownVerboseEncoder = new UnknownVerboseArgEncoder();

        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the argument to.</param>
        /// <param name="verbose">If the argument encoding should include the type information.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        public int Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg)
        {
            switch (arg) {
            case BinaryIntDltArg binaryArg:
                return m_BinaryEncoder.Encode(buffer, verbose, msbf, binaryArg);
            case HexIntDltArg hexArg:
                return m_HexEncoder.Encode(buffer, verbose, msbf, hexArg);
            case SignedIntDltArg signedArg:
                return m_SignedIntEncoder.Encode(buffer, verbose, msbf, signedArg);
            case UnsignedIntDltArg unsignedArg:
                return m_UnsignedIntEncoder.Encode(buffer, verbose, msbf, unsignedArg);
            case BoolDltArg boolArg:
                return m_BoolEncoder.Encode(buffer, verbose, msbf, boolArg);
            case Float32DltArg float32Arg:
                return m_Float32Encoder.Encode(buffer, verbose, msbf, float32Arg);
            case Float64DltArg float64Arg:
                return m_Float64Encoder.Encode(buffer, verbose, msbf, float64Arg);
            case StringDltArg stringArg:
                return m_StringEncoder.Encode(buffer, verbose, msbf, stringArg);
            case RawDltArg rawArg:
                return m_RawEncoder.Encode(buffer, verbose, msbf, rawArg);
            case NonVerboseDltArg nvArg:
                return m_NonVerboseEncoder.Encode(buffer, verbose, msbf, nvArg);
            case UnknownVerboseDltArg unknownVerboseArg:
                return m_UnknownVerboseEncoder.Encode(buffer, verbose, msbf, unknownVerboseArg);
            case UnknownNonVerboseDltArg unknownNonVerboseArg:
                return m_NonVerboseEncoder.Encode(buffer, verbose, msbf, unknownNonVerboseArg);
            default:
                return -1;
            }
        }
    }
}
