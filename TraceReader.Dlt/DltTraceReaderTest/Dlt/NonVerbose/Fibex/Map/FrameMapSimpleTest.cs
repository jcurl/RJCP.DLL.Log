namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class FrameMapSimpleTest
    {
        [Test]
        public void InitNullFrame()
        {
            Assert.That(() => {
                _ = new FrameMapSimpleAccessor(0, null);
            }, Throws.ArgumentNullException);
        }
    }

    [TestFixture(typeof(FrameMapSimpleAccessor))]
    [TestFixture(typeof(FrameMapEcuSimpleAccessor))]
    public class FrameMapSimpleTest<T> where T : IFrameMapLoader
    {
        private static IFrameMapLoader GetFrameMap()
        {
            return Activator.CreateInstance<T>();
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, "ECU1")]
        [TestCase("APP1", "CTX1", null)]
        [TestCase("APP1", "CTX1", "ECU1")]
        public void AddNullFrame(string appId, string ctxId, string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(() => {
                frameMap.TryAddFrame(0, appId, ctxId, ecuId, null);
            }, Throws.ArgumentNullException);
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, "ECU1")]
        [TestCase("APP1", "CTX1", null)]
        [TestCase("APP1", "CTX1", "ECU1")]
        public void AddMessageId(string appId, string ctxId, string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(1, appId, ctxId, ecuId, new TestFrame(1)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, ecuId).Id, Is.EqualTo(1));
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, "ECU1")]
        [TestCase("APP1", "CTX1", null)]
        [TestCase("APP1", "CTX1", "ECU1")]
        public void AddMessageIds(string appId, string ctxId, string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(1, appId, ctxId, ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(2, appId, ctxId, ecuId, new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, ecuId).Id, Is.EqualTo(1));
            Assert.That(frameMap.TryGetFrame(2, appId, ctxId, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(2, appId, ctxId, ecuId).Id, Is.EqualTo(2));
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, "ECU1")]
        [TestCase("APP1", "CTX1", null)]
        [TestCase("APP1", "CTX1", "ECU1")]
        public void AddDuplicateMessageIds(string appId, string ctxId, string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(1, appId, ctxId, ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, appId, ctxId, ecuId, new TestFrame(2)), Is.False);

            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, ecuId).Id, Is.EqualTo(1));
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, "ECU1")]
        [TestCase("APP1", "CTX1", null)]
        [TestCase("APP1", "CTX1", "ECU1")]
        public void GetUnknownId(string appId, string ctxId, string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, ecuId, out IFrame frame), Is.False);
            Assert.That(frame, Is.Null);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, appId, ctxId, ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
        }
    }
}
