namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetLogInfoTest
    {
        [TestCase(0, "[get_log_info options=0] -all-")]
        [TestCase(1, "[get_log_info options=1] -all-")]
        [TestCase(2, "[get_log_info options=2] -all-")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] -all-")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] -all-")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] -all-")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] -all-")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] -all-")]
        [TestCase(8, "[get_log_info options=8] -all-")]
        [TestCase(255, "[get_log_info options=255] -all-")]
        public void GetLogInfoReqAllAppIdNoComId(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, null, null);
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0, "[get_log_info options=0] -all-")]
        [TestCase(1, "[get_log_info options=1] -all-")]
        [TestCase(2, "[get_log_info options=2] -all-")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] -all-")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] -all-")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] -all-")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] -all-")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] -all-")]
        [TestCase(8, "[get_log_info options=8] -all-")]
        [TestCase(255, "[get_log_info options=255] -all-")]
        public void GetLogInfoReqAllAppIdIgnCtxIdNoComId(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, null, "CTX1");
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0, "[get_log_info options=0] APP1 (CTX1)")]
        [TestCase(1, "[get_log_info options=1] APP1 (CTX1)")]
        [TestCase(2, "[get_log_info options=2] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] APP1 (CTX1)")]
        [TestCase(8, "[get_log_info options=8] APP1 (CTX1)")]
        [TestCase(255, "[get_log_info options=255] APP1 (CTX1)")]
        public void GetLogInfoReqNoComId(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, "APP1", "CTX1");
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0, "[get_log_info options=0] APP1 (CTX1)")]
        [TestCase(1, "[get_log_info options=1] APP1 (CTX1)")]
        [TestCase(2, "[get_log_info options=2] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] APP1 (CTX1)")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] APP1 (CTX1)")]
        [TestCase(8, "[get_log_info options=8] APP1 (CTX1)")]
        [TestCase(255, "[get_log_info options=255] APP1 (CTX1)")]
        public void GetLogInfoReqComIdNull(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, "APP1", "CTX1", null);
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0, "[get_log_info options=0] -all- eth0")]
        [TestCase(1, "[get_log_info options=1] -all- eth0")]
        [TestCase(2, "[get_log_info options=2] -all- eth0")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] -all- eth0")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] -all- eth0")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] -all- eth0")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] -all- eth0")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] -all- eth0")]
        [TestCase(8, "[get_log_info options=8] -all- eth0")]
        [TestCase(255, "[get_log_info options=255] -all- eth0")]
        public void GetLogInfoReqAllAppId(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, null, null, "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0, "[get_log_info options=0] APP1 (CTX1) eth0")]
        [TestCase(1, "[get_log_info options=1] APP1 (CTX1) eth0")]
        [TestCase(2, "[get_log_info options=2] APP1 (CTX1) eth0")]
        [TestCase(GetLogInfoRequest.OptionsNoLogNoTrace, "[get_log_info options=3] APP1 (CTX1) eth0")]
        [TestCase(GetLogInfoRequest.OptionsWithLogNoTrace, "[get_log_info options=4] APP1 (CTX1) eth0")]
        [TestCase(GetLogInfoRequest.OptionsNoLogWithTrace, "[get_log_info options=5] APP1 (CTX1) eth0")]
        [TestCase(GetLogInfoRequest.OptionsWithLogWithTrace, "[get_log_info options=6] APP1 (CTX1) eth0")]
        [TestCase(GetLogInfoRequest.OptionsFullInfo, "[get_log_info options=7] APP1 (CTX1) eth0")]
        [TestCase(8, "[get_log_info options=8] APP1 (CTX1) eth0")]
        [TestCase(255, "[get_log_info options=255] APP1 (CTX1) eth0")]
        public void GetLogInfoReq(int options, string result)
        {
            GetLogInfoRequest arg = new GetLogInfoRequest(options, "APP1", "CTX1", "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(ControlResponse.StatusOk, "[get_log_info ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[get_log_info not_supported]")]
        [TestCase(ControlResponse.StatusError, "[get_log_info error]")]
        [TestCase(GetLogInfoResponse.StatusNoLogNoTrace, "[get_log_info 3]")]
        [TestCase(GetLogInfoResponse.StatusWithLogNoTrace, "[get_log_info 4]")]
        [TestCase(GetLogInfoResponse.StatusNoLogWithTrace, "[get_log_info 5]")]
        [TestCase(GetLogInfoResponse.StatusWithLogWithTrace, "[get_log_info 6]")]
        [TestCase(GetLogInfoResponse.StatusFullInfo, "[get_log_info 7]")]
        [TestCase(GetLogInfoResponse.StatusNoMatch, "[get_log_info no_matching_context_id]")]
        public void GetLogInfoResNoApps(int status, string result)
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(status, "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
            Assert.That(arg.Status, Is.EqualTo(status));
            Assert.That(arg.AppIds, Is.Empty);
        }

        [TestCase(ControlResponse.StatusOk, "[get_log_info ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[get_log_info not_supported]")]
        [TestCase(ControlResponse.StatusError, "[get_log_info error]")]
        [TestCase(GetLogInfoResponse.StatusNoLogNoTrace, "[get_log_info 3]")]
        [TestCase(GetLogInfoResponse.StatusWithLogNoTrace, "[get_log_info 4]")]
        [TestCase(GetLogInfoResponse.StatusNoLogWithTrace, "[get_log_info 5]")]
        [TestCase(GetLogInfoResponse.StatusWithLogWithTrace, "[get_log_info 6]")]
        [TestCase(GetLogInfoResponse.StatusFullInfo, "[get_log_info 7]")]
        [TestCase(GetLogInfoResponse.StatusNoMatch, "[get_log_info no_matching_context_id]")]
        public void GetLogInfoResNoAppsNoComId(int status, string result)
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(status, null);
            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(status));
            Assert.That(arg.AppIds, Is.Empty);
        }

        [Test]
        public void GetLogInfoResOneApp()
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo, null);
            arg.AppIds.Add(new AppId("APP1", "Application"));

            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo("[get_log_info 7] APP1 ();"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
            Assert.That(arg.AppIds, Has.Count.EqualTo(1));
            Assert.That(arg.AppIds[0].ContextIds, Is.Empty);
        }

        [TestCase(ContextId.StatusOff, "[get_log_info 7] APP1 (CTX1 info off);")]
        [TestCase(ContextId.StatusOn, "[get_log_info 7] APP1 (CTX1 info on);")]
        [TestCase(ContextId.StatusDefaultTrace, "[get_log_info 7] APP1 (CTX1 info default);")]
        [TestCase(ContextId.StatusUndefined, "[get_log_info 7] APP1 (CTX1 info);")]
        public void GetLogInfoResOneAppOneCtx(int traceStatus, string result)
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo, null);
            AppId app1 = new AppId("APP1", "Application");
            app1.ContextIds.Add(new ContextId("CTX1", LogLevel.Info, traceStatus, "Context"));
            arg.AppIds.Add(app1);

            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
            Assert.That(arg.AppIds, Has.Count.EqualTo(1));
            Assert.That(arg.AppIds[0].ContextIds, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetLogInfoResOneAppMultiCtx()
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo, null);
            AppId app1 = new AppId("APP1", "Application");
            app1.ContextIds.Add(new ContextId("CTX1", LogLevel.Info, ContextId.StatusOn, "Context"));
            app1.ContextIds.Add(new ContextId("CTX2", LogLevel.Fatal, ContextId.StatusOn, "Context2"));
            arg.AppIds.Add(app1);

            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo("[get_log_info 7] APP1 (CTX1 info on, CTX2 fatal on);"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
            Assert.That(arg.AppIds, Has.Count.EqualTo(1));
            Assert.That(arg.AppIds[0].ContextIds, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetLogInfoResMultiAppOneCtx()
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo, null);
            AppId app1 = new AppId("APP1", "Application");
            app1.ContextIds.Add(new ContextId("CTX1", LogLevel.Info, ContextId.StatusOn, "Context"));
            arg.AppIds.Add(app1);
            AppId app2 = new AppId("APP2", "Application 2");
            app2.ContextIds.Add(new ContextId("VSIP", LogLevel.Info, ContextId.StatusOn, "Context"));
            arg.AppIds.Add(app2);

            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo("[get_log_info 7] APP1 (CTX1 info on); APP2 (VSIP info on);"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
            Assert.That(arg.AppIds, Has.Count.EqualTo(2));
            Assert.That(arg.AppIds[0].ContextIds, Has.Count.EqualTo(1));
            Assert.That(arg.AppIds[1].ContextIds, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetLogInfoResMultiAppMultiCtx()
        {
            GetLogInfoResponse arg = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo, null);
            AppId app1 = new AppId("APP1", "Application");
            app1.ContextIds.Add(new ContextId("CTX1", LogLevel.Info, ContextId.StatusOn, "Context"));
            app1.ContextIds.Add(new ContextId("CTX2", LogLevel.Fatal, ContextId.StatusOn, "Context2"));
            arg.AppIds.Add(app1);
            AppId app2 = new AppId("APP2", "Application 2");
            app2.ContextIds.Add(new ContextId("CTX3", LogLevel.Info, ContextId.StatusOff, "Context3"));
            app2.ContextIds.Add(new ContextId("CTX4", LogLevel.Fatal, ContextId.StatusOn, "Context4"));
            arg.AppIds.Add(app2);

            Assert.That(arg.ServiceId, Is.EqualTo(0x03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo("[get_log_info 7] APP1 (CTX1 info on, CTX2 fatal on); APP2 (CTX3 info off, CTX4 fatal on);"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
            Assert.That(arg.AppIds, Has.Count.EqualTo(2));
            Assert.That(arg.AppIds[0].ContextIds, Has.Count.EqualTo(2));
            Assert.That(arg.AppIds[1].ContextIds, Has.Count.EqualTo(2));
        }
    }
}
