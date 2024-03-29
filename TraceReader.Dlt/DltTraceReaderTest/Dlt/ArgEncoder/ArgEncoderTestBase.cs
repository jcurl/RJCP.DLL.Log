﻿namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using System.IO;
    using Args;
    using Encoder;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core;

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
        /// Gets a value indicating whether lines should be encoded as verbose
        /// </summary>
        /// <value>I <see langword="true"/> if lines should be encoded as verbose; otherwise, <see langword="false"/>.</value>
        protected bool IsVerbose { get { return m_LineType == LineType.Verbose; } }

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
            byte[] buffer = new byte[
                (IsWriter ? DltFileTraceEncoderTest.StorageHeader.Length : 0) +
                (IsVerbose ? 4 : 0) + HeaderLen + expLen];
            return ArgEncode(buffer, arg, out _);
        }

        /// <summary>
        /// Encodes the arguments in to the buffer specified, using the style of the test case.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="arg">The argument to encode.</param>
        /// <param name="result">The result of the encoding operation (actual length).</param>
        /// <returns>The result and the buffer where the argument is encoded to.</returns>
        protected Span<byte> ArgEncode(Span<byte> buffer, IDltArg arg, out Result<int> result)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder
                .SetNumberOfArgs(1)
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDeviceTimeStamp(0)
                .SetBigEndian(IsBigEndian)
                .SetIsVerbose(true)
                .AddArgument(arg);
            DltTraceLine line = (DltTraceLine)builder.GetResult();

            switch (m_EncoderType) {
            case EncoderType.Argument:
                IArgEncoder encoder = GetEncoder();
                result = encoder.Encode(buffer, IsVerbose, IsBigEndian, arg);
                if (!result.HasValue) return Array.Empty<byte>();
                return buffer[..result.Value];
            case EncoderType.Arguments:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                IDltEncoder<DltTraceLine> dltEncoder = new VerboseDltEncoder(GetEncoder());
                result = dltEncoder.Encode(buffer, line);
                if (!result.HasValue) return Array.Empty<byte>();
                return buffer[..result.Value];
            case EncoderType.TraceEncoder:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                ITraceEncoderFactory<DltTraceLineBase> encFactory = new DltTraceEncoderFactory();
                ITraceEncoder<DltTraceLineBase> lineEncoder = encFactory.Create();
                result = lineEncoder.Encode(buffer, line);
                if (!result.HasValue) return Array.Empty<byte>();
                return buffer[HeaderLen..result.Value];
            case EncoderType.TraceWriter:
                if (!IsVerbose) throw new InvalidOperationException("The EncoderType doesn't support non-verbose");

                string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "write", "verbose", IsBigEndian ? "big" : "little");
                string fileName = Path.Combine(dir, $"{Deploy.TestName}.dlt");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (MemoryStream stream = new())
                using (ITraceWriter<DltTraceLineBase> writer = new DltTraceWriter(stream))
                using (FileStream file = new(fileName, FileMode.Create)) {
                    stream.Write(DltFileTraceEncoderTest.StorageHeader);
                    bool success = writer.WriteLineAsync(line).GetAwaiter().GetResult();
                    if (!success) {
                        result = Result.FromException<int>(new DltEncodeException("Test case WriteLineAsync failed"));
                        return Array.Empty<byte>();
                    }
                    long end = stream.Position;
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(file);
                    stream.Seek(0, SeekOrigin.Begin);
                    int rpos = 0;
                    while (rpos < end) {
                        int read = stream.Read(buffer);
                        Assert.That(read, Is.Not.EqualTo(0));
                        rpos += read;
                    }
                    result = rpos;
                    return buffer[(HeaderLen + DltFileTraceEncoderTest.StorageHeader.Length)..rpos];
                }
            default:
                throw new NotImplementedException();
            }
        }
    }
}
