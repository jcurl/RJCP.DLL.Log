namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using Encoder;

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

            switch (m_EncoderType) {
            case EncoderType.TraceEncoder:
                HeaderLen = 22;
                break;
            }
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

        /// <summary>
        /// Gets the length of the header, extra space needed at the start of the buffer when encoding.
        /// </summary>
        /// <value>The length of the header.</value>
        protected int HeaderLen { get; }

        private static IArgEncoder GetEncoder()
        {
            return Activator.CreateInstance<TArgEncoder>();
        }

        /// <summary>
        /// Encodes the arguments in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="arg">The argument to encode.</param>
        /// <param name="result">The result of the encoding operation (actual length)</param>
        /// <returns>The result and the buffer where the argument is encoded to.</returns>
        protected Span<byte> ArgEncode(IDltArg arg, int expLen)
        {
            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen + expLen];
            return ArgEncode(buffer, arg, out _);
        }

        /// <summary>
        /// Encodes the arguments in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="arg">The argument to encode.</param>
        /// <param name="result">The result of the encoding operation (actual length)</param>
        /// <returns>The result and the buffer where the argument is encoded to.</returns>
        protected Span<byte> ArgEncode(Span<byte> buffer, IDltArg arg, out int result)
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { arg }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                Type = DltType.LOG_INFO,
                DeviceTimeStamp = new TimeSpan(0),
                Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature +
                    DltLineFeatures.DevTimeStampFeature + DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature
            };
            if (IsBigEndian) line.Features += DltLineFeatures.BigEndianFeature;

            switch (m_EncoderType) {
            case EncoderType.Argument:
                IArgEncoder encoder = GetEncoder();
                result = encoder.Encode(buffer, IsVerbose, IsBigEndian, arg);
                if (result == -1) return Array.Empty<byte>();
                return buffer[..result];
            case EncoderType.Arguments:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                IDltEncoder<DltTraceLine> dltEncoder = new VerboseDltEncoder(GetEncoder());
                result = dltEncoder.Encode(buffer, line);
                if (result == -1) return Array.Empty<byte>();
                return buffer[..result];
            case EncoderType.TraceEncoder:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                ITraceEncoderFactory<DltTraceLineBase> encFactory = new DltTraceEncoderFactory();
                ITraceEncoder<DltTraceLineBase> lineEncoder = encFactory.Create();
                result = lineEncoder.Encode(buffer, line);
                if (result == -1) return Array.Empty<byte>();
                return buffer[22..result];
            default:
                throw new NotImplementedException();
            }
        }
    }
}
