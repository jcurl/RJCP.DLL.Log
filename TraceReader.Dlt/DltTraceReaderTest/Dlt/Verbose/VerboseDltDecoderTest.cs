namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VerboseDltDecoderTest
    {
        [Test]
        public void DecodeNoArguments()
        {
            IDltLineBuilder lineBuilder = new DltLineBuilder();
            lineBuilder.SetNumberOfArgs(0);
            lineBuilder.SetDltType(DltType.LOG_INFO);

            IVerboseDltDecoder dltDecoder = new VerboseDltDecoder(new VerboseArgDecoder());
            int length = dltDecoder.Decode(Array.Empty<byte>(), lineBuilder);
            Assert.That(length, Is.EqualTo(0));
        }
    }
}
