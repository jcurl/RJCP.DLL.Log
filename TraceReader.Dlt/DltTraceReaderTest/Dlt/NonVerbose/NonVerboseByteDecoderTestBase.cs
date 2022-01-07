namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using Dlt.Packet;
    using NUnit.Framework;

    public abstract class NonVerboseByteDecoderTestBase : DecoderTestBase
    {
        protected NonVerboseByteDecoderTestBase(DecoderType decoderType, Endianness endian) : base(decoderType, endian) { }

        protected void Decode(byte[] data, string fileName, out IDltArg arg)
        {
            switch (Type) {
            case DecoderType.Line:
                DecodeLine(data, fileName, out arg);
                break;
            case DecoderType.Packet:
                DecodePacket(data, out arg);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void DecodeLine(byte[] data, string fileName, out IDltArg arg)
        {
            DltTraceLineBase line = DecodeLine(null, DltType.UNKNOWN, data, fileName);
            Assert.That(line, Is.TypeOf<DltTraceLine>());
            DltTraceLine message = (DltTraceLine)line;
            Assert.That(message.Arguments.Count, Is.EqualTo(1));
            arg = message.Arguments[0];
        }

        protected override void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf)
        {
            factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), data).BigEndian(msbf).Append();
        }

        private void DecodePacket(byte[] data, out IDltArg arg)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(DltType.UNKNOWN);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            INonVerboseDltDecoder dltDecoder = new NonVerboseByteDecoder();
            int length = dltDecoder.Decode(data, lineBuilder);

            Assert.That(length, Is.EqualTo(data.Length));
            Assert.That(lineBuilder.Arguments.Count, Is.EqualTo(1));
            arg = lineBuilder.Arguments[0];
        }
    }
}
