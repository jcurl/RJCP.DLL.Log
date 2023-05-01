namespace RJCP.App.DltDump.Services
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core.Collections;
    using RJCP.Core.Collections.Specialized;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;

    [TestFixture]
    public class FibexMapTest
    {
        private static readonly string FibexDir = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "valid");
        private static readonly string FibexTcb = Path.Combine(FibexDir, "fibex-tcb.xml");
        private static readonly string FibexTcb2 = Path.Combine(FibexDir, "fibex-tcb2.xml");

        private static void WriteLog(EventLog<FibexLogEntry> log)
        {
            foreach (IEvent<FibexLogEntry> entry in log) {
                string fileName = Path.GetFileName(entry.Identifier.FileName);
                Console.WriteLine($"{entry.TimeStamp}: [{fileName}] {entry.Identifier.Message}");
            }
        }

        [Test]
        public void Default()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(map.EventLog.Count, Is.EqualTo(0));
        }

        [Test]
        public void LoadFibex()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(map.LoadFibex(FibexTcb), Is.True);
            WriteLog(map.EventLog);

            Assert.That(map.EventLog.Count, Is.EqualTo(0));

            Assert.That(map.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB", out IFrame frameEcu), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", null, out IFrame frameHdr), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", "TCB", out IFrame frameFull), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu));
            Assert.That(frame, Is.EqualTo(frameHdr));
            Assert.That(frame, Is.EqualTo(frameFull));

            Assert.That(map.GetFrame(10, null, null, null), Is.EqualTo(frame));
            Assert.That(map.GetFrame(10, null, null, "TCB"), Is.EqualTo(frameEcu));
            Assert.That(map.GetFrame(10, "TEST", "CON1", null), Is.EqualTo(frameHdr));
            Assert.That(map.GetFrame(10, "TEST", "CON1", "TCB"), Is.EqualTo(frameFull));
        }

        [Test]
        public void LoadFibexMergeNoOptions()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(map.LoadFibex(FibexTcb), Is.True);
            Assert.That(map.EventLog.Count, Is.EqualTo(0));

            Assert.That(map.LoadFibex(FibexTcb2), Is.False);
            WriteLog(map.EventLog);

            Assert.That(map.EventLog.Count, Is.Not.EqualTo(0));

            Assert.That(map.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB", out IFrame frameEcu), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", null, out IFrame frameHdr), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", "TCB", out IFrame frameFull), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu));
            Assert.That(frame, Is.EqualTo(frameHdr));
            Assert.That(frame, Is.EqualTo(frameFull));

            // Because we didn't choose WithEcuId, the search ignores the EcuID.
            Assert.That(map.TryGetFrame(10, null, null, "TCB2", out IFrame frameEcu2), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu2));

            Assert.That(map.GetFrame(10, null, null, null), Is.EqualTo(frame));
            Assert.That(map.GetFrame(10, null, null, "TCB"), Is.EqualTo(frameEcu));
            Assert.That(map.GetFrame(10, null, null, "TCB2"), Is.EqualTo(frameEcu2));
            Assert.That(map.GetFrame(10, "TEST", "CON1", null), Is.EqualTo(frameHdr));
            Assert.That(map.GetFrame(10, "TEST", "CON1", "TCB"), Is.EqualTo(frameFull));
        }

        [Test]
        public void LoadFibexMergeWithEcuId()
        {
            FibexMap map = new FibexMap(FibexOptions.WithEcuId);
            Assert.That(map.LoadFibex(FibexTcb), Is.True);
            Assert.That(map.EventLog.Count, Is.EqualTo(0));

            Assert.That(map.LoadFibex(FibexTcb2), Is.True);
            WriteLog(map.EventLog);

            Assert.That(map.EventLog.Count, Is.EqualTo(0));

            // Because TCB was loaded first, this is the default.
            Assert.That(map.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB", out IFrame frameEcu), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", null, out IFrame frameHdr), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", "TCB", out IFrame frameFull), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu));
            Assert.That(frame, Is.EqualTo(frameHdr));
            Assert.That(frame, Is.EqualTo(frameFull));

            // These are separate
            Assert.That(map.TryGetFrame(10, null, null, "TCB2", out IFrame frameEcu2), Is.True);
            Assert.That(frame, Is.Not.EqualTo(frameEcu2));

            Assert.That(map.GetFrame(10, null, null, null), Is.EqualTo(frame));
            Assert.That(map.GetFrame(10, null, null, "TCB"), Is.EqualTo(frameEcu));
            Assert.That(map.GetFrame(10, null, null, "TCB2"), Is.EqualTo(frameEcu2));
            Assert.That(map.GetFrame(10, "TEST", "CON1", null), Is.EqualTo(frameHdr));
            Assert.That(map.GetFrame(10, "TEST", "CON1", "TCB"), Is.EqualTo(frameFull));
        }

        [Test]
        public void LoadFibexMergeDirNoOptions()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(map.LoadFibex(FibexDir), Is.False);
            WriteLog(map.EventLog);

            Assert.That(map.EventLog.Count, Is.Not.EqualTo(0));

            Assert.That(map.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB", out IFrame frameEcu), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", null, out IFrame frameHdr), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", "TCB", out IFrame frameFull), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu));
            Assert.That(frame, Is.EqualTo(frameHdr));
            Assert.That(frame, Is.EqualTo(frameFull));

            // Because we didn't choose WithEcuId, the search ignores the EcuID.
            Assert.That(map.TryGetFrame(10, null, null, "TCB2", out IFrame frameEcu2), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu2));

            Assert.That(map.GetFrame(10, null, null, null), Is.EqualTo(frame));
            Assert.That(map.GetFrame(10, null, null, "TCB"), Is.EqualTo(frameEcu));
            Assert.That(map.GetFrame(10, null, null, "TCB2"), Is.EqualTo(frameEcu2));
            Assert.That(map.GetFrame(10, "TEST", "CON1", null), Is.EqualTo(frameHdr));
            Assert.That(map.GetFrame(10, "TEST", "CON1", "TCB"), Is.EqualTo(frameFull));
        }

        [Test]
        public void LoadFibexMergeDirWithEcuId()
        {
            FibexMap map = new FibexMap(FibexOptions.WithEcuId);
            Assert.That(map.LoadFibex(FibexDir), Is.True);
            WriteLog(map.EventLog);

            Assert.That(map.EventLog.Count, Is.EqualTo(0));

            // Because TCB was loaded first, this is the default.
            Assert.That(map.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB", out IFrame frameEcu), Is.True);
            Assert.That(map.TryGetFrame(10, null, null, "TCB2", out IFrame frameEcu2), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", null, out IFrame frameHdr), Is.True);
            Assert.That(map.TryGetFrame(10, "TEST", "CON1", "TCB", out IFrame frameFull), Is.True);
            Assert.That(frame, Is.EqualTo(frameEcu).Or.EqualTo(frameEcu2));
            Assert.That(frameHdr, Is.EqualTo(frameEcu).Or.EqualTo(frameEcu2));
            Assert.That(frameFull, Is.EqualTo(frameEcu));

            Assert.That(map.GetFrame(10, null, null, null), Is.EqualTo(frame));
            Assert.That(map.GetFrame(10, null, null, "TCB"), Is.EqualTo(frameEcu));
            Assert.That(map.GetFrame(10, null, null, "TCB2"), Is.EqualTo(frameEcu2));
            Assert.That(map.GetFrame(10, "TEST", "CON1", null), Is.EqualTo(frameHdr));
            Assert.That(map.GetFrame(10, "TEST", "CON1", "TCB"), Is.EqualTo(frameFull));
        }

        [Test]
        public void LoadFibexNull()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(() => {
                _ = map.LoadFibex(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void LoadFibexEmpty()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(() => {
                _ = map.LoadFibex(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void LoadFibexWhitespace()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(() => {
                _ = map.LoadFibex(" ");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void LoadFibexNotFound()
        {
            FibexMap map = new FibexMap(FibexOptions.None);
            Assert.That(() => {
                _ = map.LoadFibex(Path.Combine(FibexDir, "x"));
            }, Throws.TypeOf<FileNotFoundException>());
        }
    }
}
