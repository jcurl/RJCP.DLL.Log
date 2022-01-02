﻿namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetLogLevelDecoderTest : ControlDecoderTestBase<SetLogLevelRequestDecoder, SetLogLevelResponseDecoder>
    {
        public SetLogLevelDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x01, typeof(SetLogLevelRequest), typeof(SetLogLevelResponse))
        { }

        [TestCase(0x00, "[set_log_level] block_all APP1 (CTX1)")]
        [TestCase(0x01, "[set_log_level] fatal APP1 (CTX1)")]
        [TestCase(0xFF, "[set_log_level] default APP1 (CTX1)")]
        public void DecodeRequestNoComId(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x31,
                0x43, 0x54, 0x58, 0x31, logLevel, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x01_SetLogLevelRequest_NoComId_{logLevel:x2}", out IControlArg service);

            SetLogLevelRequest request = (SetLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_log_level] block_all APP1 (CTX1) eth0")]
        [TestCase(0x01, "[set_log_level] fatal APP1 (CTX1) eth0")]
        [TestCase(0xFF, "[set_log_level] default APP1 (CTX1) eth0")]
        public void DecodeRequest(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x31,
                0x43, 0x54, 0x58, 0x31, logLevel, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x01_SetLogLevelRequest_{logLevel:x2}", out IControlArg service);

            SetLogLevelRequest request = (SetLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_log_level] block_all eth0")]
        [TestCase(0x01, "[set_log_level] fatal eth0")]
        [TestCase(0xFF, "[set_log_level] default eth0")]
        public void DecodeRequestComIdOnly(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x01_SetLogLevelRequest_ComIdOnly_{logLevel:x2}", out IControlArg service);

            SetLogLevelRequest request = (SetLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_log_level] block_all APP (CTX) eth")]
        [TestCase(0x01, "[set_log_level] fatal APP (CTX) eth")]
        [TestCase(0xFF, "[set_log_level] default APP (CTX) eth")]
        public void DecodeRequest3Char(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x00,
                0x43, 0x54, 0x58, 0x00, logLevel, 0x65, 0x74, 0x68, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x01_SetLogLevelRequest_3Char_{logLevel:x2}", out IControlArg service);

            SetLogLevelRequest request = (SetLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_log_level] block_all")]
        [TestCase(0x01, "[set_log_level] fatal")]
        [TestCase(0xFF, "[set_log_level] default")]
        public void DecodeRequestNoAppId(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, logLevel, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x01_SetLogLevelRequest_NoAppId_{logLevel:x2}", out IControlArg service);

            SetLogLevelRequest request = (SetLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_log_level ok]")]
        [TestCase(0x01, "[set_log_level not_supported]")]
        [TestCase(0x02, "[set_log_level error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x01, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x01_SetLogLevel_Response_{status:x2}", out IControlArg service);

            SetLogLevelResponse response = (SetLogLevelResponse)service;
            Assert.That(response.Status, Is.EqualTo((int)status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}