namespace RJCP.App.DltDump.Infrastructure.Uri
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class UriExtensions_ParseQueryTest
    {
        [TestCase("udp://foo:bar@239.255.255.255:3490/path/to?bindto=192.168.1.109&timeout=500#fragment")]
        [TestCase("udp://239.255.255.255:3490/?bindto=192.168.1.109")]
        public void PrintUriParts(string uriString)
        {
            Uri uri = new(uriString);
            Console.WriteLine($"URI     Scheme: {uri.Scheme}");
            Console.WriteLine($"URI  User Info: {uri.UserInfo}");
            Console.WriteLine($"URI  Authority: {uri.Authority}");
            Console.WriteLine($"URI       Host: {uri.Host}");
            Console.WriteLine($"URI       Port: {uri.Port}");
            Console.WriteLine($"URI   Abs Path: {uri.AbsolutePath}");
            Console.WriteLine($"URI Local Path: {uri.LocalPath}");
            Console.WriteLine($"URI Path+Query: {uri.PathAndQuery}");
            Console.WriteLine($"URI      Query: {uri.Query}");
            IReadOnlyList<KeyValuePair<string, string>> kvps = uri.ParseQuery();
            foreach (KeyValuePair<string, string> kvp in kvps) {
                Console.WriteLine($"  Key {kvp.Key} = {kvp.Value}");
            }
            Console.WriteLine($"URI   Fragment: {uri.Fragment}");
            Console.WriteLine("URI Segments:");
            foreach (string segment in uri.Segments) {
                Console.WriteLine($"  Segment: {segment}");
            }
        }

        [Test]
        public void NoQuery()
        {
            Uri uri = new("http://address");
            var fields = uri.ParseQuery();
            Assert.That(fields, Is.Empty);
        }

        [Test]
        public void NoQuery2()
        {
            Uri uri = new("http://address/");
            var fields = uri.ParseQuery();
            Assert.That(fields, Is.Empty);
        }

        [Test]
        public void EmptyQuery()
        {
            Uri uri = new("http://address/?");
            var fields = uri.ParseQuery();
            Assert.That(fields, Is.Empty);
        }

        [Test]
        public void SingleKvpEntry()
        {
            Uri uri = new("http://address/?a=b");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo("a"));
            Assert.That(fields[0].Value, Is.EqualTo("b"));
        }

        [Test]
        public void SingleKvpEntryEscaped()
        {
            Uri uri = new("http://address/?%41%20%32= %42");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo("A 2"));
            Assert.That(fields[0].Value, Is.EqualTo(" B"));
        }

        [Test]
        public void TwoKvpEntries()
        {
            Uri uri = new("http://address/?a=b&c=d");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(2));
            Assert.That(fields[0].Key, Is.EqualTo("a"));
            Assert.That(fields[0].Value, Is.EqualTo("b"));
            Assert.That(fields[1].Key, Is.EqualTo("c"));
            Assert.That(fields[1].Value, Is.EqualTo("d"));
        }

        [Test]
        public void SingleKvpEntryNoKey()
        {
            Uri uri = new("http://address/?=b");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo(string.Empty));
            Assert.That(fields[0].Value, Is.EqualTo("b"));
        }

        [Test]
        public void SingleKvpEntryNoValue1()
        {
            Uri uri = new("http://address/?a=");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo("a"));
            Assert.That(fields[0].Value, Is.EqualTo(string.Empty));
        }

        [Test]
        public void SingleKvpEntryNoValue2()
        {
            Uri uri = new("http://address/?a");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo("a"));
            Assert.That(fields[0].Value, Is.EqualTo(string.Empty));
        }

        [Test]
        public void SingleKvpEntryWithQuestionMark()
        {
            Uri uri = new("http://address/?a=b?");
            var fields = uri.ParseQuery();
            Assert.That(fields, Has.Count.EqualTo(1));
            Assert.That(fields[0].Key, Is.EqualTo("a"));
            Assert.That(fields[0].Value, Is.EqualTo("b?"));
        }
    }
}
