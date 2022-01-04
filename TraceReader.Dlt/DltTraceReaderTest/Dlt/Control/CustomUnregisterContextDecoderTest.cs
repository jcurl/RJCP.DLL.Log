namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class CustomUnregisterContextDecoderTest : ControlDecoderTestBase<NoDecoder, CustomUnregisterContextResponseDecoder>
    {
        public CustomUnregisterContextDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0F01, null, typeof(CustomUnregisterContextResponse))
        { }

        [TestCase(0x00, "[unregister_context ok] APP1 (CTX1) eth0")]
        [TestCase(0x01, "[unregister_context not_supported] APP1 (CTX1) eth0")]
        [TestCase(0x02, "[unregister_context error] APP1 (CTX1) eth0")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x0F, 0x00, 0x00, status,
                0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF01_CustomUnregisterContextResponse_{status:x2}", out IControlArg service);

            CustomUnregisterContextResponse response = (CustomUnregisterContextResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(response.ContextId, Is.EqualTo("CTX1"));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, "[unregister_context ok] APP1 (CTX1)")]
        [TestCase(0x01, "[unregister_context not_supported] APP1 (CTX1)")]
        [TestCase(0x02, "[unregister_context error] APP1 (CTX1)")]
        public void DecodeResponseNoComId(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x0F, 0x00, 0x00, status,
                0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF01_CustomUnregisterContextResponse_NoComId_{status:x2}", out IControlArg service);

            CustomUnregisterContextResponse response = (CustomUnregisterContextResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(response.ContextId, Is.EqualTo("CTX1"));
            Assert.That(response.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0x00, "[unregister_context ok] eth0")]
        [TestCase(0x01, "[unregister_context not_supported] eth0")]
        [TestCase(0x02, "[unregister_context error] eth0")]
        public void DecodeResponseNoAppCtx(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x0F, 0x00, 0x00, status,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF01_CustomUnregisterContextResponse_NoAppCtx_{status:x2}", out IControlArg service);

            CustomUnregisterContextResponse response = (CustomUnregisterContextResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(response.ContextId, Is.EqualTo(string.Empty));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, "[unregister_context ok] APP1 () eth0")]
        [TestCase(0x01, "[unregister_context not_supported] APP1 () eth0")]
        [TestCase(0x02, "[unregister_context error] APP1 () eth0")]
        public void DecodeResponseNoCtx(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x0F, 0x00, 0x00, status,
                0x41, 0x50, 0x50, 0x31, 0x00, 0x00, 0x00, 0x00, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF01_CustomUnregisterContextResponse_NoCtx_{status:x2}", out IControlArg service);

            CustomUnregisterContextResponse response = (CustomUnregisterContextResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(response.ContextId, Is.EqualTo(string.Empty));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
        }
    }
}
