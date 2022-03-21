namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.Collections.Generic;
    using Diagnostics.Log.Dlt;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Constraints;
    using TestResources;

    [TestFixture]
    public class ContextTest
    {
        [TestCase(0, 0, 1)]
        [TestCase(1, 0, 2)]
        [TestCase(0, 1, 2)]
        [TestCase(2, 1, 4)]
        public void ContextCheck(int before, int after, int matches)
        {
            DltTraceLineBase[] lines = {
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 3)]
        [TestCase(0, 1, 3)]
        [TestCase(2, 1, 5)]
        public void ContextCheckTwoMatchesSpace0(int before, int after, int matches)
        {
            DltTraceLineBase[] lines = {
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 6)]
        public void ContextCheckTwoMatchesSpace1(int before, int after, int matches)
        {
            DltTraceLineBase[] lines = {
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 7)]
        public void ContextCheckTwoMatchesSpace2(int before, int after, int matches)
        {
            DltTraceLineBase[] lines = {
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
        }

        [TestCase(0, 0, 2)]
        [TestCase(1, 0, 4)]
        [TestCase(0, 1, 4)]
        [TestCase(2, 1, 8)]
        public void ContextCheckTwoMatchesSpace3(int before, int after, int matches)
        {
            DltTraceLineBase[] lines = {
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose2,   // Match
                TestLines.Verbose,
                TestLines.Verbose,
                TestLines.Verbose,
            };
            Assert.That(Matches(lines, before, after), Is.EqualTo(matches));
        }

        private static int Matches(IEnumerable<DltTraceLineBase> lines, int before, int after)
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            Context context = new Context(filter, before, after);
            int matches = 0;

            // This example shows how to use the Context class. A check is always needed, so that the after context is
            // reset.
            foreach (DltTraceLineBase line in lines) {
                if (context.Check(line)) {
                    foreach (DltTraceLineBase beforeLine in context.GetBeforeContext()) {
                        Console.WriteLine("B: {0}", beforeLine);
                        matches++;
                    }
                    Console.WriteLine("M: {0}", line);
                    matches++;
                } else if (context.IsAfterContext()) {
                    Console.WriteLine("A: {0}", line);
                    matches++;
                }
            }

            return matches;
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
