namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using Decoder;
    using NUnit.Framework;

    public class CustomSwInjectionRequest : SwInjectionRequest
    {
        public CustomSwInjectionRequest(int serviceId, byte[] payLoad)
            : base(serviceId, payLoad)
        { }
    }

    public class CustomSwInjectionRequestDecoder : SwInjectionRequestDecoder
    {
        protected override int Decode(int serviceId, int length, ReadOnlySpan<byte> buffer, out IControlArg service)
        {
            service = new CustomSwInjectionRequest(serviceId, buffer.ToArray());
            return buffer.Length;
        }
    }

    public class CustomDltDecoder : ControlDltDecoder
    {
        public CustomDltDecoder()
        {
            RegisterRequest(0x1011, new CustomSwInjectionRequestDecoder());
        }
    }

    public class CustomDltFileTraceDecoder : DltFileTraceDecoder
    {
        public CustomDltFileTraceDecoder()
            : base(GetVerboseDecoder(), new CustomDltDecoder(), new DltLineBuilder()) { }
    }

    public class CustomDltFileTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            return new CustomDltFileTraceDecoder();
        }
    }

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SwInjectionCustomClassTest : ControlDecoderTestBase<CustomSwInjectionRequestDecoder, SwInjectionResponseDecoder>
    {
        public SwInjectionCustomClassTest(DecoderType decoderType)
            : base(decoderType, 0x1011, typeof(CustomSwInjectionRequest), typeof(SwInjectionResponse),
                  new DltFactory(DltFactoryType.File, new CustomDltFileTraceReaderFactory()),
                  new CustomDltDecoder())
        { }

        [Test]
        public void CustomReq()
        {
            byte[] payload = new byte[] {
                0x11, 0x10, 0x00, 0x00,
                0x05, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55
            };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1011_CustomSwInjectionRequest", out IControlArg service);

            CustomSwInjectionRequest request = (CustomSwInjectionRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[] 05 00 00 00 11 22 33 44 55"));
            Assert.That(request.Payload.Length, Is.EqualTo(5));
            Assert.That(request.Payload, Is.EqualTo(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55 }));
        }
    }
}
