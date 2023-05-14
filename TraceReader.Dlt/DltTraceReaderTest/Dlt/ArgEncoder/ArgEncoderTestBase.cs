namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;

    /// <summary>
    /// Choose the style of test case for encoding.
    /// </summary>
    public enum EncoderType
    {
        Argument,
        Arguments
    }

    /// <summary>
    /// The endianness of the output when encoding.
    /// </summary>
    public enum Endianness
    {
        Little,
        Big
    }

    public enum LineType
    {
        Verbose,
        NonVerbose
    }

    /// <summary>
    /// Base class containing common code for testing DLT argument encoders.
    /// </summary>
    /// <typeparam name="TArgEncoder">The type of the argument encoder which is instantiated during the test.</typeparam>
    public abstract class ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        private readonly EncoderType m_EncoderType;
        private readonly Endianness m_Endianness;
        private readonly LineType m_LineType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgEncoderTestBase{TArgEncoder}"/> class.
        /// </summary>
        /// <param name="encoderType">The style of test case for encoding.</param>
        /// <param name="endianness">The endianness of the output when encoding.</param>
        protected ArgEncoderTestBase(EncoderType encoderType, Endianness endianness, LineType lineType)
        {
            m_EncoderType = encoderType;
            m_Endianness = endianness;
            m_LineType = lineType;
        }

        /// <summary>
        /// Gets a value indicating whether the endianness is big endian or not.
        /// </summary>
        /// <value>Is <see langword="true"/> if Big Endian; otherwise, <see langword="false"/>.</value>
        protected bool IsBigEndian { get { return m_Endianness == Endianness.Big; } }

        /// <summary>
        /// Gets a value indicating whether lines should be encoded as verbose
        /// </summary>
        /// <value>I <see langword="true"/> if lines should be encoded as verbose; otherwise, <see langword="false"/>.</value>
        protected bool IsVerbose { get { return m_LineType == LineType.Verbose; } }

        private static IArgEncoder GetEncoder()
        {
            return Activator.CreateInstance<TArgEncoder>();
        }

        /// <summary>
        /// Encodes the arguments in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="verbose">Serialise to verbose if <see langword="true"/>, otherwise non-verbose.</param>
        /// <param name="arg">The argument to encode.</param>
        /// <returns>System.Int32.</returns>
        protected int ArgEncode(Span<byte> buffer, IDltArg arg)
        {
            switch (m_EncoderType) {
            case EncoderType.Argument:
                IArgEncoder encoder = GetEncoder();
                return encoder.Encode(buffer, IsVerbose, IsBigEndian, arg);
            case EncoderType.Arguments:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                DltTraceLine line = new DltTraceLine(new IDltArg[] { arg }) {
                    EcuId = "ECU1",
                    ApplicationId = "APP1",
                    ContextId = "CTX1",
                    Type = DltType.LOG_INFO,
                    DeviceTimeStamp = new TimeSpan(0),
                };
                if (IsBigEndian) line.Features += DltLineFeatures.BigEndianFeature;
                IDltEncoder<DltTraceLine> dltEncoder = new VerboseDltEncoder(GetEncoder());
                return dltEncoder.Encode(buffer, line);
            default:
                throw new NotImplementedException();
            }
        }
    }
}
