namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(GetLogInfoResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(GetLogInfoResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    public class GetLogInfoResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public GetLogInfoResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        [TestCase(GetLogInfoResponse.StatusOverflow)]
        public void EncodeError(int statusCode)
        {
            GetLogInfoResponse response = new GetLogInfoResponse(statusCode);

            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
        }

        [Test]
        public void EncodeUnknownStatus()
        {
            GetLogInfoResponse response = new GetLogInfoResponse(20);

            byte[] buffer = new byte[1024];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }

        private static GetLogInfoResponse GetResponse(int statusCode)
        {
            GetLogInfoResponse response = new GetLogInfoResponse(statusCode);

            ContextId app1c1 = new ContextId("CTX1", LogLevel.Info, 1, "App1 Context 1");
            ContextId app1c2 = new ContextId("CTX2", LogLevel.Debug, 1, "App1 Context 2");
            AppId app1 = new AppId("APP1", "Application 1");
            app1.ContextIds.Add(app1c1);
            app1.ContextIds.Add(app1c2);

            ContextId app2c1 = new ContextId("CTX3", LogLevel.Error, 0, "App2 Context 3");
            ContextId app2c2 = new ContextId("CTX4", LogLevel.Fatal, 1, "App2 Context 4");
            AppId app2 = new AppId("APP2", "Application 2");
            app2.ContextIds.Add(app2c1);
            app2.ContextIds.Add(app2c2);

            response.AppIds.Add(app1);
            response.AppIds.Add(app2);
            return response;
        }

        [Test]
        public void FullResponse()
        {
            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x07,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01, 0x00, 0x0F, 0x41, 0x70, 0x70, 0x31, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x31, 0x00,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01, 0x00, 0x0F, 0x41, 0x70, 0x70, 0x31, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x32, 0x00,
                    0x00, 0x0E, 0x41, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x31, 0x00,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00, 0x00, 0x0F, 0x41, 0x70, 0x70, 0x32, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x33, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01, 0x00, 0x0F, 0x41, 0x70, 0x70, 0x32, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x34, 0x00,
                    0x00, 0x0E, 0x41, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x32, 0x00,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x07,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01, 0x0F, 0x00, 0x41, 0x70, 0x70, 0x31, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x31, 0x00,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01, 0x0F, 0x00, 0x41, 0x70, 0x70, 0x31, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x32, 0x00,
                    0x0E, 0x00, 0x41, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x31, 0x00,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00, 0x0F, 0x00, 0x41, 0x70, 0x70, 0x32, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x33, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01, 0x0F, 0x00, 0x41, 0x70, 0x70, 0x32, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x78, 0x74, 0x20, 0x34, 0x00,
                    0x0E, 0x00, 0x41, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x32, 0x00,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusFullInfo);
            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusFullInfo));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void WithLogWithTrace()
        {
            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x06,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x06,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusWithLogWithTrace);
            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusWithLogWithTrace));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NoLogNoTrace()
        {
            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x03,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31,
                    0x43, 0x54, 0x58, 0x32,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33,
                    0x43, 0x54, 0x58, 0x34,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x03,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31,
                    0x43, 0x54, 0x58, 0x32,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33,
                    0x43, 0x54, 0x58, 0x34,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusNoLogNoTrace);
            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusNoLogNoTrace));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void WithLogNoTrace()
        {
            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x04,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04,
                    0x43, 0x54, 0x58, 0x32, 0x05,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02,
                    0x43, 0x54, 0x58, 0x34, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x04,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04,
                    0x43, 0x54, 0x58, 0x32, 0x05,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02,
                    0x43, 0x54, 0x58, 0x34, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusWithLogNoTrace);
            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusWithLogNoTrace));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NoLogWithTrace()
        {
            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x05,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x01,
                    0x43, 0x54, 0x58, 0x32, 0x01,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x05,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x01,
                    0x43, 0x54, 0x58, 0x32, 0x01,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusNoLogWithTrace);
            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusNoLogWithTrace));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void FullResponseInsufficientBuffer([Range(1, 146, 1)] int length)
        {
            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusFullInfo);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void FullResponseZeroLengthDescription()
        {
            GetLogInfoResponse response = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo);
            ContextId app1c1 = new ContextId("CTX1", LogLevel.Info, 1, "");
            ContextId app1c2 = new ContextId("CTX2", LogLevel.Debug, 1, "");
            AppId app1 = new AppId("APP1", "");
            app1.ContextIds.Add(app1c1);
            app1.ContextIds.Add(app1c2);
            ContextId app2c1 = new ContextId("CTX3", LogLevel.Error, 0, "");
            ContextId app2c2 = new ContextId("CTX4", LogLevel.Fatal, 1, "");
            AppId app2 = new AppId("APP2", "");
            app2.ContextIds.Add(app2c1);
            app2.ContextIds.Add(app2c2);
            response.AppIds.Add(app1);
            response.AppIds.Add(app2);

            byte[] expected = IsBigEndian ?
                new byte[] {
                    0x00, 0x00, 0x00, 0x03, 0x07,         // Service ID, status
                    0x00, 0x02,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x00, 0x02,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01, 0x00, 0x01, 0x00,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01, 0x00, 0x01, 0x00,
                    0x00, 0x01, 0x00,
                    0x41, 0x50, 0x50, 0x32, 0x00, 0x02,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00, 0x00, 0x01, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01, 0x00, 0x01, 0x00,
                    0x00, 0x01, 0x00,
                    0x00, 0x00, 0x00, 0x00                // COM
                } :
                new byte[] {
                    0x03, 0x00, 0x00, 0x00, 0x07,         // Service ID, status
                    0x02, 0x00,                           // app_ids_count = 2
                    0x41, 0x50, 0x50, 0x31, 0x02, 0x00,   // APP1, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x31, 0x04, 0x01, 0x01, 0x00, 0x00,
                    0x43, 0x54, 0x58, 0x32, 0x05, 0x01, 0x01, 0x00, 0x00,
                    0x01, 0x00, 0x00,
                    0x41, 0x50, 0x50, 0x32, 0x02, 0x00,   // APP2, ctx_ids_count = 2
                    0x43, 0x54, 0x58, 0x33, 0x02, 0x00, 0x01, 0x00, 0x00,
                    0x43, 0x54, 0x58, 0x34, 0x01, 0x01, 0x01, 0x00, 0x00,
                    0x01, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00                // COM
                };

            Span<byte> buffer = ControlEncode(response, expected.Length);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusFullInfo));

            Assert.That(buffer.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void FullResponseZeroLengthDescriptionInsufficientBuffer([Range(1, 64, 1)] int length)
        {
            GetLogInfoResponse response = new GetLogInfoResponse(GetLogInfoResponse.StatusFullInfo);
            ContextId app1c1 = new ContextId("CTX1", LogLevel.Info, 1, "");
            ContextId app1c2 = new ContextId("CTX2", LogLevel.Debug, 1, "");
            AppId app1 = new AppId("APP1", "");
            app1.ContextIds.Add(app1c1);
            app1.ContextIds.Add(app1c2);
            ContextId app2c1 = new ContextId("CTX3", LogLevel.Error, 0, "");
            ContextId app2c2 = new ContextId("CTX4", LogLevel.Fatal, 1, "");
            AppId app2 = new AppId("APP2", "");
            app2.ContextIds.Add(app2c1);
            app2.ContextIds.Add(app2c2);
            response.AppIds.Add(app1);
            response.AppIds.Add(app2);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void NoMatch()
        {
            GetLogInfoResponse response = GetResponse(GetLogInfoResponse.StatusNoMatch);
            Span<byte> buffer = ControlEncode(response, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoResponse.StatusNoMatch));

            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }
    }
}
