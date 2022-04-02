namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Constraints;
    using TestResources;

    [TestFixture]
    public class ContextOutputTest
    {
        [Test]
        public void FilterNull()
        {
            IOutputStream output = new MemoryOutput();
            Assert.That(() => {
                _ = new ContextOutput(null, 1, 1, output);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OutputNull()
        {
            Constraint filter = new Constraint().None;
            Assert.That(() => {
                _ = new ContextOutput(filter, 1, 1, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NegativeBeforeContext()
        {
            Constraint filter = new Constraint().None;
            IOutputStream output = new MemoryOutput();
            Assert.That(() => {
                _ = new ContextOutput(filter, -1, 1, output);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void NegativeAfterContext()
        {
            Constraint filter = new Constraint().None;
            IOutputStream output = new MemoryOutput();
            Assert.That(() => {
                _ = new ContextOutput(filter, 1, -1, output);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FilterSupportsBinary(bool binary)
        {
            Constraint filter = new Constraint().DltAppId("APP1");
            MemoryOutput output = new MemoryOutput(binary);
            using (IOutputStream filterOutput = new ContextOutput(filter, 1, 1, output)) {
                Assert.That(filterOutput.SupportsBinary, Is.EqualTo(binary));
            }
        }

        [Test]
        public void FilterLineContextBefore()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 1, 0, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(2));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.Null);
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[1].Packet, Is.Null);
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterLineContextAfter()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 0, 1, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(2));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[0].Packet, Is.Null);
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[1].Packet, Is.Null);
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterLineContext()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 1, 1, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(3));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.Null);
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[1].Packet, Is.Null);
                Assert.That(output.Lines[2].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[2].Packet, Is.Null);
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        private static readonly byte[] packet1 = new byte[] { 0x00, 0x01 };
        private static readonly byte[] packet2 = new byte[] { 0x00, 0x02 };
        private static readonly byte[] packet3 = new byte[] { 0x00, 0x03 };

        [Test]
        public void FilterPacketContextBefore()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 1, 0, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose, packet1), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2, packet2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose, packet3), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(2));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.EqualTo(new byte[] { 0x00, 0x01 }));
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[1].Packet, Is.EqualTo(new byte[] { 0x00, 0x02 }));
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterPacketContextAfter()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 0, 1, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose, packet1), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2, packet2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose, packet3), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(2));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[0].Packet, Is.EqualTo(new byte[] { 0x00, 0x02 }));
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[1].Packet, Is.EqualTo(new byte[] { 0x00, 0x03 }));
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterPacketContext()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            MemoryOutput output = new MemoryOutput();
            using (IOutputStream filterOutput = new ContextOutput(filter, 1, 1, output)) {
                Assert.That(filterOutput.Write(TestLines.Verbose, packet1), Is.False);
                Assert.That(filterOutput.Write(TestLines.Verbose2, packet2), Is.True);
                Assert.That(filterOutput.Write(TestLines.Verbose, packet3), Is.False);

                Assert.That(output.Lines.Count, Is.EqualTo(3));
                Assert.That(output.Lines[0].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[0].Packet, Is.EqualTo(new byte[] { 0x00, 0x01 }));
                Assert.That(output.Lines[1].Line, Is.EqualTo(TestLines.Verbose2));
                Assert.That(output.Lines[1].Packet, Is.EqualTo(new byte[] { 0x00, 0x02 }));
                Assert.That(output.Lines[2].Line, Is.EqualTo(TestLines.Verbose));
                Assert.That(output.Lines[2].Packet, Is.EqualTo(new byte[] { 0x00, 0x03 }));
            }

            // Dispose for filterOutput was propagated.
            Assert.That(output.Lines.Count, Is.EqualTo(0));
        }
    }
}
