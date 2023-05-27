namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using NUnit.Framework;
    using RJCP.Core;

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
            Result<int> length = dltDecoder.Decode(Array.Empty<byte>(), lineBuilder);
            Assert.That(length.Value, Is.EqualTo(0));
        }
    }
}
