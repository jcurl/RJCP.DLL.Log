namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Defines test class FrameMapEcuTest.
    /// </summary>
    /// <remarks>
    /// Default behaviour when an ECUID is provided either as NULL, or returned is in FrameMapDefaultTest. This test
    /// fixture additionally tests the fall back functionality.
    /// </remarks>
    [TestFixture]
    public class FrameMapEcuTest
    {
        [Test]
        public void AddEcuIdGetDefault()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU1", new TestFrame(1)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", "ECU1").Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", null).Id, Is.EqualTo(1));
        }

        [Test]
        public void AddEcuIdsGetDefault()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU1", new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", "ECU2", new TestFrame(2)), Is.True);

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", "ECU1", out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", "ECU1").Id, Is.EqualTo(1));

            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", "ECU2", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", "ECU2").Id, Is.EqualTo(2));

            // If no ECU-ID is given, then we return the default (first) entry.
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", null).Id, Is.EqualTo(1));
        }

        [Test]
        public void AddNoEcuIdGet()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", null, new TestFrame(0)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", null, new TestFrame(1)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);

            // Get the default entry.
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", null).Id, Is.EqualTo(0));

            // No entry with ECU1 exists for message id 0, so the ECU-ID is ignored, and the "default" is returned,
            // which is the same behaviour as the DLT-Viewer.
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", "ECU1", out _), Is.False);

            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(1));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX1", null).Id, Is.EqualTo(1));

            // A more specific entry with ECU1 exists, so this is returned instead of the default.
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU1", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX1", "ECU1").Id, Is.EqualTo(2));
        }

        [Test]
        public void AddEcuIdGetDuplicate()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(0, "APP1", "CTX1", null, new TestFrame(0)), Is.True);

            // The previous entry also adds to the default, which is why it's important that all FIBEX files contain the
            // ECU entry at the top of the file. This entry will never be returned, as it was never added.
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", null, new TestFrame(1)), Is.False);

            // Get the default entry.
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(0));
            Assert.That(frameMap.GetFrame(0, "APP1", "CTX1", null).Id, Is.EqualTo(0));

            // No entry with ECU1 exists for message id 0, so the ECU-ID is ignored, and the "default" is returned,
            // which is the same behaviour as the DLT-Viewer.
            Assert.That(frameMap.TryGetFrame(0, "APP1", "CTX1", "ECU1", out _), Is.False);
            Assert.That(() => {
                _ = frameMap.GetFrame(0, "APP1", "CTX1", "ECU1");
            }, Throws.TypeOf<KeyNotFoundException>());

            // The first entry added was for ECU1.
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", null, out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX1", null).Id, Is.EqualTo(2));

            // A more specific entry with ECU1 exists, so this is returned instead of the default.
            Assert.That(frameMap.TryGetFrame(1, "APP1", "CTX1", "ECU1", out frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(2));
            Assert.That(frameMap.GetFrame(1, "APP1", "CTX1", "ECU1").Id, Is.EqualTo(2));
        }

        [Test]
        public void MultipleEcuIdenticalFrames()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(2)), Is.True);
            Assert.That(frameMap.TryAddFrame(1, "APP1", "CTX1", "ECU1", new TestFrame(0)), Is.False);
        }

        [Test]
        public void MultipleEcuFrames()
        {
            FrameMapEcuAccessor frameMap = new FrameMapEcuAccessor();
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
