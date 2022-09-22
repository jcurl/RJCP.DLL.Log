﻿namespace RJCP.Diagnostics.Log
{
    using System;
    using IO;
    using NUnit.Framework;

    [TestFixture]
    public class TraceReaderTest
    {
        [Test]
        public void WriteOnlyStream()
        {
            Assert.That(async () => {
                _ = await new EmptyTraceReaderFactory().CreateAsync(new WriteOnlyStream());
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
