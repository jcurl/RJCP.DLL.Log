namespace RJCP.App.DltDump.Infrastructure.Uri
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class UriExtensions_CommaSeparatedListTest
    {
        [Test]
        public void ParseEmptyList()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList("");
            Assert.That(list, Is.Empty);
        }

        [Test]
        public void ParseSingleEntry()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList("one");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo("one"));
        }

        [TestCase("one,two")]
        [TestCase("one, two")]
        [TestCase("one ,two")]
        [TestCase("one , two")]
        [TestCase(" one,two")]
        [TestCase("one,two ")]
        [TestCase(" one , two ")]
        public void ParseListEntry(string uriPath)
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList(uriPath);
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0], Is.EqualTo("one"));
            Assert.That(list[1], Is.EqualTo("two"));
        }

        [Test]
        public void ParseWithQuotes()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList("' one '");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(" one "));
        }

        [Test]
        public void ParseWithQuotesWithComma()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList("' one,two '");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(" one,two "));
        }

        [TestCase("' one,two ',three")]
        [TestCase("' one,two ', three")]
        [TestCase("' one,two ' ,three")]
        [TestCase("  ' one,two ' , three")]
        public void ParseWithQuotesMultiple(string uriPath)
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList(uriPath);
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(" one,two "));
            Assert.That(list[1], Is.EqualTo("three"));
        }

        [Test]
        public void ParseQuoteWithEscape()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList(@"'With \'quote\' '");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo("With 'quote' "));
        }

        [Test]
        public void ParseEscapeNonQuote()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList(@"\a");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo("a"));
        }

        [Test]
        public void ParseEscapeEscape()
        {
            IReadOnlyList<string> list = UriExtensions.ParseCommaSeparatedList(@"\\");
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(@"\"));
        }

        [Test]
        public void ParseErrorIncompleteEscape()
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList(@"ab\");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ParseErrorIncompleteQuote()
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList("'quote");
            }, Throws.TypeOf<ArgumentException>());
        }

        [TestCase("qu'ote'")]
        [TestCase("'qu'ote")]
        public void ParseErrorQuoteInMiddle(string uriPath)
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList(uriPath);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ParseErrorQuoteInMiddleNoEnd()
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList("qu'ote");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ParseErrorMultipleQuote()
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList("'quote' 'two'");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ParseErrorMixQuotes()
        {
            Assert.That(() => {
                _ = UriExtensions.ParseCommaSeparatedList("'quote \"two\"'");
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
