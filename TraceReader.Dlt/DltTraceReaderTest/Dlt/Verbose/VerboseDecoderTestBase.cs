namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.Core;

    /// <summary>
    /// Common test code for verbose argument decoders.
    /// </summary>
    /// <typeparam name="TArgDecoder">The type of the verbose argument decoder.</typeparam>
    public abstract class VerboseDecoderTestBase<TArgDecoder> : DecoderTestBase
        where TArgDecoder : IVerboseArgDecoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseDecoderTestBase{TArgDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="endian">Indicates the endianness flag to set in the standard header.</param>
        protected VerboseDecoderTestBase(DecoderType decoderType, Endianness endian) : base(decoderType, endian) { }

        /// <summary>
        /// Decodes the specified DLT type.
        /// </summary>
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
            DltTraceLineBase line = DecodeLine(null, DltType.LOG_INFO, data, fileName);

            if (isValid) {
                Assert.That(line, Is.TypeOf<DltTraceLine>());
                DltTraceLine message = (DltTraceLine)line;
                Assert.That(message.Arguments, Has.Count.EqualTo(1));
                arg = message.Arguments[0];
            } else {
                Assert.That(line, Is.TypeOf<DltSkippedTraceLine>());
                arg = null;
            }
        }

        protected override void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf)
        {
            factory.Verbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), DltType.LOG_INFO, 1, data).BigEndian(msbf).Append();
        }

        private void DecodePacket(byte[] data, bool isValid, out IDltArg arg)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(DltType.LOG_INFO);
            lineBuilder.SetNumberOfArgs(1);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            IVerboseDltDecoder dltDecoder = new VerboseDltDecoder(new VerboseArgDecoder());
            Result<int> length = dltDecoder.Decode(data, lineBuilder);
            if (isValid) {
                Assert.That(length.Value, Is.EqualTo(data.Length));
                Assert.That(lineBuilder.Arguments, Has.Count.EqualTo(1));
                arg = lineBuilder.Arguments[0];
            } else {
                Assert.That(length.HasValue, Is.False);
                arg = null;
            }
        }

        private void DecodeSpecialized(byte[] data, bool isValid, out IDltArg arg)
        {
            IVerboseArgDecoder argDecoder = Activator.CreateInstance<TArgDecoder>();

            bool isBig = Endian == Endianness.Big;
            int typeInfo = BitOperations.To32Shift(data, !isBig);
            Result<int> length = argDecoder.Decode(typeInfo, data, isBig, out arg);
            if (isValid) {
                Assert.That(length.Value, Is.EqualTo(data.Length));
            } else {
                Assert.That(length.HasValue, Is.False);
                Assert.That(arg, Is.Null);
            }
        }
    }
}
