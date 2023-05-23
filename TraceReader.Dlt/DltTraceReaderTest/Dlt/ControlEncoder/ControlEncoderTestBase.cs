namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;

    public abstract class ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        private readonly EncoderType m_EncoderType;
        private readonly Endianness m_Endianness;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlEncoderTestBase{TControlEncoder}"/> class.
        /// </summary>
        /// <param name="encoderType">The style of test case for encoding.</param>
        /// <param name="endianness">The endianness of the output when encoding.</param>
        protected ControlEncoderTestBase(EncoderType encoderType, Endianness endianness)
        {
            m_EncoderType = encoderType;
            m_Endianness = endianness;
        }

        /// <summary>
        /// Gets a value indicating whether the endianness is big endian or not.
        /// </summary>
        /// <value>Is <see langword="true"/> if Big Endian; otherwise, <see langword="false"/>.</value>
        protected bool IsBigEndian { get { return m_Endianness == Endianness.Big; } }

        private static TControlEncoder GetEncoder()
        {
            return Activator.CreateInstance<TControlEncoder>();
        }

        /// Encodes the arguments in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="arg">The argument to encode.</param>
        /// <param name="result">The result of the encoding operation (actual length)</param>
        /// <returns>The result and the buffer where the argument is encoded to.</returns>
        protected Span<byte> ControlEncode(IControlArg arg, int expLen)
        {
            byte[] buffer = new byte[expLen];
            return ControlEncode(buffer, arg, out _);
        }

        /// <summary>
        /// Encodes the control argument in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="arg">The argument to encode.</param>
        /// <param name="result">The result of the encoding operation (actual length).</param>
        /// <returns>The result and the buffer where the argument is encoded to.</returns>
        protected Span<byte> ControlEncode(Span<byte> buffer, IControlArg arg, out int result)
        {
            switch (m_EncoderType) {
            case EncoderType.Argument:
                IControlArgEncoder encoder = GetEncoder();
                result = encoder.Encode(buffer, IsBigEndian, arg);
                if (result == -1) return Array.Empty<byte>();
                return buffer[..result];
            default:
                throw new NotImplementedException();
            }
        }
    }
}
