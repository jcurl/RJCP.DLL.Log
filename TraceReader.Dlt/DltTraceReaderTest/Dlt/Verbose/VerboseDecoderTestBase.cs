namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Args;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core;

    /// <summary>
    /// Common test code for verbose argument decoders.
    /// </summary>
    /// <typeparam name="TArgDecoder">The type of the verbose argument decoder.</typeparam>
    /// <remarks>
    /// The implementation of this base class is similar (but not identical to)
    /// <see cref="Control.ControlDecoderTestBase{TReqDecoder, TResDecoder}"/>. It is kept separate as decoders are
    /// different.
    /// </remarks>
    public abstract class VerboseDecoderTestBase<TArgDecoder>
        where TArgDecoder : IVerboseArgDecoder
    {
        public enum DecoderType
        {
            Line,
            Packet,
            Specialized,
        }

        public enum Endianness
        {
            Little = 1,
            Big = 2
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseDecoderTestBase{TArgDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="endian">Indicates the endianness flag to set in the standard header.</param>
        /// <param name="argType">Type of the argument that is expected to be generated.</param>
        protected VerboseDecoderTestBase(DecoderType decoderType, Endianness endian)
        {
            Type = decoderType;
            Endian = endian;
        }

        protected DecoderType Type { get; }
        protected Endianness Endian { get; }

        /// <summary>
        /// Decodes the specified DLT type.
        /// </summary>
        /// <param name="dltType">Type of the DLT argument.</param>
        /// <param name="data">The data.</param>
        /// <param name="fileName">For the <see cref="DecoderType.Line"/>, specify the file name to write to.</param>
        /// <param name="arg">The argument.</param>
        /// <exception cref="NotImplementedException">
        /// The requested decoder type <see cref="Type"/> used when instantiating this class is unknown.
        /// </exception>
        protected void Decode(byte[] data, string fileName, out IDltArg arg)
        {
            switch (Type) {
            case DecoderType.Line:
                DecodeLine(data, fileName, true, out arg);
                break;
            case DecoderType.Packet:
                DecodePacket(data, true, out arg);
                break;
            case DecoderType.Specialized:
                DecodeSpecialized(data, true, out arg);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        protected void DecodeIsInvalid(byte[] data, string fileName)
        {
            switch (Type) {
            case DecoderType.Line:
                DecodeLine(data, fileName, false, out _);
                break;
            case DecoderType.Packet:
                DecodePacket(data, false, out _);
                break;
            case DecoderType.Specialized:
                DecodeSpecialized(data, false, out _);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void DecodeLine(byte[] data, string fileName, bool isValid, out IDltArg arg)
        {
            DltFactory factory = new DltFactory(DltFactoryType.File);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                bool isBig = Endian == Endianness.Big;
                factory.Generate(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, 1, data).BigEndian(isBig).Append();
                using (Stream stream = writer.Stream()) {
                    if (!string.IsNullOrEmpty(fileName)) {
                        string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "message", isBig ? "big" : "little");
                        string outPath = Path.Combine(dir, $"{fileName}.dlt");
                        if (!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }
                        writer.Write(outPath);
                    }

                    Task<DltTraceLineBase> lineTask = WriteDltPacket(factory, stream);
                    DltTraceLineBase line = lineTask.GetAwaiter().GetResult();
                    if (isValid) {
                        Assert.That(line, Is.TypeOf<DltTraceLine>());
                        DltTraceLine message = (DltTraceLine)line;
                        Assert.That(message.Arguments.Count, Is.EqualTo(1));
                        arg = message.Arguments[0];
                    } else {
                        Assert.That(line, Is.TypeOf<DltSkippedTraceLine>());
                        arg = null;
                    }
                }
            }
        }

        private static async Task<DltTraceLineBase> WriteDltPacket(DltFactory factory, Stream stream)
        {
            DltTraceLineBase line;
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                line = await reader.GetLineAsync();

                // This is the only way to check the length, that this was the only packet decoded.
                DltTraceLineBase lastLine = await reader.GetLineAsync();
                Assert.That(lastLine, Is.Null);
                return line;
            }
        }

        private void DecodePacket(byte[] data, bool isValid, out IDltArg arg)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(DltType.LOG_INFO);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            IVerboseDltDecoder dltDecoder = new VerboseDltDecoder(new VerboseArgDecoder());
            int length = dltDecoder.Decode(data, lineBuilder);
            if (isValid) {
                Assert.That(length, Is.EqualTo(data.Length));
                Assert.That(lineBuilder.Arguments.Count, Is.EqualTo(1));
                arg = lineBuilder.Arguments[0];
            } else {
                Assert.That(length, Is.EqualTo(-1));
                arg = null;
            }
        }

        private void DecodeSpecialized(byte[] data, bool isValid, out IDltArg arg)
        {
            IVerboseArgDecoder argDecoder = Activator.CreateInstance<TArgDecoder>();

            bool isBig = Endian == Endianness.Big;
            int typeInfo = BitOperations.To32Shift(data, !isBig);
            int length = argDecoder.Decode(typeInfo, data, isBig, out arg);
            if (isValid) {
                Assert.That(length, Is.EqualTo(data.Length));
            } else {
                Assert.That(length, Is.EqualTo(-1));
                Assert.That(arg, Is.Null);
            }
        }
    }
}
