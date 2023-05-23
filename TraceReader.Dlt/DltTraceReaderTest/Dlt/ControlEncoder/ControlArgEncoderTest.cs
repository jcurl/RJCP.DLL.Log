namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class ControlArgEncoderTest
    {
        private sealed class UnknownControlRequest : ControlRequest
        {
            public UnknownControlRequest(int serviceId)
            {
                ServiceId = serviceId;
            }

            /// <summary>
            /// Gets the service identifier of the control message.
            /// </summary>
            /// <value>The service identifier of the control message.</value>
            public override int ServiceId { get; }
        }

        [Test]
        public void EncodeUnknownControlRequestMessage()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, false, new UnknownControlRequest(0x40));
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void EncodeUnknownCustomControlRequestMessage()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, false, new UnknownControlRequest(0xF40));
            Assert.That(result, Is.EqualTo(-1));
        }

        private sealed class UnknownControlResponse : ControlResponse
        {
            public UnknownControlResponse(int serviceId, int status) : base(status)
            {
                ServiceId = serviceId;
            }

            /// <summary>
            /// Gets the service identifier of the control message.
            /// </summary>
            /// <value>The service identifier of the control message.</value>
            public override int ServiceId { get; }
        }

        [Test]
        public void EncodeUnknownControlResponseMessage()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, false, new UnknownControlResponse(0x40, ControlResponse.StatusOk));
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void EncodeUnknownCustomControlResponseMessage()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, false, new UnknownControlResponse(0xF40, ControlResponse.StatusOk));
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void ControlErrorNotSupportedResponse([Values(false, true)] bool bigEndian)
        {
            byte[] buffer = new byte[1024];

            // Even though this is not registered with the `ControlArgEncoder`, it should still be encoded.
            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, bigEndian, new ControlErrorNotSupported(0x40, ControlResponse.StatusNotSupported, "name"));
            Assert.That(result, Is.EqualTo(5));

            byte[] expected = bigEndian ?
                new byte[] { 0x00, 0x00, 0x00, 0x40, 0x01 } :
                new byte[] { 0x40, 0x00, 0x00, 0x00, 0x01 };

            Assert.That(buffer[0..5], Is.EqualTo(expected));
        }

        private sealed class ControlArgEncoderUnreg : ControlArgEncoder
        {
            public ControlArgEncoderUnreg(int serviceId)
            {
                Unregister(serviceId, DltType.CONTROL_REQUEST);
                Unregister(serviceId, DltType.CONTROL_RESPONSE);
            }
        }

        [Test]
        public void UnregisteredExistingRequest()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoderUnreg(0x03); // GetLogInfoRequestEncoder
            GetLogInfoRequest request = new GetLogInfoRequest(GetLogInfoRequest.OptionsFullInfo, "APP1", "CTX1");

            int result = encoder.Encode(buffer, false, request);
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void UnregisteredExistingResponse()
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoderUnreg(0x13); // GetSoftwareVersionResponseEncoder
            GetSoftwareVersionResponse request = new GetSoftwareVersionResponse(ControlResponse.StatusOk, "Version1");

            int result = encoder.Encode(buffer, false, request);
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void UnregisteredNonExisting([Values(0x40, 0xF40)] int serviceId)
        {
            byte[] buffer = new byte[1024];

            // Should just ignore that it wasn't registered.
            IControlArgEncoder encoder = new ControlArgEncoderUnreg(serviceId);
            UnknownControlRequest request = new UnknownControlRequest(serviceId);

            int result = encoder.Encode(buffer, false, request);
            Assert.That(result, Is.EqualTo(-1));
        }

        private sealed class ControlArgEncoderUnregUnsupported : ControlArgEncoder
        {
            public ControlArgEncoderUnregUnsupported(int serviceId)
            {
                Unregister(serviceId, DltType.LOG_INFO);
            }
        }

        [Test]
        public void UnregisteredUnsupported()
        {
            byte[] buffer = new byte[1024];

            // Should just ignore that it is unsupported.
            IControlArgEncoder encoder = new ControlArgEncoderUnregUnsupported(0x03);
            GetLogInfoRequest request = new GetLogInfoRequest(GetLogInfoRequest.OptionsFullInfo, "APP1", "CTX1");

            int result = encoder.Encode(buffer, false, request);
            Assert.That(result, Is.EqualTo(17));
        }

        private sealed class ControlArgEncoderRegisterNew : ControlArgEncoder
        {
            public ControlArgEncoderRegisterNew(int serviceId)
            {
                Register(serviceId, DltType.CONTROL_RESPONSE, EmptyResponseEncoder);
                Register(serviceId, DltType.CONTROL_REQUEST, EmptyRequestEncoder);
            }
        }

        [Test]
        public void RegisteredNewResponse([Values(0x40, 0xF40)] int serviceId)
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoderRegisterNew(serviceId);
            int result = encoder.Encode(buffer, false, new UnknownControlResponse(serviceId, ControlResponse.StatusOk));
            Assert.That(result, Is.EqualTo(5));

            byte[] expected = new byte[5];
            BitOperations.Copy32ShiftLittleEndian(serviceId, expected);
            Assert.That(buffer[0..5], Is.EqualTo(expected));
        }

        [Test]
        public void RegisteredNewRequest([Values(0x40, 0xF40)] int serviceId)
        {
            byte[] buffer = new byte[1024];

            IControlArgEncoder encoder = new ControlArgEncoderRegisterNew(serviceId);
            int result = encoder.Encode(buffer, false, new UnknownControlRequest(serviceId));
            Assert.That(result, Is.EqualTo(4));

            byte[] expected = new byte[5];
            BitOperations.Copy32ShiftLittleEndian(serviceId, expected);
            Assert.That(buffer[0..5], Is.EqualTo(expected));
        }

        private sealed class ControlArgEncoderRegisterNull : ControlArgEncoder
        {
            public ControlArgEncoderRegisterNull()
            {
                Register(0x40, DltType.CONTROL_REQUEST, null);
            }
        }

        [Test]
        public void RegisterNull()
        {
            Assert.That(() => {
                _ = new ControlArgEncoderRegisterNull();
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private sealed class ControlArgEncoderUnsupported : ControlArgEncoder
        {
            public ControlArgEncoderUnsupported(int serviceId)
            {
                Register(serviceId, DltType.LOG_INFO, EmptyRequestEncoder);
            }
        }

        [Test]
        public void RegisterUnsupported([Values(0x40, 0xF40)] int serviceId)
        {
            Assert.That(() => {
                _ = new ControlArgEncoderUnsupported(serviceId);
            }, Throws.TypeOf<ArgumentException>());
        }

        private sealed class InvalidControl : IControlArg
        {
            public int ServiceId { get { return 0; } }

            public DltType DefaultType { get { return DltType.LOG_INFO; } }
        }

        [Test]
        public void EncodeInvalidControlMessage([Values(0, 64)] int len)
        {
            // We can't register this control message, and we should get an error trying to decode it.
            byte[] buffer = new byte[len];

            IControlArgEncoder encoder = new ControlArgEncoder();
            int result = encoder.Encode(buffer, false, new InvalidControl());
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
