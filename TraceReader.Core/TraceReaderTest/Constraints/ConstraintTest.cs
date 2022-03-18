namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture(ConstraintOptions.None)]
    [TestFixture(ConstraintOptions.Interpreted)]
    [TestFixture(ConstraintOptions.Compiled)]
    public class ConstraintTest
    {
        public enum ConstraintOptions
        {
            None,
            Interpreted,
            Compiled
        }

        private readonly ConstraintOptions m_Options;

        public ConstraintTest(ConstraintOptions option)
        {
            m_Options = option;
        }

        private Constraint Constraint()
        {
            switch (m_Options) {
            case ConstraintOptions.None: return new Constraint();
            case ConstraintOptions.Interpreted: return new Constraint(Constraints.ConstraintOptions.Interpreted);
            case ConstraintOptions.Compiled: return new Constraint(Constraints.ConstraintOptions.Compiled);
            default: throw new InvalidOperationException("Unknown test case option");
            }
        }

        [Test]
        public void Initialize()
        {
            TraceLine line = new TraceLine("A", 0, 0);
            Constraint c = Constraint();

            // We haven't given it a valid constraint.
            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [Test]
        public void None()
        {
            TraceLine line = new TraceLine("A", 0, 0);
            IMatchConstraint c = Constraint().None;

            Assert.That(c.Check(line), Is.True);
        }

        [TestCase("Foo", "Foo")]
        [TestCase("Foo", "Bar")]
        public void NoneFirst(string text, string match)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().None.TextEquals(match);

            Assert.That(c.Check(line), Is.EqualTo(text == match));
        }

        [TestCase("Foo", "Foo")]
        [TestCase("Foo", "Bar")]
        public void NoneLast(string text, string match)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextEquals(match).None;

            Assert.That(c.Check(line), Is.EqualTo(text == match));
        }

        [Test]
        public void NotNone()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().Not.None;

            // Will always fail. Users should really not be writing such test cases.
            Assert.That(c.Check(line), Is.False);
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        public void TextString(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextString(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void TextStringMultipleMatch()
        {
            TraceLine line = new TraceLine("Apple", 0, 0);
            Constraint c = Constraint().TextString("App").TextString("ple");

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void TextStringMultipleMismatch1()
        {
            TraceLine line = new TraceLine("Apple", 0, 0);
            Constraint c = Constraint().TextString("Ban").TextString("ple");

            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void TextStringMultipleMismatch2()
        {
            TraceLine line = new TraceLine("Apple", 0, 0);
            Constraint c = Constraint().TextString("App").TextString("ana");

            Assert.That(c.Check(line), Is.False);
        }

        [TestCase("asdasStrassedfsdf", "Strasse", true)]
        [TestCase("Strasseasdasd", "Strasse", true)]
        [TestCase("Straßeasdasd", "Strasse", false)]
        public void TextString_InvariantCulture(string text, string match, bool result)
        {
            string originalCulture = Thread.CurrentThread.CurrentCulture.Name;

            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");

                TraceLine line = new TraceLine(text, 0, 0);
                Constraint c = Constraint().TextString(match);
                Assert.That(c.Check(line), Is.EqualTo(result));
            } finally {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            }
        }

        [Test]
        public void TextStringNull()
        {
            Assert.That(() => {
                _ = Constraint().TextString(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", false)]
        [TestCase("Apple", "ple", false)]
        [TestCase("Apple", "le", false)]
        [TestCase("Apple", "", false)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        public void TextEquals(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextEquals(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [TestCase("Straßesdfsdf", "Strasse", false)]
        [TestCase("Strassesdfsdf", "Strasse", true)]
        public void TextEquals_InvariantCulture(string text, string match, bool result)
        {
            string originalCulture = Thread.CurrentThread.CurrentCulture.Name;

            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-MO");

                TraceLine line = new TraceLine(text, 0, 0);
                Constraint c = Constraint().TextStartsWith(match);
                Assert.That(c.Check(line), Is.EqualTo(result));

            } finally {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            }
        }

        [Test]
        public void TextEqualsNull()
        {
            Assert.That(() => {
                _ = Constraint().TextEquals(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        [TestCase("Apple", "^App", true)]
        [TestCase("Apple", "ple$", true)]
        [TestCase("Apple", "App$", false)]
        [TestCase("Apple", @"^(\S+)$", true)]
        [TestCase("Apple Banana", @"^(\S+)$", false)]
        public void TextRegEx(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextRegEx(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        [TestCase("Apple", "^App", true)]
        [TestCase("Apple", "ple$", true)]
        [TestCase("Apple", "App$", false)]
        [TestCase("Apple", @"^(\S+)$", true)]
        [TestCase("Apple Banana", @"^(\S+)$", false)]
        public void TextRegExOption(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextRegEx(match, RegexOptions.CultureInvariant);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void TextRegExNull()
        {
            Assert.That(() => {
                _ = Constraint().TextRegEx(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TextRegExOptionNull()
        {
            Assert.That(() => {
                _ = Constraint().TextRegEx(null, RegexOptions.CultureInvariant);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", true)]
        [TestCase("Apple", "Banana", false)]
        [TestCase("Apple", "^App", true)]
        [TestCase("Apple", "ple$", true)]
        [TestCase("Apple", "App$", false)]
        [TestCase("Apple", @"^(\S+)$", true)]
        [TestCase("Apple Banana", @"^(\S+)$", false)]
        public void TextIRegEx(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextIRegEx(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", true)]
        [TestCase("Apple", "Banana", false)]
        [TestCase("Apple", "^App", true)]
        [TestCase("Apple", "ple$", true)]
        [TestCase("Apple", "App$", false)]
        [TestCase("Apple", @"^(\S+)$", true)]
        [TestCase("Apple Banana", @"^(\S+)$", false)]
        public void TextIRegExOption(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextIRegEx(match, RegexOptions.CultureInvariant);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void TextIRegExNull()
        {
            Assert.That(() => {
                _ = Constraint().TextIRegEx(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TextIRegExOptionNull()
        {
            Assert.That(() => {
                _ = Constraint().TextIRegEx(null, RegexOptions.CultureInvariant);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", false)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        public void TextStartsWith(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().TextStartsWith(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [TestCase("Straße", "Strasse", false)]
        [TestCase("STRasse", "strasse", false)]
        [TestCase("strasse", "strasse", true)]
        public void TextStartsWith_InvariantCulture(string text, string match, bool result)
        {
            string originalCulture = Thread.CurrentThread.CurrentCulture.Name;

            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");

                TraceLine line = new TraceLine(text, 0, 0);
                Constraint c = Constraint().TextStartsWith(match);
                Assert.That(c.Check(line), Is.EqualTo(result));
            } finally {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            }
        }

        [Test]
        public void TextStartsWithNull()
        {
            Assert.That(() => {
                _ = Constraint().TextStartsWith(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("Straße", "STRasse", false)]
        [TestCase("strasse", "STRasse", true)]
        [TestCase("strasse", "strasse", true)]
        public void TextIEquals_InvariantCulture(string text, string match, bool result)
        {
            string originalCulture = Thread.CurrentThread.CurrentCulture.Name;

            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-MO");

                TraceLine line = new TraceLine(text, 0, 0);
                Constraint c = Constraint().TextIEquals(match);
                Assert.That(c.Check(line), Is.EqualTo(result));

            } finally {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            }
        }

        [Test]
        public void TextIEqualsNull()
        {
            Assert.That(() => {
                _ = Constraint().TextIEquals(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("asdasstrassEdfsdf", "Strasse", true)]
        [TestCase("strasseasdasd", "Strasse", true)]
        [TestCase("straßeasdasd", "Strasse", false)]
        public void TextIString_InvariantCulture(string text, string match, bool result)
        {
            string originalCulture = Thread.CurrentThread.CurrentCulture.Name;

            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");

                TraceLine line = new TraceLine(text, 0, 0);
                Constraint c = Constraint().TextIString(match);
                Assert.That(c.Check(line), Is.EqualTo(result));
            } finally {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(originalCulture);
            }
        }

        [Test]
        public void TextIStringNull()
        {
            Assert.That(() => {
                _ = Constraint().TextIString(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TypeOfSuccess()
        {
            LogTraceLine line = new LogTraceLine("X", 0, 0);
            Constraint c = Constraint().TypeOf<LogTraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(true));
        }

        [Test]
        public void TypeOfFail1()
        {
            TraceLine line = new TraceLine("X", 0, 0);
            Constraint c = Constraint().TypeOf<LogTraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(false));
        }

        [Test]
        public void TypeOfFail2()
        {
            LogTraceLine line = new LogTraceLine("X", 0, 0);
            Constraint c = Constraint().TypeOf<TraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(false));
        }

        [Test]
        public void TypeOfNull()
        {
            Assert.That(() => {
                _ = new TypeOf(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TypeOfWrongType()
        {
            Assert.That(() => {
                _ = Constraint().TypeOf<object>();
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InstanceOfSuccess1()
        {
            LogTraceLine line = new LogTraceLine("X", 0, 0);
            Constraint c = Constraint().InstanceOf<LogTraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(true));
        }

        [Test]
        public void InstanceOfFail()
        {
            TraceLine line = new TraceLine("X", 0, 0);
            Constraint c = Constraint().InstanceOf<LogTraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(false));
        }

        [Test]
        public void InstanceOfSuccess2()
        {
            LogTraceLine line = new LogTraceLine("X", 0, 0);
            Constraint c = Constraint().InstanceOf<TraceLine>();

            Assert.That(c.Check(line), Is.EqualTo(true));
        }

        [Test]
        public void InstanceOfNull()
        {
            Assert.That(() => {
                _ = new InstanceOf(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InstanceOfWrongType()
        {
            Assert.That(() => {
                _ = Constraint().InstanceOf<object>();
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void OrLeft()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().TextEquals("Text").Or.TextEquals("Bar");

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void OrRight()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().TextEquals("Bar").Or.TextEquals("Text");

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void OrPrecedenceLeft()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().TextString("Te").TextString("xt").Or.TextString("Foo").TextString("Bar");

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void OrPrecedenceRight()
        {
            TraceLine line = new TraceLine("FooBar", 0, 0);
            Constraint c = Constraint().TextString("Te").TextString("xt").Or.TextString("Foo").TextString("Bar");

            Assert.That(c.Check(line), Is.True);
        }

        [TestCase("Apple", "Apple", false)]
        [TestCase("Apple", "App", true)]
        [TestCase("Apple", "ple", true)]
        [TestCase("Apple", "le", true)]
        [TestCase("Apple", "", true)]
        [TestCase("Apple", "apple", true)]
        [TestCase("Apple", "Banana", true)]
        public void NotTextEquals(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().Not.TextEquals(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [TestCase("Apple", "Apple", true)]
        [TestCase("Apple", "App", false)]
        [TestCase("Apple", "ple", false)]
        [TestCase("Apple", "le", false)]
        [TestCase("Apple", "", false)]
        [TestCase("Apple", "apple", false)]
        [TestCase("Apple", "Banana", false)]
        public void NotNotTextEquals(string text, string match, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            Constraint c = Constraint().Not.Not.TextEquals(match);

            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        // Checks that "Not" only applies to the next operation, not to more than one.
        public void NotSingleToken()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().Not.TextString("Foo").TextString("xt");

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void End()
        {
            TraceLine line = new TraceLine("Apple", 0, 0);
            Constraint c = Constraint().TextString("App").TextString("ple").End();

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void EndReadOnly()
        {
            Constraint c = Constraint().TextString("a").End();
            Assert.That(() => {
                c.TextString("b");
            }, Throws.TypeOf<InvalidOperationException>());
        }

        [TestCase("Text", true)]
        [TestCase("Foo", true)]
        [TestCase("Bar", true)]
        [TestCase("FooBar", false)]
        public void ComplexExpression1(string text, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);

            // Both equivalent to de Morgan's theorem.
            Constraint c1 = Constraint().Not.TextString("Foo").Or.Not.TextString("Bar");
            Constraint c2 = Constraint().Not.Expr(Constraint().TextString("Foo").TextString("Bar"));

            Assert.That(c1.Check(line), Is.EqualTo(result));
            Assert.That(c2.Check(line), Is.EqualTo(result));
        }

        [TestCase("Text", false)]
        [TestCase("Foo", false)]
        [TestCase("Bar", false)]
        [TestCase("FooBar", false)]
        [TestCase("Hill", false)]
        [TestCase("Billies", false)]
        [TestCase("HillBillies", false)]
        [TestCase("FooBillies", true)]
        [TestCase("FooBarBillies", true)]
        [TestCase("FooHill", true)]
        [TestCase("FooBarHill", true)]
        [TestCase("BarBillies", true)]
        [TestCase("BarHill", true)]
        [TestCase("BarHillBillies", true)]
        public void ComplexExpression2(string text, bool result)
        {
            TraceLine line = new TraceLine(text, 0, 0);

            // Both equivalent to de Morgan's theorem.
            Constraint c1 = Constraint()
                .Expr(
                    Constraint().TextString("Foo").Or.TextString("Bar"))
                .Expr(
                    Constraint().TextString("Hill").Or.TextString("Billies"));
            Constraint c2 = Constraint()
                .Not.Expr(
                    Constraint().Not.TextString("Foo").Not.TextString("Bar")
                    .Or.Not.TextString("Hill").Not.TextString("Billies"));

            Assert.That(c1.Check(line), Is.EqualTo(result));
            Assert.That(c2.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void ComplexExpression3()
        {
            // ( ( a + b ) . !c) + ( d + e )
            Constraint c = Constraint()
                .Expr(Constraint()
                    .Expr(Constraint().TextString("a").Or.TextString("b"))
                    .Not.TextString("c"))
                .Or.Expr(Constraint()
                    .TextString("d").Or.TextString("e")
                );

            for (int i = 0; i < 63; i++) {
                bool result = ComplexExpression3Calc(i, out string condition);
                TraceLine line = new TraceLine(condition, 0, 0);
                Assert.That(c.Check(line), Is.EqualTo(result));
            }
        }

        private static bool ComplexExpression3Calc(int bitField, out string condition)
        {
            bool a = (bitField & 0x01) != 0x00;
            bool b = (bitField & 0x02) != 0x00;
            bool c = (bitField & 0x04) != 0x00;
            bool d = (bitField & 0x08) != 0x00;
            bool e = (bitField & 0x10) != 0x00;
            condition = string.Format("{0}{1}{2}{3}{4}",
                a ? "a" : "",
                b ? "b" : "",
                c ? "c" : "",
                d ? "d" : "",
                e ? "e" : "");
            return (a || b) && !c || (d || e);
        }

        [Test]
        public void InvalidExpressionDanglingNot()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().Not;
            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [Test]
        public void InvalidExpressionDanglingOr1()
        {
            TraceLine line = new TraceLine("Text", 0, 0);
            Constraint c = Constraint().Or;
            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [TestCase("Foo")]
        [TestCase("Bar")]
        public void InvalidExpressionDanglingOr2(string match)
        {
            TraceLine line = new TraceLine("Foo", 0, 0);
            Constraint c = Constraint().TextString(match).Or;
            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [TestCase("Foo")]
        [TestCase("Bar")]
        public void InvalidExpressionUnityOr(string match)
        {
            TraceLine line = new TraceLine("Foo", 0, 0);
            Constraint c = Constraint().Or.TextString(match);
            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [TestCase("Text")]
        [TestCase("Foo")]
        [TestCase("Bar")]
        [TestCase("FooBar")]
        public void InvalidExpressionNotOr(string text)
        {
            TraceLine line = new TraceLine(text, 0, 0);
            // Should be .Or.Not
            Constraint c = Constraint().TextString("Foo").Not.Or.TextString("Bar");

            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [Test]
        public void CustomPositionConstraintMatch()
        {
            TraceLine line = new TraceLine("Foo", 0, 0);

            // This is how you would add your own constraints to an existing Constraint object.
            Constraint c = Constraint().Expr(new Position(0));

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void CustomPositionConstraintMatchDirect()
        {
            TraceLine line = new TraceLine("Foo", 0, 0);

            // You could also use it directly if you don't need anything else.
            IMatchConstraint c = new Position(0);

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void ConstraintWithExceptionInCheck()
        {
            TraceLine line = new TraceLine("Foo", 0, 0);
            Constraint c = Constraint().Expr(new InvalidConstraint());

            Assert.That(() => { c.Check(line); }, Throws.TypeOf<ConstraintException>());
        }

        [Test]
        public void NullConstraintTrue()
        {
            Constraint c = Constraint().Null;
            Assert.That(c.Check(null), Is.True);
        }

        [Test]
        public void NullConstraintFalse()
        {
            TraceLine line = new TraceLine("Foo", 0, 0);
            Constraint c = Constraint().Null;

            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void NotNullConstraintFalse()
        {
            Constraint c = Constraint().Not.Null;
            Assert.That(c.Check(null), Is.False);
        }

        [Test]
        public void NotNullConstraintTrue()
        {
            TraceLine line = new TraceLine("Foo", 0, 0);
            Constraint c = Constraint().Not.Null;

            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void ConstraintNullTraceLine()
        {
            Constraint c = Constraint().TextString("Text");
            Assert.That(() => { c.Check(null); }, Throws.TypeOf<ConstraintException>());
        }

        [TestCase("Foo", 1, 1, 0, false)]
        [TestCase("Bar", 1, 0, 0, false)]
        [TestCase("Baz", 1, 0, 0, false)]
        [TestCase("FooBar", 1, 1, 1, true)]
        [TestCase("BarFoo", 1, 1, 1, true)]
        public void AssociativityAnd(string text, int val1, int val2, int val3, bool result)
        {
            Counter ctr1 = new Counter();
            Counter ctr2 = new Counter();
            Counter ctr3 = new Counter();
            TraceLine line = new TraceLine(text, 0, 0);

            // With "and", evaluation should stop as soon as a result is false. When evaluation has stopped, the counter
            // is not incremented.
            Constraint c = Constraint().Expr(ctr1).TextString("Foo").Expr(ctr2).TextString("Bar").Expr(ctr3);

            Assert.That(c.Check(line), Is.EqualTo(result));
            Assert.That(ctr1.Count, Is.EqualTo(val1));
            Assert.That(ctr2.Count, Is.EqualTo(val2));
            Assert.That(ctr3.Count, Is.EqualTo(val3));
        }

        [TestCase("Foo", 1, 1, 0, 0, true)]
        [TestCase("Bar", 1, 0, 1, 1, true)]
        [TestCase("Baz", 1, 0, 1, 0, false)]
        [TestCase("FooBar", 1, 1, 0, 0, true)]
        [TestCase("BarFoo", 1, 1, 0, 0, true)]
        public void AssociativityOr(string text, int val1, int val2, int val3, int val4, bool result)
        {
            Counter ctr1 = new Counter();
            Counter ctr2 = new Counter();
            Counter ctr3 = new Counter();
            Counter ctr4 = new Counter();
            TraceLine line = new TraceLine(text, 0, 0);

            // With "or", evaluation should stop as soon as a result is true. When evaluation has stopped, the counter
            // is not incremented.
            Constraint c = Constraint().Expr(ctr1).TextString("Foo").Expr(ctr2).Or.Expr(ctr3).TextString("Bar").Expr(ctr4);

            Assert.That(c.Check(line), Is.EqualTo(result));
            Assert.That(ctr1.Count, Is.EqualTo(val1));
            Assert.That(ctr2.Count, Is.EqualTo(val2));
            Assert.That(ctr3.Count, Is.EqualTo(val3));
            Assert.That(ctr4.Count, Is.EqualTo(val4));
        }

        [TestCase("Foo", 1, 0, false)]
        [TestCase("Bar", 1, 1, true)]
        public void AssociativityNot(string text, int val1, int val2, bool result)
        {
            Counter ctr1 = new Counter();
            Counter ctr2 = new Counter();
            TraceLine line = new TraceLine(text, 0, 0);

            // Note, we can't have a "ctr" after "Not", as that would invert the result of "ctr" which is always true.
            // If you want to apply "not" to the result of TextString() with the counter, you need to make it an
            // expression on its own.
            Constraint c = Constraint().Expr(ctr1).Not.TextString("Foo").Expr(ctr2);
            Assert.That(c.Check(line), Is.EqualTo(result));
            Assert.That(ctr1.Count, Is.EqualTo(val1));
            Assert.That(ctr2.Count, Is.EqualTo(val2));
        }
    }
}
