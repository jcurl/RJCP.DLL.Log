namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Diagnostics.Log.Constraints;
    using NUnit.Framework;
    using TestResources;

    [TestFixture]
    public class FilterOutputTest
    {
        [Test]
        public void FilterNull()
        {
            IOutputStream output = new MemoryOutput();
            Assert.That(() => {
                _ = new FilterOutput(null, output);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OutputNull()
        {
            Constraint filter = new Constraint().None;
            Assert.That(() => {
                _ = new FilterOutput(filter, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FilterSupportsBinary(bool binary)
        {
            Constraint filter = new Constraint().DltAppId("APP1");
            MemoryOutput output = new MemoryOutput(binary);
            using (IOutputStream filterOutput = new FilterOutput(filter, output)) {
                Assert.That(filterOutput.SupportsBinary, Is.EqualTo(binary));
            }
        }

        [Test]
        public void FilterLinePass()
        {
            Constraint filter = new Constraint().DltAppId("APP1");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new FilterOutput(filter, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.True);
                Assert.That(output.Lines.Count, Is.EqualTo(1));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.Null);
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterLineFail()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new FilterOutput(filter, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);
                Assert.That(output.Lines.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void FilterPacketPass()
        {
            byte[] packet = new byte[] { 0x00, 0x01 };
            Constraint filter = new Constraint().DltAppId("APP1");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new FilterOutput(filter, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose, packet.AsSpan()), Is.True);
                Assert.That(output.Lines.Count, Is.EqualTo(1));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.EqualTo(new byte[] { 0x00, 0x01 }));
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterPacketFail()
        {
            byte[] packet = new byte[] { 0x00, 0x01 };
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new FilterOutput(filter, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose, packet.AsSpan()), Is.False);
                Assert.That(output.Lines.Count, Is.EqualTo(0));
            }
        }
    }
}
