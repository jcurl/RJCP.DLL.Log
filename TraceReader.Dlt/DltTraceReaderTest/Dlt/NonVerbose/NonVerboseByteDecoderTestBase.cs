namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Dlt.Packet;
    using NUnit.Framework;

    public abstract class NonVerboseByteDecoderTestBase : DecoderTestBase
    {
        protected NonVerboseByteDecoderTestBase(DecoderType decoderType, Endianness endian) : base(decoderType, endian) { }

        protected void Decode(byte[] data, string fileName, int messageId, out DltNonVerboseTraceLine line)
        {
            switch (Type) {
            case DecoderType.Line:
                DecodeLine(data, fileName, messageId, out line);
                break;
            case DecoderType.Packet:
                DecodePacket(data, out line);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void DecodeLine(byte[] data, string fileName, int messageId, out DltNonVerboseTraceLine line)
        {
            DltTraceLineBase baseLine = DecodeLine(null, DltType.UNKNOWN, data, fileName);
            Assert.That(baseLine, Is.TypeOf<DltNonVerboseTraceLine>());
            line = (DltNonVerboseTraceLine)baseLine;
            Assert.That(line.MessageId, Is.EqualTo(messageId));
            Assert.That(line.Arguments.Count, Is.EqualTo(1));
        }

        protected override void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf)
        {
            factory.NonVerbose(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), data).BigEndian(msbf).Append();
        }

        private void DecodePacket(byte[] data, out DltNonVerboseTraceLine line)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetIsVerbose(false);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            INonVerboseDltDecoder dltDecoder = new NonVerboseByteDecoder();
            int length = dltDecoder.Decode(data, lineBuilder);

            Assert.That(length, Is.EqualTo(data.Length));
            Assert.That(lineBuilder.Arguments.Count, Is.EqualTo(1));
            line = (DltNonVerboseTraceLine)lineBuilder.GetResult();
        }
    }
}
