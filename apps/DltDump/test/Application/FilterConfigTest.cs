namespace RJCP.App.DltDump.Application
{
    using System;
    using Diagnostics.Log.Dlt;
    using Domain.Dlt;
    using NUnit.Framework;
    using TestResources;

    [TestFixture]
    public class FilterConfigTest
    {
        [Test]
        public void FilterConfigEmptyList()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            Assert.That(config.Input.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterConfigSingleEntry()
        {
            FilterConfig config = new FilterConfig(new[] { "" });
            Assert.That(config.Input.Count, Is.EqualTo(1));
        }

        [Test]
        public void FilterConfigDefaultInputFormat()
        {
            FilterConfig config = new FilterConfig(new[] { "" });
            Assert.That(config.InputFormat, Is.EqualTo(InputFormat.Automatic));
        }

        [TestCase("ECU1", true, TestName = "FilterEcuIdFound")]
        [TestCase("ECU2", false, TestName = "FilterEcuIdNotFound")]
        [TestCase("ecu1", false, TestName = "FilterEcuIdNotFoundCase")]
        [TestCase("", false, TestName = "FilterEcuIdNotFoundEmpty")]
        public void FilterEcuId(string ecuid, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId(ecuid);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [Test]
        public void FilterEcuIdMultiple()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId("ECU2");
            config.AddEcuId("ECU1");

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
        }

        [TestCase("ECU1", TestName = "FilterEcuIdNotPresent")]
        [TestCase("", TestName = "FilterEcuIdNotPresentEmpty")]
        public void FilterEcuIdNotPresent(string ecuid)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId(ecuid);

            Assert.That(config.GetFilter().Check(TestLines.NoEcuId), Is.False);
        }

        [TestCase("ECU1", TestName = "FilterEcuIdNotPresentButSet")]
        [TestCase("", TestName = "FilterEcuIdNotPresentEmptyButSet")]
        public void FilterEcuIdNotPresentButSet(string ecuid)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId(ecuid);

            // The field is set, but the feature bit is not, thus it should always return false.
            Assert.That(config.GetFilter().Check(TestLines.NoEcuIdStorageHeader), Is.False);
        }

        [TestCase("APP1", true, TestName = "FilterAppIdFound")]
        [TestCase("APP2", false, TestName = "FilterAppIdNotFound")]
        [TestCase("app1", false, TestName = "FilterAppIdNotFoundCase")]
        [TestCase("", false, TestName = "FilterAppIdNotFoundEmpty")]
        public void FilterAppId(string appid, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddAppId(appid);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [Test]
        public void FilterAppIdMultiple()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddAppId("APP1");
            config.AddAppId("CTX1");

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
        }

        [TestCase("APP2", TestName = "FilterAppIdNotPresent")]
        [TestCase("", TestName = "FilterAppIdNotPresentEmpty")]
        public void FilterAppIdNotPresent(string ctxid)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddAppId(ctxid);

            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.False);
        }

        [TestCase("CTX1", true, TestName = "FilterCtxIdFound")]
        [TestCase("CTX2", false, TestName = "FilterCtxIdNotFound")]
        [TestCase("ctx1", false, TestName = "FilterCtxIdNotFoundCase")]
        [TestCase("", false, TestName = "FilterCtxIdNotFoundEmpty")]
        public void FilterCtxId(string ctxid, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddCtxId(ctxid);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [Test]
        public void FilterCtxIdMultiple()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddCtxId("CTX2");
            config.AddCtxId("CTX1");

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
        }

        [TestCase("CTX1", TestName = "FilterCtxIdNotPresent")]
        [TestCase("", TestName = "FilterCtxIdNotPresentEmpty")]
        public void FilterCtxIdNotPresent(string ctxid)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddCtxId(ctxid);

            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.False);
        }

        [TestCase(127, true, TestName = "FilterSessionIdFound")]
        [TestCase(128, false, TestName = "FilterSessionIdNotFound")]
        public void FilterSessionId(int sessiond, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddSessionId(sessiond);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [Test]
        public void FilterSessionIdMultiple()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddSessionId(128);
            config.AddSessionId(127);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
        }

        [Test]
        public void FilterSessionIdNotPresent()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddSessionId(127);

            Assert.That(config.GetFilter().Check(TestLines.NoSessionId), Is.False);
        }

        [Test]
        public void FilterVerbose()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetVerbose();

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
        }

        [Test]
        public void FilterVerboseNotSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetVerbose();

            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.False);
        }

        [Test]
        public void FilterVerboseControlSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetVerbose();

            // Regardless of the verbose bit, control messages are not accepted.
            Assert.That(config.GetFilter().Check(TestLines.ControlVerbose), Is.False);
        }

        [Test]
        public void FilterVerboseControl()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetVerbose();

            // Regardless of the verbose bit, control messages are not accepted.
            Assert.That(config.GetFilter().Check(TestLines.Control), Is.False);
        }

        [Test]
        public void FilterNonVerboseNotSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetNonVerbose();

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.False);
        }

        [Test]
        public void FilterNonVerbose()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetNonVerbose();

            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.True);
        }

        [Test]
        public void FilterNonVerboseControlSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetNonVerbose();

            // Regardless of the verbose bit, control messages are not accepted.
            Assert.That(config.GetFilter().Check(TestLines.ControlVerbose), Is.False);
        }

        [Test]
        public void FilterNonVerboseControl()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetNonVerbose();

            // Regardless of the verbose bit, control messages are not accepted.
            Assert.That(config.GetFilter().Check(TestLines.Control), Is.False);
        }

        [Test]
        public void FilterControlMessage()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetControlMessage();

            Assert.That(config.GetFilter().Check(TestLines.Control), Is.True);
        }

        [Test]
        public void FilterControlMessageVerbose()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetControlMessage();

            Assert.That(config.GetFilter().Check(TestLines.ControlVerbose), Is.True);
        }

        [Test]
        public void FilterNonControlMessage()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetControlMessage();

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.False);
        }

        [Test]
        public void FilterAllMessagesSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId("ECU1");
            config.SetControlMessage();
            config.SetVerbose();
            config.SetNonVerbose();

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.True);
            Assert.That(config.GetFilter().Check(TestLines.Control), Is.True);
        }

        [Test]
        public void FilterAllMessagesNotSet()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddEcuId("ECU1");

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.True);
            Assert.That(config.GetFilter().Check(TestLines.NoExtHdr), Is.True);
            Assert.That(config.GetFilter().Check(TestLines.Control), Is.True);
        }

        [Test]
        public void FilterAllThrough()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            Assert.That(config.GetFilter(), Is.Null);
        }

        [Test]
        public void FilterAllThroughWithVerbose()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.SetControlMessage();
            config.SetVerbose();
            config.SetNonVerbose();

            Assert.That(config.GetFilter(), Is.Null);
        }

        [TestCase("Message", false, true, TestName = "FilterStringCaseSensitivePartialMatch")]
        [TestCase("Message 1", false, true, TestName = "FilterStringCaseSensitiveFullMatch")]
        [TestCase("message", true, true, TestName = "FilterStringCaseInsensitivePartialMatch")]
        [TestCase("message 1", true, true, TestName = "FilterStringCaseInsensitiveFullMatch")]
        [TestCase("message", false, false, TestName = "FilterStringCaseSensitivePartialNoMatch")]
        [TestCase("message 1", false, false, TestName = "FilterStringCaseSensitiveFullNoMatch")]
        [TestCase("sage", false, true, TestName = "FilterStringCaseSensitivePartialMatchNotStart")]
        [TestCase("sage 1", false, true, TestName = "FilterStringCaseSensitiveEndMatchNotStart")]
        [TestCase("SAGE", true, true, TestName = "FilterStringCaseInsensitivePartialMatchNotStart")]
        [TestCase("SAGE 1", true, true, TestName = "FilterStringCaseInsensitiveEndMatchNotStart")]
        public void FilterString(string search, bool ignoreCase, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddSearchString(search, ignoreCase);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [TestCase("Mes+age", false, true, TestName = "FilterRegexCaseSensitivePartialMatch")]
        [TestCase("Message \\d", false, true, TestName = "FilterRegexCaseSensitiveFullMatch")]
        [TestCase("mes+age", true, true, TestName = "FilterRegexCaseInsensitivePartialMatch")]
        [TestCase("mes+age \\d+", true, true, TestName = "FilterRegexCaseInsensitiveFullMatch")]
        [TestCase("mes+age", false, false, TestName = "FilterRegexCaseSensitivePartialNoMatch")]
        [TestCase("mes+age \\d", false, false, TestName = "FilterRegexCaseSensitiveFullNoMatch")]
        [TestCase("s+age", false, true, TestName = "FilterRegexCaseSensitivePartialMatchNotStart")]
        [TestCase("s+age \\d", false, true, TestName = "FilterRegexCaseSensitiveEndMatchNotStart")]
        [TestCase("S+AGE", true, true, TestName = "FilterRegexCaseInsensitivePartialMatchNotStart")]
        [TestCase("S+AGE\\s+\\d", true, true, TestName = "FilterRegexCaseInsensitiveEndMatchNotStart")]
        public void FilterRegex(string search, bool ignoreCase, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddRegexString(search, ignoreCase);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [Test]
        public void FilterRegexBadString()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());

            // On .NET 5 and later, this is a RegexParseException
            Assert.That(() => {
                config.AddRegexString(@"Me(ssage", false);
            }, Throws.InstanceOf<ArgumentException>());

            Assert.That(config.GetFilter(), Is.Null);
        }

        [TestCase(DltType.LOG_INFO, true)]
        [TestCase(DltType.LOG_WARN, false)]
        public void FilterDltTypeVerboseInfo(DltType dltType, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageType(dltType);

            Assert.That(config.GetFilter().Check(TestLines.Verbose), Is.EqualTo(result));
        }

        [TestCase(DltType.CONTROL_REQUEST, false)]
        [TestCase(DltType.CONTROL_RESPONSE, true)]
        [TestCase(DltType.LOG_WARN, false)]
        public void FilterDltTypeControlResponse(DltType dltType, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageType(dltType);

            Assert.That(config.GetFilter().Check(TestLines.Control), Is.EqualTo(result));
        }

        [TestCase(42, true)]
        [TestCase(43, false)]
        [TestCase(-1, false)]
        public void FilterDltMessageId(int messageId, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageId(messageId);

            Assert.That(config.GetFilter().Check(TestLines.NonVerbose), Is.EqualTo(result));
        }

        [TestCase(42, true)]
        [TestCase(43, false)]
        [TestCase(-1, false)]
        public void FilterDltMessageIdWithArgs(int messageId, bool result)
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageId(messageId);

            Assert.That(config.GetFilter().Check(TestLines.NonVerboseWithArgs), Is.EqualTo(result));
        }

        [Test]
        public void FilterDltMessageIds()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageId(42);
            config.AddMessageId(43);

            Assert.That(config.GetFilter().Check(TestLines.NonVerbose), Is.True);
        }

        [Test]
        public void FilterDltMessageIdsNoMatch()
        {
            FilterConfig config = new FilterConfig(Array.Empty<string>());
            config.AddMessageId(41);
            config.AddMessageId(43);

            Assert.That(config.GetFilter().Check(TestLines.NonVerbose), Is.False);
        }
    }
}
