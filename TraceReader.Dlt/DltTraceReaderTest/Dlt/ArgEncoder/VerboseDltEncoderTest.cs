namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture]
    public class VerboseDltEncoderTest
    {
        [TestCase(65536)]
        [TestCase(25)]
        public void WriteMultipleArgs(int len)
        {
            byte[] buffer = new byte[len];
            VerboseDltEncoder encoder = new VerboseDltEncoder();

            DltTraceLine line = new DltTraceLine(new IDltArg[] {
                new StringDltArg("Temperature:"),
                new SignedIntDltArg(45, 2)
            }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                DeviceTimeStamp = new TimeSpan(0)
            };
            line.Features += DltLineFeatures.VerboseFeature;

            Assert.That(encoder.Encode(buffer, line), Is.EqualTo(25));
        }

        [TestCase(0)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(23)]
        public void WriteInsufficientBuffer(int len)
        {
            byte[] buffer = new byte[len];
            VerboseDltEncoder encoder = new VerboseDltEncoder();

            DltTraceLine line = new DltTraceLine(new IDltArg[] {
                new StringDltArg("Temperature:"),
                new SignedIntDltArg(45, 2)
            }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                DeviceTimeStamp = new TimeSpan(0)
            };
            line.Features += DltLineFeatures.VerboseFeature;

            Assert.That(encoder.Encode(buffer, line), Is.EqualTo(-1));
        }
    }
}
