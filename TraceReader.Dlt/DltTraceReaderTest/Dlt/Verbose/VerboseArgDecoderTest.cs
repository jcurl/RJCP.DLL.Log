namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using Args;
    using NUnit.Framework;

    [TestFixture]
    public class VerboseArgDecoderTest
    {
        [Test]
        public void UnknownTypeInfo()
        {
            VerboseArgDecoder decoder = new VerboseArgDecoder();
            int length = decoder.Decode(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, false, out IDltArg args);
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(args, Is.Null.Or.TypeOf<DltArgError>());
        }
    }
}
