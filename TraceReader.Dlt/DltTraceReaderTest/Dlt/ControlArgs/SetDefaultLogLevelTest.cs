namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetDefaultLogLevelTest
    {
        [TestCase(SetDefaultLogLevelRequest.LogLevelBlock, "[set_default_log_level] block_all")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelFatal, "[set_default_log_level] fatal")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelError, "[set_default_log_level] error")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelWarn, "[set_default_log_level] warning")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelInfo, "[set_default_log_level] info")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelDebug, "[set_default_log_level] debug")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelVerbose, "[set_default_log_level] verbose")]
        [TestCase(100, "[set_default_log_level] log_level=100")]
        public void SetDefaultLogLevelReq(int logLevel, string result)
        {
            SetDefaultLogLevelRequest arg = new SetDefaultLogLevelRequest(logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(SetDefaultLogLevelRequest.LogLevelBlock, "eth0", "[set_default_log_level] block_all eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelFatal, "eth0", "[set_default_log_level] fatal eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelError, "eth0", "[set_default_log_level] error eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelWarn, "eth0", "[set_default_log_level] warning eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelInfo, "eth0", "[set_default_log_level] info eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelDebug, "eth0", "[set_default_log_level] debug eth0")]
        [TestCase(SetDefaultLogLevelRequest.LogLevelVerbose, "eth0", "[set_default_log_level] verbose eth0")]
        [TestCase(100, "eth0", "[set_default_log_level] log_level=100 eth0")]
        public void SetDefaultLogLevelReq(int logLevel, string comInterface, string result)
        {
            SetDefaultLogLevelRequest arg = new SetDefaultLogLevelRequest(logLevel, comInterface);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
            Assert.That(arg.ComInterface, Is.EqualTo(comInterface));
        }

        [TestCase(SetDefaultLogLevelResponse.StatusOk, "[set_default_log_level ok]")]
        [TestCase(SetDefaultLogLevelResponse.StatusNotSupported, "[set_default_log_level not_supported]")]
        [TestCase(SetDefaultLogLevelResponse.StatusError, "[set_default_log_level error]")]
        public void SetDefaultLogLevelRes(int status, string result)
        {
            SetDefaultLogLevelResponse arg = new SetDefaultLogLevelResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
