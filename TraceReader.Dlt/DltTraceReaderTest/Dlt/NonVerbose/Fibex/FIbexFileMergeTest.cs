namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using NUnit.Framework;
    using RJCP.Core.Xml;

    [TestFixture]
    public class FIbexFileMergeTest : FibexFileTestBase
    {
        [Test]
        public void LoadFibexInvalidOption()
        {
            Assert.That(() => {
                _ = new FibexFile((FibexOptions)100);
            }, Throws.TypeOf<ArgumentException>());
        }

        [TestCase(FibexOptions.None)]
        [TestCase(FibexOptions.WithEcuId)]
        [TestCase(FibexOptions.WithoutExtHeader)]
        [TestCase(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
        public void LoadFibexTwice(FibexOptions option)
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            using (Stream stream = GetStream(fibexDoc)) {
                HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
                FibexFile fibex = new FibexFile(option);
                fibex.LoadErrorEvent += (s, e) => {
                    Console.WriteLine($"{e}");
                    foundWarnings.Add(e.Warning);
                };
                fibex.LoadFile(stream);

                stream.Seek(0, SeekOrigin.Begin);
                fibex.LoadFile(stream);

                // No matter the mode, we are loading duplicate entries.
                Assert.That(foundWarnings.Count, Is.EqualTo(1));
                Assert.That(foundWarnings, Does.Contain(FibexWarnings.DuplicateEntry));
            }
        }

        private static XmlDocument GetDocument_Frame10(string fileName)
        {
            XmlDocument fibexDoc = GetDocument(fileName);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);
            XmlElement node;
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_0']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']", nsmgr);
            node.RemoveElement();
            return fibexDoc;
        }

        private static XmlDocument GetDocument_Frame11(string fileName)
        {
            XmlDocument fibexDoc = GetDocument(fileName);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);
            XmlElement node;
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_10']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_12']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_13']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_14']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_10_0']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_12_0']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_12_1']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_12_2']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_12_3']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_13_0']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_13_1']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_13_2']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_14_0']", nsmgr);
            node.RemoveElement();
            node = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_14_1']", nsmgr);
            node.RemoveElement();
            return fibexDoc;
        }

        [TestCase(FibexOptions.None)]
        [TestCase(FibexOptions.WithEcuId)]
        [TestCase(FibexOptions.WithoutExtHeader)]
        [TestCase(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
        public void LoadFibexMergeNoOverlap(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument_Frame10(FibexPath);
            XmlDocument fibexDoc2 = GetDocument_Frame11(FibexPath);

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);
                Assert.That(foundWarnings.Count, Is.EqualTo(0));
            }
            CompareFrames(fibex, DefaultFrames);
        }

        [TestCase(FibexOptions.WithEcuId)]
        [TestCase(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
        public void LoadFibexTwiceDifferentEcu(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            XmlDocument fibexDoc2 = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr2);
            node.Attributes["ID"].Value = "TCB2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);
                Assert.That(foundWarnings.Count, Is.EqualTo(0));
            }
            CompareFrames(fibex, "TCB", DefaultFrames);
            CompareFrames(fibex, "TCB2", DefaultFramesTCB2);
        }

        [TestCase(FibexOptions.None)]
        [TestCase(FibexOptions.WithoutExtHeader)]
        public void LoadFibexTwiceDifferentEcu_Duplicate(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            XmlDocument fibexDoc2 = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr2);
            node.Attributes["ID"].Value = "TCB2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);
                Assert.That(foundWarnings.Count, Is.EqualTo(1));
                Assert.That(foundWarnings, Does.Contain(FibexWarnings.DuplicateEntry));
            }
        }

        [TestCase(FibexOptions.None)]
        [TestCase(FibexOptions.WithEcuId)]
        public void LoadFibexDuplicateMessageIdDifferentApp(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            // Frame 11 with new Application ID
            XmlDocument fibexDoc2 = GetDocument_Frame11(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/APPLICATION_ID", nsmgr2);
            node.InnerText = "APP2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);
                Assert.That(foundWarnings.Count, Is.EqualTo(0));
            }
            Assert.That(fibex.GetFrame(11, null, null, null),
                Is.EqualTo(Frame11).Using(FrameComparer.Comparer));

            // Getting the default gave APP1, because this was the first one loaded.
            Assert.That(fibex.TryGetFrame(11, "APP2", "CON1", null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(11));
            Assert.That(frame.ApplicationId, Is.EqualTo("APP2"));
            Assert.That(frame.ContextId, Is.EqualTo("CON1"));

            Assert.That(fibex.TryGetFrame(11, "APP2", "CON2", null, out _), Is.False);
        }

        [TestCase(FibexOptions.WithoutExtHeader)]
        [TestCase(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
        public void LoadFibexDuplicateMessageIdDifferentApp_Duplicate(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            // Frame 11 with new Application ID
            XmlDocument fibexDoc2 = GetDocument_Frame11(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/APPLICATION_ID", nsmgr2);
            node.InnerText = "APP2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);

                Assert.That(foundWarnings.Count, Is.EqualTo(1));
                Assert.That(foundWarnings, Does.Contain(FibexWarnings.DuplicateEntry));
            }
        }

        [TestCase(FibexOptions.None)]
        [TestCase(FibexOptions.WithEcuId)]
        public void LoadFibexDuplicateMessageIdDifferentContext(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            // Frame 11 with new Application ID
            XmlDocument fibexDoc2 = GetDocument_Frame11(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/CONTEXT_ID", nsmgr2);
            node.InnerText = "CTX2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);
                Assert.That(foundWarnings.Count, Is.EqualTo(0));
            }
            Assert.That(fibex.GetFrame(11, null, null, null),
                Is.EqualTo(Frame11).Using(FrameComparer.Comparer));

            // Getting the default gave APP1, because this was the first one loaded.
            Assert.That(fibex.TryGetFrame(11, "APP1", "CTX2", null, out IFrame frame), Is.True);
            Assert.That(frame.Id, Is.EqualTo(11));
            Assert.That(frame.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(frame.ContextId, Is.EqualTo("CTX2"));

            Assert.That(fibex.TryGetFrame(11, "APP2", "CTX2", null, out _), Is.False);
        }

        [TestCase(FibexOptions.WithoutExtHeader)]
        [TestCase(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
        public void LoadFibexDuplicateMessageIdDifferentContext_Duplicate(FibexOptions option)
        {
            XmlDocument fibexDoc1 = GetDocument(FibexPath);
            // Frame 11 with new Application ID
            XmlDocument fibexDoc2 = GetDocument_Frame11(FibexPath);
            XmlNamespaceManager nsmgr2 = GetNsMgr(fibexDoc2);
            XmlElement node = (XmlElement)fibexDoc2.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/CONTEXT_ID", nsmgr2);
            node.InnerText = "CTX2";

            FibexFile fibex = new FibexFile(option);
            HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                foundWarnings.Add(e.Warning);
            };

            using (Stream stream1 = GetStream(fibexDoc1))
            using (Stream stream2 = GetStream(fibexDoc2)) {
                fibex.LoadFile(stream1);
                fibex.LoadFile(stream2);

                Assert.That(foundWarnings.Count, Is.EqualTo(1));
                Assert.That(foundWarnings, Does.Contain(FibexWarnings.DuplicateEntry));
            }
        }
    }
}
