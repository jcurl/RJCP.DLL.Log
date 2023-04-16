namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class FrameMapDefaultTest
    {
        [Test]
        public void InitNullAppId()
        {
            Assert.That(() => {
                _ = new FrameMapDefaultAccessor(0, null, string.Empty, new TestFrame());
            }, Throws.ArgumentNullException);
        }

        [Test]
        public void InitNullCtxId()
        {
            Assert.That(() => {
                _ = new FrameMapDefaultAccessor(0, string.Empty, null, new TestFrame());
            }, Throws.ArgumentNullException);
        }

        [Test]
        public void InitNullFrame()
        {
            Assert.That(() => {
                _ = new FrameMapDefaultAccessor(0, string.Empty, string.Empty, null);
            }, Throws.ArgumentNullException);
        }
    }

    /// <summary>
    /// Tests the behaviour against the DLT default behaviour.
    /// </summary>
    /// <typeparam name="T">The <see cref="IFrameMapLoader"/> object to test.</typeparam>
    [TestFixture(typeof(FrameMapDefaultAccessor))]
    [TestFixture(typeof(FrameMapEcuAccessor))]
    public class FrameMapDefaultTest<T> where T : IFrameMapLoader
    {
        private static IFrameMapLoader GetFrameMap()
        {
            return Activator.CreateInstance<T>();
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void AddNullAppId(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(() => {
                frameMap.TryAddFrame(0, null, string.Empty, ecuId, new TestFrame());
            }, Throws.ArgumentNullException);
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void AddNullCtxId(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(() => {
                frameMap.TryAddFrame(0, string.Empty, null, ecuId, new TestFrame());
            }, Throws.ArgumentNullException);
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void AddNullFrame(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(() => {
                frameMap.TryAddFrame(0, string.Empty, string.Empty, ecuId, null);
            }, Throws.ArgumentNullException);
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageId(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", ecuId).Id, Is.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageIdCommonAppCtx(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", ecuId, new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX1", ecuId).Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(0, "APP2", "CTX1", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(0, "APP2", "CTX1", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX2", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(0, "APP1", "CTX2", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, "APP2", "CTX1", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP2", "CTX1", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX2", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP1", "CTX2", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageIdDistinctAppCtx(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX2", ecuId, new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX2", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX2", ecuId).Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP1", "CTX1", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX2", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(0, "APP1", "CTX2", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, "APP2", "CTX1", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP2", "CTX1", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(0, "APP2", "CTX2", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(0, "APP2", "CTX2", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageIdUniqueNoAppCtx(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX2", ecuId, new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, null, null, ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, null, ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(1, null, null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, null, null, ecuId).Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(2, null, null, ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(2, null, null, ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageIdCommonNoAppCtx(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX2", ecuId, new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX3", ecuId, new TestFrame(3)), Is.True);

            // Returns the first one that was added.
            Assert.That(frameMap.TryGetFrame(0, null, null, ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, null, ecuId).Id, Is.EqualTo(1));

            // If either App or Ctx is null, then it's the same as if both are null.
            Assert.That(frameMap.TryGetFrame(0, null, "CTX1", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX1", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, null, "CTX2", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX2", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, null, "CTX3", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX3", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP1", null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", null, ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP2", null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP2", null, ecuId).Id, Is.EqualTo(1));

            // And if the message id doesn't exist.
            Assert.That(frameMap.TryGetFrame(1, null, null, ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, null, null, ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, null, "CTX1", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, null, "CTX1", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, null, "CTX2", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, null, "CTX2", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, null, "CTX3", ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, null, "CTX3", ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, "APP1", null, ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP1", null, ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
            Assert.That(frameMap.TryGetFrame(1, "APP2", null, ecuId, out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP2", null, ecuId);
            }, Throws.TypeOf<KeyNotFoundException>());
        }

        [TestCase(null)]
        [TestCase("ECU1")]
        public void MessageIdDuplicate(string ecuId)
        {
            IFrameMapLoader frameMap = GetFrameMap();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX2", ecuId, new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX3", ecuId, new TestFrame(3)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", ecuId, new TestFrame(4)), Is.False);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", ecuId, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX2", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX2", ecuId).Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX3", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(3));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX3", ecuId).Id, Is.EqualTo(3));

            // Returns the first one that was added.
            Assert.That(frameMap.TryGetFrame(0, null, null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, null, ecuId).Id, Is.EqualTo(1));

            // If either App or Ctx is null, then it's the same as if both are null.
            Assert.That(frameMap.TryGetFrame(0, null, "CTX1", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX1", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, null, "CTX2", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX2", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, null, "CTX3", ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, null, "CTX3", ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP1", null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", null, ecuId).Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP2", null, ecuId, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP2", null, ecuId).Id, Is.EqualTo(1));
        }
    }
}
