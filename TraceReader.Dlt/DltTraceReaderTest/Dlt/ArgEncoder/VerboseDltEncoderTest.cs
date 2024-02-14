namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using Encoder;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(EncoderType.Arguments)]
    [TestFixture(EncoderType.TraceEncoder)]
    public class VerboseDltEncoderTest
    {
        private readonly EncoderType m_EncoderType;

        public VerboseDltEncoderTest(EncoderType encType)
        {
            m_EncoderType = encType;
        }

        private static readonly DltTraceLine TraceLine = new(new IDltArg[] {
            new StringDltArg("Temperature:"),
            new SignedIntDltArg(45, 2)
        }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Type = DltType.LOG_INFO,
            DeviceTimeStamp = new TimeSpan(0),
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature +
                DltLineFeatures.DevTimeStampFeature + DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature
        };

        private Span<byte> Encode(DltTraceLine line, int len)
        {
            switch (m_EncoderType) {
            case EncoderType.Arguments:
                return EncodeArguments(line, len);
            case EncoderType.TraceEncoder:
                return EncodeLine(line, len);
            default:
                throw new NotSupportedException();
            }
        }

        private static Span<byte> EncodeArguments(DltTraceLine line, int len)
        {
            byte[] buffer = new byte[len];
            VerboseDltEncoder encoder = new();

            Result<int> result = encoder.Encode(buffer, line);
            if (!result.HasValue) return Array.Empty<byte>();
            return buffer.AsSpan(0, result.Value);
        }

        private static Span<byte> EncodeLine(DltTraceLine line, int len)
        {
            byte[] buffer = new byte[len + 22];
            ITraceEncoderFactory<DltTraceLineBase> factory = new DltTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, line);
            if (!result.HasValue) return Array.Empty<byte>();
            return buffer.AsSpan(22, result.Value - 22);
        }

        [TestCase(65536)]
        [TestCase(25)]
        public void WriteMultipleArgs(int len)
        {
            Span<byte> encoded = Encode(TraceLine, len);
            Assert.That(encoded.Length, Is.EqualTo(25));
        }

        [TestCase(0)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(23)]
        public void WriteInsufficientBuffer(int len)
        {
            Span<byte> encoded = Encode(TraceLine, len);
            Assert.That(encoded.Length, Is.EqualTo(0));
        }
    }
}
