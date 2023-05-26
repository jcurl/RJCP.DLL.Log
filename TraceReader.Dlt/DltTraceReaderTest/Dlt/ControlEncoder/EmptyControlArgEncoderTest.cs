namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using System.Collections.Generic;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(EmptyControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(EmptyControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class EmptyControlArgEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public EmptyControlArgEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        private static readonly IList<ControlRequest> EmptyRequests = new List<ControlRequest>() {
            new GetDefaultLogLevelRequest(),
            new StoreConfigurationRequest(),
            new ResetFactoryDefaultRequest(),
            new GetSoftwareVersionRequest(),
            new GetDefaultTraceStatusRequest(),
            new BufferOverflowNotificationRequest(),
            new SyncTimeStampRequest(),
            new GetLocalTimeRequest(),
            new MessageBufferOverflowRequest(),
            new GetVerboseModeStatusRequest(),
            new GetMessageFilteringStatusRequest(),
            new GetUseEcuIdRequest(),
            new GetUseSessionIdRequest(),
            new GetUseTimeStampRequest(),
            new GetUseExtendedHeaderRequest()
        };

        [Test]
        public void EmptyRequest([ValueSource(nameof(EmptyRequests))] ControlRequest request)
        {
            Span<byte> buffer = ControlEncode(request, 4);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(request.ServiceId));
        }

        [Test]
        public void InsufficientBuffer(
            [ValueSource(nameof(EmptyRequests))] ControlRequest request,
            [Range(1, 3, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
