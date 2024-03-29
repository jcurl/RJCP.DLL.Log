﻿namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class SetDefaultTraceStatusDecoderTest : ControlDecoderTestBase<SetDefaultTraceStatusRequestDecoder, SetDefaultTraceStatusResponseDecoder>
    {
        public SetDefaultTraceStatusDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x12, typeof(SetDefaultTraceStatusRequest), typeof(SetDefaultTraceStatusResponse))
        { }

        [TestCase(0x00, "[set_default_trace_status] off")]
        [TestCase(0x01, "[set_default_trace_status] on")]
        [TestCase(0xFF, "[set_default_trace_status] on")]
        public void DecodeRequestNoComId(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, logLevel, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, logLevel, 0x00, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x12_SetDefaultTraceStatusRequest_NoComId_{logLevel:x2}", out IControlArg service);

            SetDefaultTraceStatusRequest request = (SetDefaultTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Enabled, Is.EqualTo(logLevel != 0));
        }

        [TestCase(0x00, "[set_default_trace_status] off eth0")]
        [TestCase(0x01, "[set_default_trace_status] on eth0")]
        [TestCase(0xFF, "[set_default_trace_status] on eth0")]
        public void DecodeRequest(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x30 } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, logLevel, 0x65, 0x74, 0x68, 0x30 };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x12_SetDefaultTraceStatusRequest_{logLevel:x2}", out IControlArg service);

            SetDefaultTraceStatusRequest request = (SetDefaultTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Enabled, Is.EqualTo(logLevel != 0));
        }

        [TestCase(0x00, "[set_default_trace_status] off eth")]
        [TestCase(0x01, "[set_default_trace_status] on eth")]
        [TestCase(0xFF, "[set_default_trace_status] on eth")]
        public void DecodeRequest3Char(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, logLevel, 0x65, 0x74, 0x68, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x12_SetDefaultTraceStatusRequest_3Char_{logLevel:x2}", out IControlArg service);

            SetDefaultTraceStatusRequest request = (SetDefaultTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
            Assert.That(request.Enabled, Is.EqualTo(logLevel != 0));
        }

        [TestCase(0x00, "[set_default_trace_status ok]")]
        [TestCase(0x01, "[set_default_trace_status not_supported]")]
        [TestCase(0x02, "[set_default_trace_status error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x12_SetDefaultTraceStatusResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x12));
            Assert.That(response.Status, Is.EqualTo((int)status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
