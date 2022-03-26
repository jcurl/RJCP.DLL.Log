namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetDefaultLogLevelTest
    {
        [TestCase(LogLevel.Block, "[set_default_log_level] block_all")]
        [TestCase(LogLevel.Fatal, "[set_default_log_level] fatal")]
        [TestCase(LogLevel.Error, "[set_default_log_level] error")]
        [TestCase(LogLevel.Warn, "[set_default_log_level] warning")]
        [TestCase(LogLevel.Info, "[set_default_log_level] info")]
        [TestCase(LogLevel.Debug, "[set_default_log_level] debug")]
        [TestCase(LogLevel.Verbose, "[set_default_log_level] verbose")]
        [TestCase(100, "[set_default_log_level] log_level=100")]
        public void SetDefaultLogLevelReq(LogLevel logLevel, string result)
        {
            SetDefaultLogLevelRequest arg = new SetDefaultLogLevelRequest(logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(LogLevel.Block, "eth0", "[set_default_log_level] block_all eth0")]
        [TestCase(LogLevel.Fatal, "eth0", "[set_default_log_level] fatal eth0")]
        [TestCase(LogLevel.Error, "eth0", "[set_default_log_level] error eth0")]
        [TestCase(LogLevel.Warn, "eth0", "[set_default_log_level] warning eth0")]
        [TestCase(LogLevel.Info, "eth0", "[set_default_log_level] info eth0")]
        [TestCase(LogLevel.Debug, "eth0", "[set_default_log_level] debug eth0")]
        [TestCase(LogLevel.Verbose, "eth0", "[set_default_log_level] verbose eth0")]
        [TestCase(100, "eth0", "[set_default_log_level] log_level=100 eth0")]
        public void SetDefaultLogLevelReq(LogLevel logLevel, string comInterface, string result)
        {
            SetDefaultLogLevelRequest arg = new SetDefaultLogLevelRequest(logLevel, comInterface);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.LogLevel, Is.EqualTo(logLevel));
            Assert.That(arg.ComInterface, Is.EqualTo(comInterface));
        }

        [TestCase(ControlResponse.StatusOk, "[set_default_log_level ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[set_default_log_level not_supported]")]
        [TestCase(ControlResponse.StatusError, "[set_default_log_level error]")]
        public void SetDefaultLogLevelRes(int status, string result)
        {
            SetDefaultLogLevelResponse arg = new SetDefaultLogLevelResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x11));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
