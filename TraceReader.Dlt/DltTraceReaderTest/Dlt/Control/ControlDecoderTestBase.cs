namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;

    using ControlArgs;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.Core;

    /// <summary>
    /// Common test code for control decoders.
    /// </summary>
    /// <typeparam name="TReqDecoder">The type of the request decoder for instantiation.</typeparam>
    /// <typeparam name="TResDecoder">The type of the response decoder for instantiation.</typeparam>
    public abstract class ControlDecoderTestBase<TReqDecoder, TResDecoder> : DecoderTestBase
        where TReqDecoder : IControlArgDecoder
        where TResDecoder : IControlArgDecoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDecoderTestBase{TReqDecoder, TResDecoder}"/> class.
        /// </summary>
        /// <param name="decoderType">The decoder that should be used to test the byte sequence.</param>
        /// <param name="endian">Indicates the endianness flag to set in the standard header.</param>
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
        protected ControlDecoderTestBase(DecoderType decoderType, Endianness endian, int serviceId, Type requestType, Type responseType)
            : base(decoderType, endian)
        {
            ServiceId = serviceId;
            m_RequestType = requestType;
            m_ResponseType = responseType;
        }

        protected int ServiceId { get; }

        private readonly Type m_RequestType;
        private readonly Type m_ResponseType;

        /// <summary>
        /// A custom factory for creating our TraceReader.
        /// </summary>
        /// <value>The custom factory.</value>
        protected virtual DltFactory CustomFactory { get { return null; } }

        /// <summary>
        /// Gets the custom control decoder.
        /// </summary>
        /// <value>The custom control decoder.</value>
        protected virtual IControlDltDecoder CustomDecoder { get { return new ControlDltDecoder(); } }

        /// <summary>
        /// Decodes the specified decoder type.
        /// </summary>
        /// <param name="dltType">Type of the control message.</param>
        /// <param name="data">The data.</param>
        /// <param name="fileName">For the <see cref="DecoderType.Line"/>, specify the file name to write to.</param>
        /// <param name="service">The service.</param>
        /// <exception cref="NotImplementedException">
        /// The requested decoder type <see cref="Type"/> used when instantiating this class is unknown.
        /// </exception>
        protected void Decode(DltType dltType, byte[] data, string fileName, out IControlArg service)
        {
            switch (Type) {
            case DecoderType.Line:
                DecodeLine(dltType, data, fileName, out service);
                break;
            case DecoderType.Packet:
                DecodePacket(dltType, data, out service);
                break;
            case DecoderType.Specialized:
                DecodeSpecialized(dltType, data, out service);
                break;
            default:
                throw new NotImplementedException();
            }

            switch (dltType) {
            case DltType.CONTROL_REQUEST:
                Assert.That(service, Is.TypeOf(m_RequestType));
                Assert.That(service.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
                break;
            case DltType.CONTROL_RESPONSE:
                Assert.That(service, Is.TypeOf(m_ResponseType).Or.TypeOf<ControlErrorNotSupported>());
                if (service is ControlErrorNotSupported notSupported)
                    Assert.That(notSupported.Status, Is.Not.EqualTo(ControlResponse.StatusOk));
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
            DltTraceLineBase line = DecodeLine(CustomFactory, dltType, data, fileName);
            Assert.That(line, Is.TypeOf<DltControlTraceLine>());
            DltControlTraceLine control = (DltControlTraceLine)line;
            service = control.Service;
        }

        protected override void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf)
        {
            factory.Control(writer, DltTestData.Time1, DltTime.DeviceTime(1.231), dltType, data).BigEndian(msbf).Append();
        }

        private void DecodePacket(DltType dltType, byte[] data, out IControlArg service)
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetDltType(dltType);
            lineBuilder.SetBigEndian(Endian == Endianness.Big);

            int length = CustomDecoder.Decode(data, lineBuilder);
            service = lineBuilder.ControlPayload;
            Assert.That(length, Is.EqualTo(data.Length));
        }

        private void DecodeSpecialized(DltType dltType, byte[] data, out IControlArg service)
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

            bool isBig = Endian == Endianness.Big;
            int serviceId = BitOperations.To32Shift(data, !isBig);
            int length = argDecoder.Decode(serviceId, data, isBig, out service);
            Assert.That(length, Is.EqualTo(data.Length));

            // Now test every packet that is smaller and expect that it doesn't raise an exception
            for (int i = 4; i < data.Length - 1; i++) {
                length = argDecoder.Decode(serviceId, data.AsSpan()[0..i], isBig, out IControlArg testService);
                Assert.That(length, Is.EqualTo(-1).Or.EqualTo(i));
                if (length == -1) {
                    Assert.That(testService, Is.Null.Or.TypeOf<ControlDecodeError>());
                } else {
                    Assert.That(testService, Is.Not.Null);
                }
                if (testService != null)
                    Assert.That(testService.DefaultType, Is.EqualTo(dltType));
            }
        }
    }
}
