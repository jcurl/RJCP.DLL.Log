namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class VerboseArgDecoderTest
    {
        [Test]
        public void UnknownTypeInfo()
        {
            VerboseArgDecoder decoder = new VerboseArgDecoder();
            Result<int> length = decoder.Decode(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, false, out IDltArg arg);
            Assert.That(length.HasValue, Is.False);
            Assert.That(arg, Is.Null);
        }
    }
}
