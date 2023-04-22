namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;
    using RJCP.Core.Xml;

    [TestFixture(FibexOptions.None)]
    [TestFixture(FibexOptions.WithEcuId)]
    [TestFixture(FibexOptions.WithoutExtHeader)]
    [TestFixture(FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader)]
    public class FibexFileTest : FibexFileTestBase
    {
        public FibexFileTest(FibexOptions fibexOptions)
        {
            m_FibexOptions = fibexOptions;
        }

        private readonly FibexOptions m_FibexOptions;

        private static void WriteDocument(XmlDocument doc)
        {
            string fileName = $"{Deploy.TestName}.xml";
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                doc.Save(file);
            }
        }

        private FibexFile CheckXml(XmlDocument fibexDoc, FibexWarnings warnings)
        {
            return CheckXml(fibexDoc, warnings, new Dictionary<int, IFrame>());
        }

        private FibexFile CheckXml(XmlDocument fibexDoc, FibexWarnings warning, Dictionary<int, IFrame> frames)
        {
            if (warning == FibexWarnings.None) {
                return CheckXml(fibexDoc, Array.Empty<FibexWarnings>(), frames);
            }
            return CheckXml(fibexDoc, new FibexWarnings[] { warning }, frames);
        }

        private FibexFile CheckXml(XmlDocument fibexDoc, IEnumerable<FibexWarnings> warnings, Dictionary<int, IFrame> frames)
        {
            using (Deploy.ScratchPad($"FibexFileTest_{m_FibexOptions}", ScratchOptions.CreateOnMissing))
            using (Stream stream = GetStream(fibexDoc)) {
                WriteDocument(fibexDoc);

                HashSet<FibexWarnings> foundWarnings = new HashSet<FibexWarnings>();
                FibexFile fibex = new FibexFile(m_FibexOptions);
                fibex.LoadErrorEvent += (s, e) => {
                    Console.WriteLine($"{e}");
                    foundWarnings.Add(e.Warning);
                };
                fibex.LoadFile(stream);

                HashSet<FibexWarnings> expectedWarnings = new HashSet<FibexWarnings>(warnings);
                Assert.That(foundWarnings, Is.EquivalentTo(expectedWarnings));

                CompareFrames(fibex, frames);
                return fibex;
            }
        }

        [Test]
        public void LoadNullFile()
        {
            FibexFile fibex = new FibexFile(m_FibexOptions);
            Assert.That(() => {
                string fileName = null;
                fibex.LoadFile(fileName);
            }, Throws.ArgumentNullException);
        }

        [Test]
        public void LoadNullStream()
        {
            FibexFile fibex = new FibexFile(m_FibexOptions);
            Assert.That(() => {
                Stream file = null;
                fibex.LoadFile(file);
            }, Throws.ArgumentNullException);
        }

        [Test]
        public void LoadFibexFile()
        {
            bool error = false;
            FibexFile fibex = new FibexFile(m_FibexOptions);
            fibex.LoadErrorEvent += (s, e) => {
                Console.WriteLine($"{e}");
                error = true;
            };
            fibex.LoadFile(FibexPath);
            Assert.That(error, Is.False);

            foreach (var frame in DefaultFrames) {
                Assert.That(fibex.GetFrame(frame.Key, null, null, null),
                    Is.EqualTo(frame.Value).Using(FrameComparer.Comparer));
            }
        }

        [Test]
        public void LoadFibexStream()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            CheckXml(fibexDoc, FibexWarnings.None, DefaultFrames);
        }

        [Test]
        public void EcuIdElementDuplicate()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            // Insert so that we have two fx:ECU elements. The second has no attributes.
            XmlElement newEcu = fibexDoc.CreateElement("fx", "ECU", nsmgr);
            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr);
            ecuNode.InsertAfter(newEcu);

            CheckXml(fibexDoc, FibexWarnings.EcusMultipleDefined, DefaultFrames);
        }

        [Test]
        public void EcuIdElementNewNode()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            // Insert so that we have two fx:ECU elements, the second with ID=TCB2.
            XmlElement newEcu = fibexDoc.CreateElement("fx", "ECU", nsmgr);
            newEcu.AppendAttribute("ID", "TCB2");
            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr);
            ecuNode.InsertAfter(newEcu);

            CheckXml(fibexDoc, FibexWarnings.EcusMultipleDefined, DefaultFrames);
        }

        [Test]
        public void EcuIdElementDuplicateId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            // Insert so that we have two fx:ECU elements with ID=TCB.
            XmlElement newEcu = fibexDoc.CreateElement("fx", "ECU", nsmgr);
            newEcu.AppendAttribute("ID", "TCB");
            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr);
            ecuNode.InsertAfter(newEcu);

            CheckXml(fibexDoc, FibexWarnings.EcusMultipleDefined, DefaultFrames);
        }

        [Test]
        public void EcuIdElementAfterFrames()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            // Move the fx:ECUS after fx:FRAMES so that we haven't parsed the ECUs yet.
            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS", nsmgr);
            ecuNode.RemoveElement();
            XmlElement frmNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES", nsmgr);
            frmNode.InsertAfter(ecuNode);

            CheckXml(fibexDoc, FibexWarnings.EcuIdMustBeBeforeFrames, DefaultFramesNoEcu);
        }

        [Test]
        public void EcuIdElementMissingId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr);
            ecuNode.RemoveAttribute("ID");

            CheckXml(fibexDoc, FibexWarnings.EcuIdMissing, DefaultFramesNoEcu);
        }

        [Test]
        public void EcuIdElementEmptyId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement ecuNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:ECUS/fx:ECU[@ID='TCB']", nsmgr);
            ecuNode.Attributes["ID"].Value = string.Empty;

            CheckXml(fibexDoc, FibexWarnings.EcuIdMissing, DefaultFramesNoEcu);
        }

        [Test]
        public void PduElementMissingId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_10_0']", nsmgr);
            pduNode.RemoveAttribute("ID");

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {11, Frame11 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };

            FibexFile fibex = CheckXml(fibexDoc, new[] { FibexWarnings.PduIdMissing, FibexWarnings.FramePduRefIdUnknown }, frames);
            Assert.That(fibex.TryGetFrame(10, null, null, null, out _), Is.False);
        }

        [Test]
        public void PduElementEmptyId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_10_0']", nsmgr);
            pduNode.Attributes["ID"].Value = string.Empty;

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {11, Frame11 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };

            FibexFile fibex = CheckXml(fibexDoc, new[] { FibexWarnings.PduIdMissing, FibexWarnings.FramePduRefIdUnknown }, frames);
            Assert.That(fibex.TryGetFrame(10, null, null, null, out _), Is.False);
        }

        [Test]
        public void PduElementWhitespaceId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_10_0']", nsmgr);
            pduNode.Attributes["ID"].Value = " ";

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {11, Frame11 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };

            FibexFile fibex = CheckXml(fibexDoc, new[] { FibexWarnings.PduIdMissing, FibexWarnings.FramePduRefIdUnknown }, frames);
            Assert.That(fibex.TryGetFrame(10, null, null, null, out _), Is.False);
        }

        [Test]
        public void PduElementDuplicateId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_0']", nsmgr);
            XmlElement newNode = (XmlElement)pduNode.CloneNode(true);
            newNode.ChildNodes[1].InnerText = "Second description";
            pduNode.InsertAfter(newNode);

            CheckXml(fibexDoc, FibexWarnings.PduIdDuplicate, DefaultFrames);
        }

        [Test]
        public void PduElementByteLengthWithWhitespace()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:BYTE-LENGTH", nsmgr);
            pduNode.InnerText = " 2 ";

            CheckXml(fibexDoc, FibexWarnings.None, DefaultFrames);
        }

        [Test]
        public void PduElementByteLengthEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:BYTE-LENGTH", nsmgr);
            pduNode.InnerText = "";

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.PduInvalidByteLength, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduLength, Is.EqualTo(0));
        }

        [Test]
        public void PduElementByteLengthMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:BYTE-LENGTH", nsmgr);
            pduNode.RemoveElement();

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduLength, Is.EqualTo(0));
        }

        [Test]
        public void PduElementByteLengthNegative()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:BYTE-LENGTH", nsmgr);
            pduNode.InnerText = "-1";

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.PduInvalidByteLength, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduLength, Is.EqualTo(0));
        }

        [TestCase("0x10", TestName = "PduElementByteLengthHexValue_Hex10")]
        [TestCase("0x1F", TestName = "PduElementByteLengthHexValue_Hex1F")]
        [TestCase("1F", TestName = "PduElementByteLengthHexValue_1F")]
        public void PduElementByteLengthHexValue(string hex)
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:BYTE-LENGTH", nsmgr);
            pduNode.InnerText = hex;

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.PduInvalidByteLength, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduLength, Is.EqualTo(0));
        }

        [Test]
        public void PduElementDescriptionEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pduNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_10_0']/ho:DESC", nsmgr);
            pduNode.InnerText = string.Empty;

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {11, Frame11 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(10, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[0].Description, Is.Empty);
        }

        [Test]
        public void PduElementSigRefMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES/fx:SIGNAL-INSTANCE/fx:SIGNAL-REF", nsmgr);
            sigNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.MissingSignalIdRef, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduType, Is.Null);
        }

        [Test]
        public void PduElementSigRefAttrMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES/fx:SIGNAL-INSTANCE/fx:SIGNAL-REF", nsmgr);
            sigNode.RemoveAttribute("ID-REF");
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.MissingSignalIdRef, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduType, Is.Empty);
        }

        [Test]
        public void PduElementSigRefIdRefEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES/fx:SIGNAL-INSTANCE/fx:SIGNAL-REF", nsmgr);
            sigNode.Attributes["ID-REF"].Value = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.MissingSignalIdRef, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.Arguments[1].PduType, Is.Empty);
        }

        [Test]
        public void PduElementSigRefIdDuplicateSignalRef()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES/fx:SIGNAL-INSTANCE/fx:SIGNAL-REF", nsmgr);
            XmlElement newNode = (XmlElement)sigNode.CloneNode(true);
            sigNode.InsertAfter(newNode);
            CheckXml(fibexDoc, FibexWarnings.MultipleSignalsInPduDefined, DefaultFrames);
        }

        [Test]
        public void PduElementSigRefIdDuplicateSignalInstance()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES/fx:SIGNAL-INSTANCE", nsmgr);
            XmlElement newNode = (XmlElement)sigNode.CloneNode(true);
            sigNode.InsertAfter(newNode);
            CheckXml(fibexDoc, FibexWarnings.MultipleSignalsInPduDefined, DefaultFrames);
        }

        [Test]
        public void PduElementSigRefIdDuplicateSignalInstances()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement sigNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:PDUS/fx:PDU[@ID='PDU_11_1']/fx:SIGNAL-INSTANCES", nsmgr);
            XmlElement newNode = (XmlElement)sigNode.CloneNode(true);
            sigNode.InsertAfter(newNode);
            CheckXml(fibexDoc, FibexWarnings.MultipleSignalsInPduDefined, DefaultFrames);
        }

        [Test]
        public void PduElementAfterFrames()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement pdusNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:PDUS", nsmgr);
            pdusNode.RemoveElement();
            XmlElement framesNode = (XmlElement)fibexDoc.SelectSingleNode("fx:FIBEX/fx:ELEMENTS/fx:FRAMES", nsmgr);
            framesNode.InsertAfter(pdusNode);

            CheckXml(fibexDoc, FibexWarnings.FramePduRefIdUnknown);
        }

        [Test]
        public void FrameElementMissingId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            frameNode.RemoveAttribute("ID");
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementEmptyId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            frameNode.Attributes["ID"].Value = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [TestCase("ID1", TestName = "FrameElementInvalidId_ID1")]
        [TestCase("ID11", TestName = "FrameElementInvalidId_ID11")]
        [TestCase("XX_11", TestName = "FrameElementInvalidId_XX_11")]
        [TestCase("ID_-11", TestName = "FrameElementInvalidId_ID_-11")]
        [TestCase("ID_C000", TestName = "FrameElementInvalidId_ID_C000")]
        [TestCase("ID_99999999999999999999999", TestName = "FrameElementInvalidId_ID_99999999999999999999999")]
        public void FrameElementInvalidId(string id)
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            frameNode.Attributes["ID"].Value = id;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameIdInvalid, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementDuplicateId()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            frameNode.Attributes["ID"].Value = "ID_10";
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameIdDuplicate, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, new[] {
                FibexWarnings.FrameMessageInfoMissing, FibexWarnings.FrameMessageTypeMissing,
                FibexWarnings.FrameApplicationIdMissing, FibexWarnings.FrameContextIdMissing
            }, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION", nsmgr);
            frameNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, new[] {
                FibexWarnings.FrameMessageInfoMissing, FibexWarnings.FrameMessageTypeMissing,
                FibexWarnings.FrameApplicationIdMissing, FibexWarnings.FrameContextIdMissing
            }, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, new[] {
                FibexWarnings.FrameMessageInfoMissing, FibexWarnings.FrameMessageTypeMissing,
                FibexWarnings.FrameApplicationIdMissing, FibexWarnings.FrameContextIdMissing
            }, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerAppIdMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/APPLICATION_ID", nsmgr);
            frameNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameApplicationIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerAppIdEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/APPLICATION_ID", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.ApplicationId, Is.Empty);
        }

        [Test]
        public void FrameElementManufacturerContextIdMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/CONTEXT_ID", nsmgr);
            frameNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameContextIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerContextIdEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/CONTEXT_ID", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.ContextId, Is.Empty);
        }

        [Test]
        public void FrameElementManufacturerMessageTypeMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_TYPE", nsmgr);
            frameNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameMessageTypeMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerMessageTypeEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_TYPE", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameMessageTypeInvalid, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.MessageType, Is.EqualTo(DltType.UNKNOWN));
        }

        [Test]
        public void FrameElementManufacturerMessageInfoMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_INFO", nsmgr);
            frameNode.RemoveElement();
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameMessageInfoMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementManufacturerMessageInfoEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement frameNode = (XmlElement)fibexDoc.SelectSingleNode("/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_INFO", nsmgr);
            frameNode.InnerXml = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.MessageType, Is.EqualTo((DltType)0));
        }

        [Test]
        public void FrameElementPduRefIdMissing()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement refNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:PDU-INSTANCES/fx:PDU-INSTANCE/fx:PDU-REF[@ID-REF='PDU_11_0']", nsmgr);
            refNode.RemoveAttribute("ID-REF");
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FramePduRefIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementPduRefIdEmpty()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement refNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:PDU-INSTANCES/fx:PDU-INSTANCE/fx:PDU-REF[@ID-REF='PDU_11_0']", nsmgr);
            refNode.Attributes["ID-REF"].Value = string.Empty;
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FramePduRefIdMissing, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [Test]
        public void FrameElementPduRefIdUnknown()
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement refNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:PDU-INSTANCES/fx:PDU-INSTANCE/fx:PDU-REF[@ID-REF='PDU_11_0']", nsmgr);
            refNode.Attributes["ID-REF"].Value = "X";
            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FramePduRefIdUnknown, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out _), Is.False);
        }

        [TestCase("DLT_TYPE_LOG", "", (DltType)0, TestName = "FrameDltType_DLT_TYPE_LOG")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_FATAL", DltType.LOG_FATAL, TestName = "FrameDltType_DLT_LOG_FATAL")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_ERROR", DltType.LOG_ERROR, TestName = "FrameDltType_DLT_LOG_ERROR")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_WARN", DltType.LOG_WARN, TestName = "FrameDltType_DLT_LOG_WARN")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_INFO", DltType.LOG_INFO, TestName = "FrameDltType_DLT_LOG_INFO")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_DEBUG", DltType.LOG_DEBUG, TestName = "FrameDltType_DLT_LOG_DEBUG")]
        [TestCase("DLT_TYPE_LOG", "DLT_LOG_VERBOSE", DltType.LOG_VERBOSE, TestName = "FrameDltType_DLT_LOG_VERBOSE")]
        [TestCase("DLT_TYPE_APP_TRACE", "", (DltType)2, TestName = "FrameDltType_DLT_TYPE_APP_TRACE")]
        [TestCase("DLT_TYPE_APP_TRACE", "DLT_TRACE_VARIABLE", DltType.APP_TRACE_VARIABLE, TestName = "FrameDltType_DLT_TRACE_VARIABLE")]
        [TestCase("DLT_TYPE_APP_TRACE", "DLT_TRACE_FUNCTION_IN", DltType.APP_TRACE_FUNCTION_IN, TestName = "FrameDltType_DLT_TRACE_FUNCTION_IN")]
        [TestCase("DLT_TYPE_APP_TRACE", "DLT_TRACE_FUNCTION_OUT", DltType.APP_TRACE_FUNCTION_OUT, TestName = "FrameDltType_DLT_TRACE_FUNCTION_OUT")]
        [TestCase("DLT_TYPE_APP_TRACE", "DLT_TRACE_STATE", DltType.APP_TRACE_STATE, TestName = "FrameDltType_DLT_TRACE_STATE")]
        [TestCase("DLT_TYPE_APP_TRACE", "DLT_TRACE_VFB", DltType.APP_TRACE_VFB, TestName = "FrameDltType_DLT_TRACE_VFB")]
        [TestCase("DLT_TYPE_NW_TRACE", "", (DltType)4, TestName = "FrameDltType_DLT_TYPE_NW_TRACE")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_IPC", DltType.NW_TRACE_IPC, TestName = "FrameDltType_DLT_NW_TRACE_IPC")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_CAN", DltType.NW_TRACE_CAN, TestName = "FrameDltType_DLT_NW_TRACE_CAN")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_FLEXRAY", DltType.NW_TRACE_FLEXRAY, TestName = "FrameDltTypeDLT_NW_TRACE_FLEXRAY")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_MOST", DltType.NW_TRACE_MOST, TestName = "FrameDltType_DLT_NW_TRACE_MOST")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_ETHERNET", DltType.NW_TRACE_ETHERNET, TestName = "FrameDltType_DLT_NW_TRACE_ETHERNET")]
        [TestCase("DLT_TYPE_NW_TRACE", "DLT_NW_TRACE_SOMEIP", DltType.NW_TRACE_SOMEIP, TestName = "FrameDltType_DLT_NW_TRACE_SOMEIP")]
        public void FrameDltType(string dltTypeStr, string dltInfoStr, DltType dltType)
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement typeNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_TYPE", nsmgr);
            typeNode.InnerText = dltTypeStr;

            XmlElement infoNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_INFO", nsmgr);
            infoNode.InnerText = dltInfoStr;

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.None, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.MessageType, Is.EqualTo(dltType));
        }

        [TestCase("DLT_TYPE_CONTROL", "DLT_CONTROL_REQUEST", TestName = "FrameDltTypeInvalid_DLT_CONTROL_REQUEST")]
        [TestCase("DLT_TYPE_CONTROL", "DLT_CONTROL_RESPONSE", TestName = "FrameDltTypeInvalid_DLT_CONTROL_RESPONSE")]
        [TestCase("DLT_TYPE_CONTROL", "DLT_CONTROL_TIME", TestName = "FrameDltTypeInvalid_DLT_CONTROL_TIME")]
        public void FrameDltTypeInvalid(string dltTypeStr, string dltInfoStr)
        {
            XmlDocument fibexDoc = GetDocument(FibexPath);
            XmlNamespaceManager nsmgr = GetNsMgr(fibexDoc);

            XmlElement typeNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_TYPE", nsmgr);
            typeNode.InnerText = dltTypeStr;

            XmlElement infoNode = (XmlElement)fibexDoc.SelectSingleNode(
                "/fx:FIBEX/fx:ELEMENTS/fx:FRAMES/fx:FRAME[@ID='ID_11']/fx:MANUFACTURER-EXTENSION/MESSAGE_INFO", nsmgr);
            infoNode.InnerText = dltInfoStr;

            Dictionary<int, IFrame> frames = new Dictionary<int, IFrame>() {
                {10, Frame10 },
                {12, Frame12 },
                {13, Frame13 },
                {14, Frame14 }
            };
            FibexFile fibex = CheckXml(fibexDoc, FibexWarnings.FrameMessageTypeInvalid, frames);
            Assert.That(fibex.TryGetFrame(11, null, null, null, out IFrame frame), Is.True);
            Assert.That(frame.MessageType, Is.EqualTo(DltType.UNKNOWN));
        }
    }
}
