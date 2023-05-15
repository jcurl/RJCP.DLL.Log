namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using Dlt.Args;
    using NUnit.Framework;

    [TestFixture]
    public class DltTraceEncoderTest
    {
        [Test]
        public void NullArgEncoder()
        {
            Assert.That(() => {
                _ = new DltTraceEncoder(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EncodeNullLine()
        {
            byte[] buffer = new byte[65535];
            ITraceEncoder<DltTraceLineBase> encoder = new DltTraceEncoder();
            Assert.That(() => {
                encoder.Encode(buffer, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TooManyArgs()
        {
            byte[] buffer = new byte[65535];
            ITraceEncoder<DltTraceLineBase> encoder = new DltTraceEncoder();

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetSessionId(15)
                .SetDltType(DltType.LOG_INFO);
            for (int i = 0; i < 257; i++) {
                builder.AddArgument(new BoolDltArg(false));
            }

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(-1));
        }

        private class UnknownTraceLine : DltTraceLineBase { }

        [Test]
        public void UnknownLine()
        {
            byte[] buffer = new byte[65535];
            ITraceEncoder<DltTraceLineBase> encoder = new DltTraceEncoder();

            int result = encoder.Encode(buffer, new UnknownTraceLine());
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
