namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;

    [TestFixture]
    public class TracePacketReaderFactoryTest
    {
        [Test]
        public void NullFactory()
        {
            Assert.That(() => {
                _ = new TracePacketReaderFactory(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullPacket()
        {
            TracePacketReaderFactory factory = new TracePacketReaderFactory(new DltTraceDecoderFactory());
            Assert.That(async () => {
                _ = await factory.CreateAsync(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
