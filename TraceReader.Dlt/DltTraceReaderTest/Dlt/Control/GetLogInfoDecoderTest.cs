namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetLogInfoDecoderTest : ControlDecoderTestBase<GetLogInfoRequestDecoder, GetLogInfoResponseDecoder>
    {
        public GetLogInfoDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x03, typeof(GetLogInfoRequest), typeof(GetLogInfoResponse))
        { }

        [TestCase(0x00, "[get_log_info options=0] APP1 (CTX1) eth0")]
        [TestCase(0x01, "[get_log_info options=1] APP1 (CTX1) eth0")]
        [TestCase(0x02, "[get_log_info options=2] APP1 (CTX1) eth0")]
        [TestCase(0x03, "[get_log_info options=3] APP1 (CTX1) eth0")]
        [TestCase(0x04, "[get_log_info options=4] APP1 (CTX1) eth0")]
        [TestCase(0x05, "[get_log_info options=5] APP1 (CTX1) eth0")]
        [TestCase(0x06, "[get_log_info options=6] APP1 (CTX1) eth0")]
        [TestCase(0x07, "[get_log_info options=7] APP1 (CTX1) eth0")]
        [TestCase(0x08, "[get_log_info options=8] APP1 (CTX1) eth0")]
        [TestCase(0xFF, "[get_log_info options=255] APP1 (CTX1) eth0")]
        public void DecodeRequest(byte options, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, options,
                0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x03_GetLogInfoRequest_{options:x2}", out IControlArg service);

            GetLogInfoRequest request = (GetLogInfoRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Options, Is.EqualTo(options));
            Assert.That(request.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(request.ContextId, Is.EqualTo("CTX1"));
            Assert.That(request.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, "[get_log_info options=0] APP1 (CTX1)")]
        [TestCase(0x01, "[get_log_info options=1] APP1 (CTX1)")]
        [TestCase(0x02, "[get_log_info options=2] APP1 (CTX1)")]
        [TestCase(0x03, "[get_log_info options=3] APP1 (CTX1)")]
        [TestCase(0x04, "[get_log_info options=4] APP1 (CTX1)")]
        [TestCase(0x05, "[get_log_info options=5] APP1 (CTX1)")]
        [TestCase(0x06, "[get_log_info options=6] APP1 (CTX1)")]
        [TestCase(0x07, "[get_log_info options=7] APP1 (CTX1)")]
        [TestCase(0x08, "[get_log_info options=8] APP1 (CTX1)")]
        [TestCase(0xFF, "[get_log_info options=255] APP1 (CTX1)")]
        public void DecodeRequestNoComId(byte options, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, options,
                0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x03_GetLogInfoRequest_NoComId_{options:x2}", out IControlArg service);

            GetLogInfoRequest request = (GetLogInfoRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Options, Is.EqualTo(options));
            Assert.That(request.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(request.ContextId, Is.EqualTo("CTX1"));
            Assert.That(request.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0x00, "[get_log_info options=0] -all-")]
        [TestCase(0x01, "[get_log_info options=1] -all-")]
        [TestCase(0x02, "[get_log_info options=2] -all-")]
        [TestCase(0x03, "[get_log_info options=3] -all-")]
        [TestCase(0x04, "[get_log_info options=4] -all-")]
        [TestCase(0x05, "[get_log_info options=5] -all-")]
        [TestCase(0x06, "[get_log_info options=6] -all-")]
        [TestCase(0x07, "[get_log_info options=7] -all-")]
        [TestCase(0x08, "[get_log_info options=8] -all-")]
        [TestCase(0xFF, "[get_log_info options=255] -all-")]
        public void DecodeRequestAllNoComId(byte options, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, options,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x03_GetLogInfoRequest_AllNoComId_{options:x2}", out IControlArg service);

            GetLogInfoRequest request = (GetLogInfoRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Options, Is.EqualTo(options));
            Assert.That(request.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(request.ContextId, Is.EqualTo(string.Empty));
            Assert.That(request.ComInterface, Is.EqualTo(string.Empty));
        }

        [TestCase(0x00, "[get_log_info options=0] -all- eth0")]
        [TestCase(0x01, "[get_log_info options=1] -all- eth0")]
        [TestCase(0x02, "[get_log_info options=2] -all- eth0")]
        [TestCase(0x03, "[get_log_info options=3] -all- eth0")]
        [TestCase(0x04, "[get_log_info options=4] -all- eth0")]
        [TestCase(0x05, "[get_log_info options=5] -all- eth0")]
        [TestCase(0x06, "[get_log_info options=6] -all- eth0")]
        [TestCase(0x07, "[get_log_info options=7] -all- eth0")]
        [TestCase(0x08, "[get_log_info options=8] -all- eth0")]
        [TestCase(0xFF, "[get_log_info options=255] -all- eth0")]
        public void DecodeRequestAll(byte options, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, options,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x03_GetLogInfoRequest_All_{options:x2}", out IControlArg service);

            GetLogInfoRequest request = (GetLogInfoRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Options, Is.EqualTo(options));
            Assert.That(request.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(request.ContextId, Is.EqualTo(string.Empty));
            Assert.That(request.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, "[get_log_info options=0] APP1 () eth0")]
        [TestCase(0x01, "[get_log_info options=1] APP1 () eth0")]
        [TestCase(0x02, "[get_log_info options=2] APP1 () eth0")]
        [TestCase(0x03, "[get_log_info options=3] APP1 () eth0")]
        [TestCase(0x04, "[get_log_info options=4] APP1 () eth0")]
        [TestCase(0x05, "[get_log_info options=5] APP1 () eth0")]
        [TestCase(0x06, "[get_log_info options=6] APP1 () eth0")]
        [TestCase(0x07, "[get_log_info options=7] APP1 () eth0")]
        [TestCase(0x08, "[get_log_info options=8] APP1 () eth0")]
        [TestCase(0xFF, "[get_log_info options=255] APP1 () eth0")]
        public void DecodeRequestAppId(byte options, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, options,
                0x41, 0x50, 0x50, 0x31, 0x00, 0x00, 0x00, 0x00, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x03_GetLogInfoRequest_AppId_{options:x2}", out IControlArg service);

            GetLogInfoRequest request = (GetLogInfoRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Options, Is.EqualTo(options));
            Assert.That(request.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(request.ContextId, Is.EqualTo(string.Empty));
            Assert.That(request.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, "[get_log_info ok]")]
        [TestCase(0x01, "[get_log_info not_supported]")]
        [TestCase(0x02, "[get_log_info error]")]
        [TestCase(0x08, "[get_log_info no_matching_context_id]")]
        [TestCase(0x09, "[get_log_info overflow]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x03, 0x00, 0x00, 0x00, status, 0x65, 0x74, 0x68, 0x30 };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x03_GetLogInfoResponse_{status:x2}", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.AppIds.Count, Is.EqualTo(0));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(status));
        }

        [Test]
        public void DecodeResponseNoLevelNoTrace_3()
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x03,
                0x01, 0x00, 0x41, 0x50, 0x50, 0x31, 0x01, 0x00, 0x43, 0x54, 0x58, 0x31,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x03_GetLogInfoResponse_03", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo("[get_log_info 3] APP1 (CTX1); eth0"));
            Assert.That(response.AppIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Undefined));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusUndefined));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusNoLogNoTrace));
        }

        [TestCase(0x00, "[get_log_info 4] APP1 (CTX1 block_all); eth0")]
        [TestCase(0x01, "[get_log_info 4] APP1 (CTX1 fatal); eth0")]
        [TestCase(0x02, "[get_log_info 4] APP1 (CTX1 error); eth0")]
        [TestCase(0x03, "[get_log_info 4] APP1 (CTX1 warning); eth0")]
        [TestCase(0x04, "[get_log_info 4] APP1 (CTX1 info); eth0")]
        [TestCase(0x05, "[get_log_info 4] APP1 (CTX1 debug); eth0")]
        [TestCase(0x06, "[get_log_info 4] APP1 (CTX1 verbose); eth0")]
        [TestCase(0xFF, "[get_log_info 4] APP1 (CTX1 default); eth0")]
        public void DecodeResponseLevelNoTrace_4(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x04,
                0x01, 0x00, 0x41, 0x50, 0x50, 0x31, 0x01, 0x00, 0x43, 0x54, 0x58, 0x31, logLevel,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x03_GetLogInfoResponse_04_{logLevel:x2}", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.AppIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo((LogLevel)unchecked((sbyte)logLevel)));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusUndefined));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusWithLogNoTrace));
        }

        [TestCase(ContextId.StatusOff, "[get_log_info 5] APP1 (CTX1 off); eth0")]
        [TestCase(ContextId.StatusOn, "[get_log_info 5] APP1 (CTX1 on); eth0")]
        [TestCase(0xFF, "[get_log_info 5] APP1 (CTX1 default); eth0")]
        public void DecodeResponseNoLevelTrace_5(byte traceStatus, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x05,
                0x01, 0x00, 0x41, 0x50, 0x50, 0x31, 0x01, 0x00, 0x43, 0x54, 0x58, 0x31, traceStatus,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x03_GetLogInfoResponse_05_{traceStatus:x2}", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.AppIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Undefined));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(unchecked((sbyte)traceStatus)));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusNoLogWithTrace));
        }

        [TestCase(0x00, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 block_all off); eth0")]
        [TestCase(0x01, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 fatal off); eth0")]
        [TestCase(0x02, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 error off); eth0")]
        [TestCase(0x03, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 warning off); eth0")]
        [TestCase(0x04, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 info off); eth0")]
        [TestCase(0x05, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 debug off); eth0")]
        [TestCase(0x06, ContextId.StatusOff, "[get_log_info 6] APP1 (CTX1 verbose off); eth0")]
        [TestCase(0x00, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 block_all on); eth0")]
        [TestCase(0x01, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 fatal on); eth0")]
        [TestCase(0x02, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 error on); eth0")]
        [TestCase(0x03, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 warning on); eth0")]
        [TestCase(0x04, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 info on); eth0")]
        [TestCase(0x05, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 debug on); eth0")]
        [TestCase(0x06, ContextId.StatusOn, "[get_log_info 6] APP1 (CTX1 verbose on); eth0")]
        [TestCase(0xFF, 0xFE, "[get_log_info 6] APP1 (CTX1 default -2); eth0")]
        public void DecodeResponseLevelTrace_6(byte logLevel, byte traceStatus, string result)
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x06,
                0x01, 0x00, 0x41, 0x50, 0x50, 0x31, 0x01, 0x00, 0x43, 0x54, 0x58, 0x31, logLevel, traceStatus,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x03_GetLogInfoResponse_06_{logLevel:x2}_{traceStatus:x2}", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.AppIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo(string.Empty));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo((LogLevel)unchecked((sbyte)logLevel)));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(unchecked((sbyte)traceStatus)));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusWithLogWithTrace));
        }

        [Test]
        public void DecodeResponseFull_7()
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x07,
                0x01, 0x00, 0x41, 0x50, 0x50, 0x31, 0x01, 0x00, 0x43, 0x54, 0x58, 0x31, 0xFF, 0xFF,
                0x15, 0x00, 0x73, 0x74, 0x64, 0x69, 0x6E, 0x20, 0x61, 0x64, 0x61, 0x70, 0x74, 0x6F, 0x72, 0x20, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74,
                0x19, 0x00, 0x73, 0x74, 0x64, 0x69, 0x6E, 0x20, 0x61, 0x64, 0x61, 0x70, 0x74, 0x6F, 0x72, 0x20, 0x61, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, "0x03_GetLogInfoResponse_07_Full", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo("[get_log_info 7] APP1 (CTX1 default default); eth0"));
            Assert.That(response.AppIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo("stdin adaptor application"));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(1));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo("stdin adaptor context"));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
        }

        [Test]
        public void DecodeResponseFullNoAppIds()
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x07,
                0x00, 0x00,
                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, "0x03_GetLogInfoResponse_07_NuAppIds", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo("[get_log_info 7]"));
            Assert.That(response.AppIds.Count, Is.EqualTo(0));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
        }

        [Test]
        public void DecodeResponseFullMultiple()
        {
            byte[] payload = new byte[] {
                0x03, 0x00, 0x00, 0x00, 0x07,
                0x03, 0x00,  // 3 applications

                0x41, 0x50, 0x50, 0x31,
                0x03, 0x00,  // App1; 3 context id's
                0x43, 0x54, 0x58, 0x31, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x31,
                0x43, 0x54, 0x58, 0x32, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x32,
                0x43, 0x54, 0x58, 0x33, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x33,
                0x06, 0x00, 0x41, 0x44, 0x65, 0x73, 0x63, 0x31,

                0x41, 0x50, 0x50, 0x32,
                0x02, 0x00,  // App2; 2 context id's
                0x43, 0x54, 0x58, 0x34, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x34,
                0x43, 0x54, 0x58, 0x35, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x35,
                0x06, 0x00, 0x41, 0x44, 0x65, 0x73, 0x63, 0x32,

                0x41, 0x50, 0x50, 0x33,
                0x04, 0x00,  // App3; 4 context id's
                0x43, 0x54, 0x58, 0x36, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x36,
                0x43, 0x54, 0x58, 0x37, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x37,
                0x43, 0x54, 0x58, 0x38, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x38,
                0x43, 0x54, 0x58, 0x39, 0xFF, 0xFF,
                0x05, 0x00, 0x54, 0x65, 0x73, 0x74, 0x39,
                0x06, 0x00, 0x41, 0x44, 0x65, 0x73, 0x63, 0x33,

                0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, "0x03_GetLogInfoResponse_07_FullMultiple", out IControlArg service);

            GetLogInfoResponse response = (GetLogInfoResponse)service;
            Assert.That(response.ToString(), Is.EqualTo("[get_log_info 7] APP1 (CTX1 default default, CTX2 default default, CTX3 default default); APP2 (CTX4 default default, CTX5 default default); APP3 (CTX6 default default, CTX7 default default, CTX8 default default, CTX9 default default); eth0"));
            Assert.That(response.AppIds.Count, Is.EqualTo(3));
            Assert.That(response.AppIds[0].Name, Is.EqualTo("APP1"));
            Assert.That(response.AppIds[0].Description, Is.EqualTo("ADesc1"));
            Assert.That(response.AppIds[0].ContextIds.Count, Is.EqualTo(3));
            Assert.That(response.AppIds[0].ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(response.AppIds[0].ContextIds[0].Description, Is.EqualTo("Test1"));
            Assert.That(response.AppIds[0].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[0].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[0].ContextIds[1].Name, Is.EqualTo("CTX2"));
            Assert.That(response.AppIds[0].ContextIds[1].Description, Is.EqualTo("Test2"));
            Assert.That(response.AppIds[0].ContextIds[1].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[0].ContextIds[1].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[0].ContextIds[2].Name, Is.EqualTo("CTX3"));
            Assert.That(response.AppIds[0].ContextIds[2].Description, Is.EqualTo("Test3"));
            Assert.That(response.AppIds[0].ContextIds[2].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[0].ContextIds[2].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[1].Name, Is.EqualTo("APP2"));
            Assert.That(response.AppIds[1].Description, Is.EqualTo("ADesc2"));
            Assert.That(response.AppIds[1].ContextIds.Count, Is.EqualTo(2));
            Assert.That(response.AppIds[1].ContextIds[0].Name, Is.EqualTo("CTX4"));
            Assert.That(response.AppIds[1].ContextIds[0].Description, Is.EqualTo("Test4"));
            Assert.That(response.AppIds[1].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[1].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[1].ContextIds[1].Name, Is.EqualTo("CTX5"));
            Assert.That(response.AppIds[1].ContextIds[1].Description, Is.EqualTo("Test5"));
            Assert.That(response.AppIds[1].ContextIds[1].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[1].ContextIds[1].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[2].Name, Is.EqualTo("APP3"));
            Assert.That(response.AppIds[2].Description, Is.EqualTo("ADesc3"));
            Assert.That(response.AppIds[2].ContextIds.Count, Is.EqualTo(4));
            Assert.That(response.AppIds[2].ContextIds[0].Name, Is.EqualTo("CTX6"));
            Assert.That(response.AppIds[2].ContextIds[0].Description, Is.EqualTo("Test6"));
            Assert.That(response.AppIds[2].ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[2].ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[2].ContextIds[1].Name, Is.EqualTo("CTX7"));
            Assert.That(response.AppIds[2].ContextIds[1].Description, Is.EqualTo("Test7"));
            Assert.That(response.AppIds[2].ContextIds[1].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[2].ContextIds[1].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[2].ContextIds[2].Name, Is.EqualTo("CTX8"));
            Assert.That(response.AppIds[2].ContextIds[2].Description, Is.EqualTo("Test8"));
            Assert.That(response.AppIds[2].ContextIds[2].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[2].ContextIds[2].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.AppIds[2].ContextIds[3].Name, Is.EqualTo("CTX9"));
            Assert.That(response.AppIds[2].ContextIds[3].Description, Is.EqualTo("Test9"));
            Assert.That(response.AppIds[2].ContextIds[3].LogLevel, Is.EqualTo(LogLevel.Default));
            Assert.That(response.AppIds[2].ContextIds[3].TraceStatus, Is.EqualTo(ContextId.StatusDefaultTrace));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
            Assert.That(response.Status, Is.EqualTo(GetLogInfoResponse.StatusFullInfo));
        }
    }
}
