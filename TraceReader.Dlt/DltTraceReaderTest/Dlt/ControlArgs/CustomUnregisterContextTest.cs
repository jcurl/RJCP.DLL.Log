namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class CustomUnregisterContextTest
    {
        [TestCase(ControlResponse.StatusOk, "[unregister_context ok] APP1 (CTX1) eth0")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported] APP1 (CTX1) eth0")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error] APP1 (CTX1) eth0")]
        [TestCase(100, "[unregister_context status=100] APP1 (CTX1) eth0")]
        public void CustomUnregisterContextRes(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "APP1", "CTX1", "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(ControlResponse.StatusOk, "[unregister_context ok] APP1 (CTX1)")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported] APP1 (CTX1)")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error] APP1 (CTX1)")]
        [TestCase(100, "[unregister_context status=100] APP1 (CTX1)")]
        public void CustomUnregisterContextRes_NoComId(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "APP1", "CTX1", "");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(ControlResponse.StatusOk, "[unregister_context ok] APP1 (CTX1)")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported] APP1 (CTX1)")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error] APP1 (CTX1)")]
        [TestCase(100, "[unregister_context status=100] APP1 (CTX1)")]
        public void CustomUnregisterContextRes_NullComId(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "APP1", "CTX1", null);
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(ControlResponse.StatusOk, "[unregister_context ok]  (CTX1) eth0")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported]  (CTX1) eth0")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error]  (CTX1) eth0")]
        [TestCase(100, "[unregister_context status=100]  (CTX1) eth0")]
        public void CustomUnregisterContextRes_NoAppId(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "", "CTX1", "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(ControlResponse.StatusOk, "[unregister_context ok] eth0")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported] eth0")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error] eth0")]
        [TestCase(100, "[unregister_context status=100] eth0")]
        public void CustomUnregisterContextRes_NoAppIdCtxId(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "", "", "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(ControlResponse.StatusOk, "[unregister_context ok] APP1 () eth0")]
        [TestCase(ControlResponse.StatusNotSupported, "[unregister_context not_supported] APP1 () eth0")]
        [TestCase(ControlResponse.StatusError, "[unregister_context error] APP1 () eth0")]
        [TestCase(100, "[unregister_context status=100] APP1 () eth0")]
        public void CustomUnregisterContextRes_NoCtxId(int status, string result)
        {
            CustomUnregisterContextResponse arg = new CustomUnregisterContextResponse(status, "APP1", "", "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }
    }
}
