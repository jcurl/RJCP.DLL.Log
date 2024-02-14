namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ControlDltEncoderTest
    {
        [Test]
        public void NullEncoder()
        {
            Assert.That(() => {
                _ = new ControlDltEncoder(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EncodeNullLine()
        {
            ControlDltEncoder encoder = new();
            Assert.That(() => {
                _ = encoder.Encode(new byte[1024], null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
