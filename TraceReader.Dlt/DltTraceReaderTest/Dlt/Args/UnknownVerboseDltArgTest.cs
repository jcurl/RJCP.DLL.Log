namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using NUnit.Framework;

    [TestFixture]
    public class UnknownVerboseDltArgTest
    {
        [Test]
        public void UnknownDltArg()
        {
            byte[] data = { 0x01, 0x23, 0x45, 0x67, 0x89 };
            UnknownVerboseDltArg arg = new UnknownVerboseDltArg(data, false);
            Assert.That(arg.Data, Is.EqualTo(data[4..]));
            Assert.That(arg.IsBigEndian, Is.False);
            Assert.That(arg.TypeInfo.Bytes, Is.EqualTo(data[0..4]));
            Assert.That(arg.TypeInfo.Encoding, Is.EqualTo(2));
            Assert.That(arg.TypeInfo.IsFixedPoint, Is.False);
            Assert.That(arg.TypeInfo.IsVariableInfo, Is.False);
            Assert.That(arg.TypeInfo.Length, Is.EqualTo(1));
            Assert.That(arg.ToString(), Is.EqualTo("Type Info: 01 23 45 67 Data: 89"));
        }
    }
}
