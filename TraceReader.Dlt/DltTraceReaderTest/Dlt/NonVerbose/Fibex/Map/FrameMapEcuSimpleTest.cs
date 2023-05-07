namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Defines test class FrameMapEcuTest.
    /// </summary>
    /// <remarks>
    /// When an ECU-ID is added to the frames, the order matters. The first found will be the 'default', in case we
    /// search for a frame with a `null` ECU-ID (not present in the standard header). If the ECU-ID is `null` when
    /// searching, the default is returned, else, only those in the FIBEX file with the ECU-ID are returned.
    /// </remarks>
    [TestFixture]
    public class FrameMapEcuSimpleTest
    {
        [TestCase("APP1", "CTX1")]
        [TestCase(null, null)]
        public void AddEcuIdGetDefault(string appId, string ctxId)
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU1", new TestFrame(1)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, "ECU1").Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, null).Id, Is.EqualTo(1));
        }

        [TestCase("APP1", "CTX1")]
        [TestCase(null, null)]
        public void AddEcuIdsGetDefault(string appId, string ctxId)
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU1", new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU2", new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, "ECU1").Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, "ECU2", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, "ECU2").Id, Is.EqualTo(2));

            // If no ECU-ID is given, then we return the default (first) entry.
            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, null).Id, Is.EqualTo(1));
        }

        [TestCase("APP1", "CTX1")]
        [TestCase(null, null)]
        public void AddNoEcuIdGet(string appId, string ctxId)
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", null, new TestFrame(0)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", null, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);

            // Get the default entry.
            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, null).Id, Is.EqualTo(0));

            // No entry with ECU1 exists for message id 0
            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, "ECU1", out _), Is.False);

            // Return the default (first) entry.
            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, null).Id, Is.EqualTo(1));

            // A more specific entry with ECU1 exists, so this is returned instead of the default.
            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, "ECU1", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, "ECU1").Id, Is.EqualTo(2));
        }

        [TestCase("APP1", "CTX1")]
        [TestCase(null, null)]
        public void AddEcuIdGetDuplicate(string appId, string ctxId)
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", null, new TestFrame(0)), Is.True);

            // The previous entry also adds to the default, which is why it's important that all FIBEX files contain the
            // ECU entry at the top of the file. This entry will never be returned, as it was never added.
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", null, new TestFrame(1)), Is.False);

            // Get the default entry.
            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
            Assert.That(frameMap.GetFrame(0, appId, ctxId, null).Id, Is.EqualTo(0));

            // No entry with ECU1 exists for message id 0, so a fallback to the binary decoder is done.
            Assert.That(frameMap.TryGetFrame(0, appId, ctxId, "ECU1", out _), Is.False);

            // The first entry added was for ECU1.
            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, null).Id, Is.EqualTo(2));

            // A more specific entry with ECU1 exists, so this is returned instead of the default.
            Assert.That(frameMap.TryGetFrame(1, appId, ctxId, "ECU1", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, appId, ctxId, "ECU1").Id, Is.EqualTo(2));
        }

        [Test]
        public void AddEcuIdOverlap()
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", string.Empty, new TestFrame(0)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", string.Empty, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));

            // When reading from a line that doesn't have an ECU-ID, the order how the FIBEX is loaded matters.
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
        }

        [Test]
        public void AddEcuIdOverlap2()
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", string.Empty, new TestFrame(0)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", string.Empty, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));

            // When reading from a line that doesn't have an ECU-ID, the order how the FIBEX is loaded matters.
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
        }

        [Test]
        public void NoEcuId()
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", string.Empty, new TestFrame(0)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU2", out _), Is.False);

            Assert.That(() => {
                _ = frameMap.GetFrame(1, "APP1", "CTX1", "ECU2");
            }, Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void MultipleEcuIdenticalFrames()
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(0)), Is.False);
        }

        [Test]
        public void MultipleEcuFrames()
        {
            FrameMapEcuSimpleAccessor frameMap = new FrameMapEcuSimpleAccessor();
            Assert.That(frameMap.TryAddFrame(2, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(0)), Is.True);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU2", out _), Is.False);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
            Assert.That(frameMap.TryGetFrame(2, "APP1", "CTX1", "ECU1", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
        }
    }
}
