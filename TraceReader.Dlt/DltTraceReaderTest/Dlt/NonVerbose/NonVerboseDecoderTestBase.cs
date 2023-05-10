namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.Core;

    public abstract class NonVerboseDecoderTestBase<TArgDecoder> : DecoderTestBase
        where TArgDecoder : INonVerboseArgDecoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseDecoderTestBase{TArgDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="endian">Indicates the endianness flag to set in the standard header.</param>
        /// <param name="map">The <see cref="IFrameMap"/> that should be used for decoding non-verbose messages.</param>
        protected NonVerboseDecoderTestBase(DecoderType decoderType, Endianness endian, IFrameMap map)
            : base(decoderType, endian, map) { }

        /// <summary>
        /// A custom factory for creating our TraceReader.
        /// </summary>
        /// <value>The custom factory.</value>
        protected virtual DltFactory CustomFactory { get { return null; } }

        /// <summary>
        /// Gets the custom argument decoder.
        /// </summary>
        /// <value>The custom argument decoder.</value>
        protected virtual INonVerboseDltDecoder CustomDecoder { get { return new NonVerboseDltDecoder(Map); } }

        /// <summary>
        /// Decodes the specified DLT type.
        /// </summary>
        /// <param name="map">The <see cref="IFrameMap"/> decoder that should be used.</param>
        /// <param name="messageId">The message ID that should be decoded.</param>
        /// <param name="data">The data.</param>
        /// <param name="fileName">For the <see cref="DecoderType.Line"/>, specify the file name to write to.</param>
        /// <param name="arg">The argument.</param>
        /// <exception cref="NotImplementedException">
        /// The requested decoder type <see cref="Type"/> used when instantiating this class is unknown.
        /// </exception>
        protected void Decode(int messageId, byte[] data, string fileName, out IDltArg arg)
        {
            byte[] packetData = new byte[data.Length + 4];
            bool littleEndian = Endian == Endianness.Little;
            BitOperations.Copy32Shift(messageId, packetData.AsSpan(), littleEndian);
            Buffer.BlockCopy(data, 0, packetData, 4, data.Length);

            switch (Type) {
            case DecoderType.Line:
                DecodeLine(packetData, fileName, true, out arg);
                break;
            case DecoderType.Packet:
                DecodePacket(packetData, true, out arg);
                break;
            case DecoderType.Specialized:
                DecodeSpecialized(messageId, data, true, out arg);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        protected void DecodeIsInvalid(int messageId, byte[] data, string fileName)
        {
            byte[] packetData = new byte[data.Length + 4];
            bool littleEndian = Endian == Endianness.Little;
            BitOperations.Copy32Shift(messageId, packetData.AsSpan(), littleEndian);
            Buffer.BlockCopy(data, 0, packetData, 4, data.Length);

            switch (Type) {
            case DecoderType.Line:
                DecodeLine(packetData, fileName, false, out _);
                break;
            case DecoderType.Packet:
                DecodePacket(packetData, false, out _);
                break;
            case DecoderType.Specialized:
                DecodeSpecialized(messageId, data, false, out _);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void DecodeLine(byte[] data, string fileName, bool isValid, out IDltArg arg)
        {
            DltTraceLineBase line = DecodeLine(CustomFactory, DltType.LOG_INFO, data, fileName);

            if (isValid) {
                Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
                DltNonVerboseTraceLine message = (DltNonVerboseTraceLine)line;
                Assert.That(message.Arguments, Has.Count.EqualTo(1));
                arg = message.Arguments[0];
            } else {
                Assert.That(line, Is.TypeOf<DltSkippedTraceLine>().Or.TypeOf<DltNonVerboseTraceLine>());
                if (line is DltNonVerboseTraceLine nvLine) {
                    // Ensure that if there was an error, we revert to the fallback decoder.
                    Assert.That(nvLine.Arguments, Has.Count.EqualTo(1));
                    Assert.That(nvLine.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                }
                arg = null;
            }
        }

        protected override void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf)
        {
            factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), data).BigEndian(msbf).Append();
        }

        private void DecodePacket(byte[] data, bool isValid, out IDltArg arg)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(DltType.LOG_INFO);
            lineBuilder.SetNumberOfArgs(1);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            int length = CustomDecoder.Decode(data, lineBuilder);
            if (isValid) {
                Assert.That(length, Is.EqualTo(data.Length));
                Assert.That(lineBuilder.Arguments, Has.Count.EqualTo(1));
                arg = lineBuilder.Arguments[0];
            } else {
                if (length != -1) {
                    // Ensure that if there was an error, we revert to the fallback decoder.
                    Assert.That(lineBuilder.Arguments, Has.Count.EqualTo(1));
                    Assert.That(lineBuilder.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
                }
                arg = null;
            }
        }

        private void DecodeSpecialized(int messageId, byte[] data, bool isValid, out IDltArg arg)
        {
            TArgDecoder argDecoder = CreateArgDecoder(messageId);

            bool isBig = Endian == Endianness.Big;
            IFrame frame = Map.GetFrame(messageId, null, null, null);
            IPdu pdu = frame.Arguments[0];

            int length = argDecoder.Decode(data, isBig, pdu, out arg);
            if (isValid) {
                Assert.That(length, Is.EqualTo(data.Length));
            } else {
                Assert.That(length, Is.EqualTo(-1));
                Assert.That(arg, Is.Null.Or.TypeOf<DltArgError>());
            }
        }

        protected virtual TArgDecoder CreateArgDecoder(int messageId)
        {
            return Activator.CreateInstance<TArgDecoder>();
        }
    }
}
