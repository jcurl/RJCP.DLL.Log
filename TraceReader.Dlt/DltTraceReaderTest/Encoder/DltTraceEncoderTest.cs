﻿namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using Dlt.ArgEncoder;
    using Dlt.Args;
    using Dlt.ControlEncoder;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class DltTraceEncoderTest
    {
        [Test]
        public void NullArgEncoder()
        {
            Assert.That(() => {
                _ = new DltTraceEncoder(null, new ControlDltEncoder());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullControlEncoder()
        {
            Assert.That(() => {
                _ = new DltTraceEncoder(new VerboseDltEncoder(), null);
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

            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.HasValue, Is.False);
        }

        private class UnknownTraceLine : DltTraceLineBase { }

        [Test]
        public void UnknownLine()
        {
            byte[] buffer = new byte[65535];
            ITraceEncoder<DltTraceLineBase> encoder = new DltTraceEncoder();

            Result<int> result = encoder.Encode(buffer, new UnknownTraceLine());
            Assert.That(result.HasValue, Is.False);
        }

        [TestCase(null, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase("", new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase("A", new byte[] { 0x41, 0x00, 0x00, 0x00 })]
        [TestCase("AB", new byte[] { 0x41, 0x42, 0x00, 0x00 })]
        [TestCase("ABC", new byte[] { 0x41, 0x42, 0x43, 0x00 })]
        [TestCase("ABCD", new byte[] { 0x41, 0x42, 0x43, 0x44 })]
        [TestCase("Ü", new byte[] { 0x5C, 0x00, 0x00, 0x00 })] // Undefined, will be stripped to lower 7 bits
        public void WriteId(string id, byte[] expected)
        {
            byte[] buffer = new byte[4];
            DltTraceEncoder.WriteId(buffer, id);
            Assert.That(buffer, Is.EqualTo(expected));
        }
    }
}
