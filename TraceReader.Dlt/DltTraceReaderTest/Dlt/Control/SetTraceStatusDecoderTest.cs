namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class SetTraceStatusDecoderTest : ControlDecoderTestBase<SetTraceStatusRequestDecoder, SetTraceStatusResponseDecoder>
    {
        public SetTraceStatusDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x02, typeof(SetTraceStatusRequest), typeof(SetTraceStatusResponse))
        { }

        [TestCase(0x00, "[set_trace_status] off APP1 (CTX1)")]
        [TestCase(0x01, "[set_trace_status] on APP1 (CTX1)")]
        [TestCase(0xFF, "[set_trace_status] default APP1 (CTX1)")]
        public void DecodeRequestNoComId(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {
                    0x02, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, logLevel, 0x00, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x02, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, logLevel, 0x00, 0x00, 0x00, 0x00
                };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x02_SetTraceStatusRequest_NoComId_{logLevel:x2}", out IControlArg service);

            SetTraceStatusRequest request = (SetTraceStatusRequest)service;
            Assert.That(request.TraceStatus, Is.EqualTo(unchecked((sbyte)logLevel)));
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_trace_status] off APP1 (CTX1) eth0")]
        [TestCase(0x01, "[set_trace_status] on APP1 (CTX1) eth0")]
        [TestCase(0xFF, "[set_trace_status] default APP1 (CTX1) eth0")]
        public void DecodeRequest(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {
                    0x02, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, logLevel, 0x65, 0x74, 0x68, 0x30
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x02, 0x41, 0x50, 0x50, 0x31,
                    0x43, 0x54, 0x58, 0x31, logLevel, 0x65, 0x74, 0x68, 0x30
                };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x02_SetTraceStatusRequest_{logLevel:x2}", out IControlArg service);

            SetTraceStatusRequest request = (SetTraceStatusRequest)service;
            Assert.That(request.TraceStatus, Is.EqualTo(unchecked((sbyte)logLevel)));
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_trace_status] off eth0")]
        [TestCase(0x01, "[set_trace_status] on eth0")]
        [TestCase(0xFF, "[set_trace_status] default eth0")]
        public void DecodeRequestComIdOnly(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {
                    0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x30
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x30
                };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x02_SetTraceStatusRequest_ComIdOnly_{logLevel:x2}", out IControlArg service);

            SetTraceStatusRequest request = (SetTraceStatusRequest)service;
            Assert.That(request.TraceStatus, Is.EqualTo(unchecked((sbyte)logLevel)));
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_trace_status] off APP (CTX) eth")]
        [TestCase(0x01, "[set_trace_status] on APP (CTX) eth")]
        [TestCase(0xFF, "[set_trace_status] default APP (CTX) eth")]
        public void DecodeRequest3Char(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {
                    0x02, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x00,
                    0x43, 0x54, 0x58, 0x00, logLevel, 0x65, 0x74, 0x68, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x02, 0x41, 0x50, 0x50, 0x00,
                    0x43, 0x54, 0x58, 0x00, logLevel, 0x65, 0x74, 0x68, 0x00
                };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x02_SetTraceStatusRequest_3Char_{logLevel:x2}", out IControlArg service);

            SetTraceStatusRequest request = (SetTraceStatusRequest)service;
            Assert.That(request.TraceStatus, Is.EqualTo(unchecked((sbyte)logLevel)));
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_trace_status] off")]
        [TestCase(0x01, "[set_trace_status] on")]
        [TestCase(0xFF, "[set_trace_status] default")]
        public void DecodeRequestNoAppId(byte logLevel, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] {
                    0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, logLevel, 0x00, 0x00, 0x00, 0x00
                } :
                new byte[] {
                    0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, logLevel, 0x00, 0x00, 0x00, 0x00
                };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x02_SetTraceStatusRequest_NoAppId_{logLevel:x2}", out IControlArg service);

            SetTraceStatusRequest request = (SetTraceStatusRequest)service;
            Assert.That(request.TraceStatus, Is.EqualTo(unchecked((sbyte)logLevel)));
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_trace_status ok]")]
        [TestCase(0x01, "[set_trace_status not_supported]")]
        [TestCase(0x02, "[set_trace_status error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x02, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x02, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x02_SetTraceStatusResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x02));
            Assert.That(response.Status, Is.EqualTo((int)status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
