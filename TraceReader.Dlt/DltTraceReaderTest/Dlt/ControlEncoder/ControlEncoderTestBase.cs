namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using System.IO;
    using ArgEncoder;
    using ControlArgs;
    using Encoder;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

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

            switch (m_EncoderType) {
            case EncoderType.TraceEncoder:
                HeaderLen = 22;
                break;
            case EncoderType.TraceWriter:
                HeaderLen = 22;
                IsWriter = true;
                break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the endianness is big endian or not.
        /// </summary>
        /// <value>Is <see langword="true"/> if Big Endian; otherwise, <see langword="false"/>.</value>
        protected bool IsBigEndian { get { return m_Endianness == Endianness.Big; } }

        /// <summary>
        /// Gets the length of the header, extra space needed at the start of the buffer when encoding.
        /// </summary>
        /// <value>The length of the header.</value>
        protected int HeaderLen { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is using a writer.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this instance is using a writer; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// A writer cannot check for invalid buffer sizes, as it writes to a stream that is theoretically infinite
        /// buffer size.
        /// </remarks>
        protected bool IsWriter { get; }

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
            byte[] buffer = new byte[
                (IsWriter ? DltFileTraceEncoderTest.StorageHeader.Length : 0) +
                HeaderLen + expLen];
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
            IDltLineBuilder builder = new DltLineBuilder();
            builder
                .SetControlPayload(arg)
                .SetDltType(arg.DefaultType)
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDeviceTimeStamp(0)
                .SetBigEndian(IsBigEndian);
            DltControlTraceLine line = (DltControlTraceLine)builder.GetResult();

            switch (m_EncoderType) {
            case EncoderType.Argument:
                IControlArgEncoder encoder = GetEncoder();
                result = encoder.Encode(buffer, IsBigEndian, arg);
                if (result == -1) return Array.Empty<byte>();
                return buffer[..result];
            case EncoderType.Arguments:
                IDltEncoder<DltControlTraceLine> dltEncoder = new ControlDltEncoder(GetEncoder());
                result = dltEncoder.Encode(buffer, line);
                if (result == -1) return Array.Empty<byte>();
                return buffer[..result];
            case EncoderType.TraceEncoder:
                ITraceEncoderFactory<DltTraceLineBase> encFactory = new DltTraceEncoderFactory();
                ITraceEncoder<DltTraceLineBase> lineEncoder = encFactory.Create();
                result = lineEncoder.Encode(buffer, line);
                if (result == -1) return Array.Empty<byte>();
                return buffer[HeaderLen..result];
            case EncoderType.TraceWriter:
                // Note, in this test case we allocate the buffer when encoding, and only copy the result.

                string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "write", "control", IsBigEndian ? "big" : "little");
                string fileName = Path.Combine(dir, $"{Deploy.TestName}.dlt");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (MemoryStream stream = new MemoryStream())
                using (ITraceWriter<DltTraceLineBase> writer = new DltTraceWriter(stream))
                using (FileStream file = GetFile(fileName)) {
                    stream.Write(DltFileTraceEncoderTest.StorageHeader);
                    bool success = writer.WriteLineAsync(line).GetAwaiter().GetResult();
                    if (!success) {
                        result = -1;
                        return Array.Empty<byte>();
                    }
                    long end = stream.Position;
                    stream.Seek(0, SeekOrigin.Begin);
                    if (file is object) {
                        stream.CopyTo(file);
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    int rpos = 0;
                    while (rpos < end) {
                        int read = stream.Read(buffer);
                        Assert.That(read, Is.Not.EqualTo(0));
                        rpos += read;
                    }
                    result = rpos;
                    return buffer[(HeaderLen + DltFileTraceEncoderTest.StorageHeader.Length)..result];
                }
            default:
                throw new NotImplementedException();
            }
        }

        private static FileStream GetFile(string fileName)
        {
            try {
                return new FileStream(fileName, FileMode.Create);
            } catch (IOException) {
                return null;
            }
        }
    }
}
