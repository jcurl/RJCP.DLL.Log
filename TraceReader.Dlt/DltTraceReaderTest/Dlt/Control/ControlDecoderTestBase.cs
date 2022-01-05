namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ControlArgs;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core;

    /// <summary>
    /// Common test code for control decoders.
    /// </summary>
    /// <typeparam name="TReqDecoder">The type of the request decoder for instantiation.</typeparam>
    /// <typeparam name="TResDecoder">The type of the response decoder for instantiation.</typeparam>
    public abstract class ControlDecoderTestBase<TReqDecoder, TResDecoder>
        where TReqDecoder : IControlArgDecoder
        where TResDecoder : IControlArgDecoder
    {
        [Flags]
        public enum DecoderType
        {
            Line = 1,
            Packet = 2,
            Specialized = 4,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecoderTestBase{TReqDecoder, TResDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="serviceId">The service identifier that is expected to be generated.</param>
        /// <param name="requestType">Type of the request that is expected to be generated.</param>
        /// <param name="responseType">Type of the response that is expected to be generated.</param>
        /// <remarks>
        /// The <paramref name="decoderType"/> defines the type of decoder that is used to decode the control payload.
        /// <list type="bullet">
        /// <item>
        /// <see cref="DecoderType.Line"/>: The payload is put in a proper DLT packet with storage header, and the main
        /// decoder is used.
        /// </item>
        /// <item>
        /// <see cref="DecoderType.Packet"/>: The <see cref="ControlDltDecoder"/> is used to decode the payload.
        /// </item>
        /// <item>
        /// <see cref="DecoderType.Specialized"/>: The <see cref="TReqDecoder"/> or <see cref="TResDecoder"/> is used to
        /// decode the payload.
        /// </item>
        /// </list>
        /// </remarks>
        protected ControlDecoderTestBase(DecoderType decoderType, int serviceId, Type requestType, Type responseType)
        {
            Type = decoderType;
            ServiceId = serviceId;
            m_RequestType = requestType;
            m_ResponseType = responseType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecoderTestBase{TReqDecoder, TResDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="serviceId">The service identifier that is expected to be generated.</param>
        /// <param name="requestType">Type of the request that is expected to be generated.</param>
        /// <param name="responseType">Type of the response that is expected to be generated.</param>
        /// <param name="factory">The custom factory to test lines.</param>
        /// <remarks>
        /// The <paramref name="decoderType"/> defines the type of decoder that is used to decode the control payload.
        /// <list type="bullet">
        /// <item>
        /// <see cref="DecoderType.Line"/>: The payload is put in a proper DLT packet with storage header, and the main
        /// decoder is used.
        /// </item>
        /// <item>
        /// <see cref="DecoderType.Packet"/>: The <see cref="ControlDltDecoder"/> is used to decode the payload.
        /// </item>
        /// <item>
        /// <see cref="DecoderType.Specialized"/>: The <see cref="TReqDecoder"/> or <see cref="TResDecoder"/> is used to
        /// decode the payload.
        /// </item>
        /// </list>
        /// </remarks>
        protected ControlDecoderTestBase(DecoderType decoderType, int serviceId, Type requestType, Type responseType,
            DltFactory factory, IControlDltDecoder ctlDecoder)
        {
            Type = decoderType;
            ServiceId = serviceId;
            m_RequestType = requestType;
            m_ResponseType = responseType;
            m_CustomFactory = factory;
            m_CustomDecoder = ctlDecoder;
        }

        protected DecoderType Type { get; }

        protected int ServiceId { get; }

        private readonly Type m_RequestType;
        private readonly Type m_ResponseType;
        private readonly DltFactory m_CustomFactory;
        private readonly IControlDltDecoder m_CustomDecoder;

        /// <summary>
        /// Decodes the specified decoder type.
        /// </summary>
        /// <param name="decoderType">Type of the control message.</param>
        /// <param name="data">The data.</param>
        /// <param name="fileName">For the <see cref="DecoderType.Line"/>, specify the file name to write to.</param>
        /// <param name="service">The service.</param>
        /// <exception cref="NotImplementedException">
        /// The requested decoder type <see cref="Type"/> used when instantiating this class is unknown.
        /// </exception>
        protected void Decode(DltType dltType, byte[] data, string fileName, out IControlArg service)
        {
            if (Type.HasFlag(DecoderType.Line)) {
                DecodeLine(dltType, data, fileName, out service);
            } else if (Type.HasFlag(DecoderType.Packet)) {
                DecodePacket(dltType, data, out service);
            } else if (Type.HasFlag(DecoderType.Specialized)) {
                DecodeSpecialized(dltType, data, out service);
            } else {
                throw new NotImplementedException();
            }

            switch (dltType) {
            case DltType.CONTROL_REQUEST:
                Assert.That(service, Is.TypeOf(m_RequestType));
                Assert.That(service.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
                break;
            case DltType.CONTROL_RESPONSE:
                Assert.That(service, Is.TypeOf(m_ResponseType));
                Assert.That(service.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
                break;
            case DltType.CONTROL_TIME:
                Assert.That(service, Is.TypeOf(m_ResponseType));
                Assert.That(service.DefaultType, Is.EqualTo(DltType.CONTROL_TIME));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dltType), "Invalid DltType requested for this test case");
            }

            Assert.That(service.ServiceId, Is.EqualTo(ServiceId));
        }

        private void DecodeLine(DltType dltType, byte[] data, string fileName, out IControlArg service)
        {
            DltFactory factory;
            if (m_CustomFactory != null) {
                factory = m_CustomFactory;
            } else {
                factory = new DltFactory(DltFactoryType.File);
            }

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                factory.Control(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), dltType, data).Append();
                using (Stream stream = writer.Stream()) {
                    Task<DltTraceLineBase> lineTask = WriteDltPacket(factory, stream);
                    DltTraceLineBase line = lineTask.GetAwaiter().GetResult();
                    Assert.That(line, Is.TypeOf<DltControlTraceLine>());
                    DltControlTraceLine control = (DltControlTraceLine)line;
                    service = control.Service;

                    if (!string.IsNullOrEmpty(fileName)) {
                        string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "control");
                        string outPath = Path.Combine(dir, $"{fileName}.dlt");
                        if (!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }
                        writer.Write(outPath);
                    }
                }
            }
        }

        private static async Task<DltTraceLineBase> WriteDltPacket(DltFactory factory, Stream stream)
        {
            DltTraceLineBase line;
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                line = await reader.GetLineAsync();

                // This is the only way to check the length, that this was the only packet decoded.
                DltTraceLineBase lastLine = await reader.GetLineAsync();
                Assert.That(lastLine, Is.Null);
                return line;
            }
        }

        private void DecodePacket(DltType dltType, byte[] data, out IControlArg service)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(dltType);

            IControlDltDecoder dltDecoder;
            if (m_CustomDecoder != null) {
                dltDecoder = m_CustomDecoder;
            } else {
                dltDecoder = new ControlDltDecoder();
            }

            int length = dltDecoder.Decode(data, lineBuilder);
            service = lineBuilder.ControlPayload;
            Assert.That(length, Is.EqualTo(data.Length));
        }

        private static void DecodeSpecialized(DltType dltType, byte[] data, out IControlArg service)
        {
            IControlArgDecoder argDecoder;
            switch (dltType) {
            case DltType.CONTROL_REQUEST:
                argDecoder = Activator.CreateInstance<TReqDecoder>();
                break;
            case DltType.CONTROL_RESPONSE:
                argDecoder = Activator.CreateInstance<TResDecoder>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dltType), "Invalid DltType requested for this test case");
            }

            int serviceId = BitOperations.To32ShiftLittleEndian(data);
            int length = argDecoder.Decode(serviceId, data, out service);
            Assert.That(length, Is.EqualTo(data.Length));
        }
    }
}
