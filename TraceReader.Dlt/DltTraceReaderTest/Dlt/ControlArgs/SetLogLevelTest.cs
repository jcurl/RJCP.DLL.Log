namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetLogLevelTest
    {
        [TestCase(LogLevel.Default, "[set_log_level] default")]
        [TestCase(LogLevel.Block, "[set_log_level] block_all")]
        [TestCase(LogLevel.Fatal, "[set_log_level] fatal")]
        [TestCase(LogLevel.Error, "[set_log_level] error")]
        [TestCase(LogLevel.Warn, "[set_log_level] warning")]
        [TestCase(LogLevel.Info, "[set_log_level] info")]
        [TestCase(LogLevel.Debug, "[set_log_level] debug")]
        [TestCase(LogLevel.Verbose, "[set_log_level] verbose")]
        [TestCase(100, "[set_log_level] log_level=100")]
        public void SetLogLevelReqAllNull(LogLevel logLevel, string result)
        {
            SetLogLevelRequest arg = new SetLogLevelRequest(null, null, logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
        }

        [TestCase(LogLevel.Default, "[set_log_level] default APP1 ()")]
        [TestCase(LogLevel.Block, "[set_log_level] block_all APP1 ()")]
        [TestCase(LogLevel.Fatal, "[set_log_level] fatal APP1 ()")]
        [TestCase(LogLevel.Error, "[set_log_level] error APP1 ()")]
        [TestCase(LogLevel.Warn, "[set_log_level] warning APP1 ()")]
        [TestCase(LogLevel.Info, "[set_log_level] info APP1 ()")]
        [TestCase(LogLevel.Debug, "[set_log_level] debug APP1 ()")]
        [TestCase(LogLevel.Verbose, "[set_log_level] verbose APP1 ()")]
        [TestCase(100, "[set_log_level] log_level=100 APP1 ()")]
        public void SetLogLevelReqAppId(LogLevel logLevel, string result)
        {
            SetLogLevelRequest arg = new SetLogLevelRequest("APP1", null, logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
        }

        [TestCase(LogLevel.Default, "[set_log_level] default")]
        [TestCase(LogLevel.Block, "[set_log_level] block_all")]
        [TestCase(LogLevel.Fatal, "[set_log_level] fatal")]
        [TestCase(LogLevel.Error, "[set_log_level] error")]
        [TestCase(LogLevel.Warn, "[set_log_level] warning")]
        [TestCase(LogLevel.Info, "[set_log_level] info")]
        [TestCase(LogLevel.Debug, "[set_log_level] debug")]
        [TestCase(LogLevel.Verbose, "[set_log_level] verbose")]
        [TestCase(100, "[set_log_level] log_level=100")]
        public void SetLogLevelReqAll(LogLevel logLevel, string result)
        {
            SetLogLevelRequest arg = new SetLogLevelRequest("", "", logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
        }

        [TestCase(LogLevel.Default, "[set_log_level] default APP1 (CTX1)")]
        [TestCase(LogLevel.Block, "[set_log_level] block_all APP1 (CTX1)")]
        [TestCase(LogLevel.Fatal, "[set_log_level] fatal APP1 (CTX1)")]
        [TestCase(LogLevel.Error, "[set_log_level] error APP1 (CTX1)")]
        [TestCase(LogLevel.Warn, "[set_log_level] warning APP1 (CTX1)")]
        [TestCase(LogLevel.Info, "[set_log_level] info APP1 (CTX1)")]
        [TestCase(LogLevel.Debug, "[set_log_level] debug APP1 (CTX1)")]
        [TestCase(LogLevel.Verbose, "[set_log_level] verbose APP1 (CTX1)")]
        [TestCase(100, "[set_log_level] log_level=100 APP1 (CTX1)")]
        public void SetLogLevelReq(LogLevel logLevel, string result)
        {
            SetLogLevelRequest arg = new SetLogLevelRequest("APP1", "CTX1", logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
        }

        [TestCase(LogLevel.Default, "[set_log_level] default APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Block, "[set_log_level] block_all APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Fatal, "[set_log_level] fatal APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Error, "[set_log_level] error APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Warn, "[set_log_level] warning APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Info, "[set_log_level] info APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Debug, "[set_log_level] debug APP1 (CTX1) SERB")]
        [TestCase(LogLevel.Verbose, "[set_log_level] verbose APP1 (CTX1) SERB")]
        [TestCase(100, "[set_log_level] log_level=100 APP1 (CTX1) SERB")]
        public void SetLogLevelReqComIntf(LogLevel logLevel, string result)
        {
            SetLogLevelRequest arg = new SetLogLevelRequest("APP1", "CTX1", logLevel, "SERB");
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo("SERB"));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
        }

        [TestCase(SetLogLevelResponse.StatusOk, "[set_log_level ok]")]
        [TestCase(SetLogLevelResponse.StatusNotSupported, "[set_log_level not_supported]")]
        [TestCase(SetLogLevelResponse.StatusError, "[set_log_level error]")]
        [TestCase(100, "[set_log_level status=100]")]
        public void SetLogLevelResp(int status, string result)
        {
            SetLogLevelResponse arg = new SetLogLevelResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x01));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
