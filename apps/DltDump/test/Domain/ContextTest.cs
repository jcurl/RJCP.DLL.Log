namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Constraints;
    using TestResources;

    [TestFixture]
    public class ContextTest
    {
        [Test]
        public void FilterNull()
        {
            Assert.That(() => {
                _ = new Context(null, 1, 1);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NegativeBeforeContext()
        {
            Assert.That(() => {
                _ = new Context(new Constraint().None, -1, 1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void NegativeAfterContext()
        {
            Assert.That(() => {
                _ = new Context(new Constraint().None, 1, -1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0, 0, 1)]
        [TestCase(1, 0, 2)]
        [TestCase(0, 1, 2)]
        [TestCase(2, 1, 4)]
        public void ContextCheck(int before, int after, int matches)
        {
            ContextPacket[] lines = {
                new ContextPacket(TestLines.Verbose, new byte[] { 0x00 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x01 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x02 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x03 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x04 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x05 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x06 })
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
            Assert.That(Matches(lines, before, after, true), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 3)]
        [TestCase(0, 1, 3)]
        [TestCase(2, 1, 5)]
        public void ContextCheckTwoMatchesSpace0(int before, int after, int matches)
        {
            ContextPacket[] lines = {
                new ContextPacket(TestLines.Verbose, new byte[] { 0x00 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x01 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x02 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x03 }),   // Match
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x04 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x05 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x06 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x07 }),
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
            Assert.That(Matches(lines, before, after, true), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 6)]
        public void ContextCheckTwoMatchesSpace1(int before, int after, int matches)
        {
            ContextPacket[] lines = {
                new ContextPacket(TestLines.Verbose, new byte[] { 0x00 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x01 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x02 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x03 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x04 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x05 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x06 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x07 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x08 }),
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
            Assert.That(Matches(lines, before, after, true), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 7)]
        public void ContextCheckTwoMatchesSpace2(int before, int after, int matches)
        {
            ContextPacket[] lines = {
                new ContextPacket(TestLines.Verbose, new byte[] { 0x00 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x01 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x02 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x03 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x04 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x05 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x06 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x07 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x08 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x09 }),
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
            Assert.That(Matches(lines, before, after, true), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 8)]
        public void ContextCheckTwoMatchesSpace3(int before, int after, int matches)
        {
            ContextPacket[] lines = {
                new ContextPacket(TestLines.Verbose, new byte[] { 0x00 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x01 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x02 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x03 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x04 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x05 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x06 }),
                new ContextPacket(TestLines.Verbose2, new byte[] { 0x07 }),   // Match
                new ContextPacket(TestLines.Verbose, new byte[] { 0x08 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x09 }),
                new ContextPacket(TestLines.Verbose, new byte[] { 0x0A }),
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
            Assert.That(Matches(lines, before, after, true), Is.EqualTo(matches));
        }

        private static int Matches(IEnumerable<ContextPacket> lines, int before, int after)
        {
            return Matches(lines, before, after, false);
        }

        private static int Matches(IEnumerable<ContextPacket> lines, int before, int after, bool packets)
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            Context context = new Context(filter, before, after);
            int matches = 0;

            // This example shows how to use the Context class. A check is always needed, so that the after context is
            // reset.
            foreach (ContextPacket line in lines) {
                bool check = packets ? context.Check(line.Line, line.Packet) : context.Check(line.Line);
                if (check) {
                    foreach (ContextPacket beforeLine in context.GetBeforeContext()) {
                        Console.WriteLine("B: {0} (packet: {1})", beforeLine.Line, GetPacket(beforeLine));
                        matches++;
                    }
                    Console.WriteLine("M: {0} (packet: {1})", line.Line, packets ? GetPacket(line) : "not used");
                    matches++;
                } else if (context.IsAfterContext()) {
                    Console.WriteLine("A: {0} (packet: {1})", line.Line, packets ? GetPacket(line) : "not used");
                    matches++;
                }
            }

            return matches;
        }

        private static string GetPacket(ContextPacket packet)
        {
            if (packet.Packet == null) return "null";
            return packet.Packet[0].ToString();
        }

        [Test]
        public void NoContextNoMatch()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            Context context = new Context(filter, 0, 0);

            Assert.That(context.Check(TestLines.Verbose), Is.False);
            Assert.That(context.GetBeforeContext(), Is.Empty);
            Assert.That(context.IsAfterContext, Is.False);
        }

        [Test]
        public void NoContextMatch()
        {
            Constraint filter = new Constraint().DltAppId("APP1");
            Context context = new Context(filter, 0, 0);

            Assert.That(context.Check(TestLines.Verbose), Is.True);
            Assert.That(context.GetBeforeContext(), Is.Empty);
            Assert.That(context.IsAfterContext, Is.False);
        }
    }
}
