namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using System.Collections.Generic;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(ControlArgResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    public class ControlArgResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public ControlArgResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        private static readonly IList<ControlResponse> EmptyResponses = new List<ControlResponse>() {
            new SetLogLevelResponse(ControlResponse.StatusOk),
            new SetLogLevelResponse(ControlResponse.StatusError),
            new SetTraceStatusResponse(ControlResponse.StatusOk),
            new SetTraceStatusResponse(ControlResponse.StatusError),
            new StoreConfigurationResponse(ControlResponse.StatusOk),
            new StoreConfigurationResponse(ControlResponse.StatusError),
            new ResetFactoryDefaultResponse(ControlResponse.StatusOk),
            new ResetFactoryDefaultResponse(ControlResponse.StatusError),
            new SetMessageFilteringResponse(ControlResponse.StatusOk),
            new SetMessageFilteringResponse(ControlResponse.StatusError),
            new SetDefaultLogLevelResponse(ControlResponse.StatusOk),
            new SetDefaultLogLevelResponse(ControlResponse.StatusError),
            new SetDefaultTraceStatusResponse(ControlResponse.StatusOk),
            new SetDefaultTraceStatusResponse(ControlResponse.StatusError),
            new SetVerboseModeResponse(ControlResponse.StatusOk),
            new SetVerboseModeResponse(ControlResponse.StatusError),
            new SetTimingPacketsResponse(ControlResponse.StatusOk),
            new SetTimingPacketsResponse(ControlResponse.StatusError),
            new GetLocalTimeResponse(ControlResponse.StatusOk),
            new GetLocalTimeResponse(ControlResponse.StatusError),
            new SetUseEcuIdResponse(ControlResponse.StatusOk),
            new SetUseEcuIdResponse(ControlResponse.StatusError),
            new SetUseSessionIdResponse(ControlResponse.StatusOk),
            new SetUseSessionIdResponse(ControlResponse.StatusError),
            new SetUseTimeStampResponse(ControlResponse.StatusOk),
            new SetUseTimeStampResponse(ControlResponse.StatusError),
            new SetUseExtendedHeaderResponse(ControlResponse.StatusOk),
            new SetUseExtendedHeaderResponse(ControlResponse.StatusError),
            new CustomMarkerResponse(ControlResponse.StatusOk),
            new CustomMarkerResponse(ControlResponse.StatusError)
        };

        [Test]
        public void EmptyRequest([ValueSource(nameof(EmptyResponses))] ControlResponse response)
        {
            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(response.ServiceId));
            Assert.That(buffer[4], Is.EqualTo(response.Status));
        }
    }
}
