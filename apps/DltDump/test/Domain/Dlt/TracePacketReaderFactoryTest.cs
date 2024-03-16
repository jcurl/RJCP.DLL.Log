namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Threading.Tasks;
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
        public async Task NullPacket()
        {
            TracePacketReaderFactory factory = new(new DltTraceDecoderFactory());
            await Assert.ThatAsync(async () => {
                _ = await factory.CreateAsync(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
