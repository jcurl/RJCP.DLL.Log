namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    public class FibexFileTestBase
    {
        protected FibexFileTestBase() { }

        protected readonly static string FibexPath = Path.Combine(Deploy.TestDirectory, "TestResources", "Fibex", "dlt-viewer-example.xml");

        #region Frames in the FIBEX file
        protected static readonly TestFrame Frame10 = new TestFrame(10) {
            EcuId = "TCB",
            ApplicationId = "TEST",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("DLT non verbose test message.");

        protected static readonly TestFrame Frame11 = new TestFrame(11) {
            EcuId = "TCB",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_WARN
        }
        .AddArgument("Buffer near limit. Free size:")
        .AddArgument("S_UINT16", 2);

        protected static readonly TestFrame Frame12 = new TestFrame(12) {
            EcuId = "TCB",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_ERROR
        }
        .AddArgument("Buffer size exceeded.")
        .AddArgument("S_UINT32", 4)
        .AddArgument("S_UINT32", 4)
        .AddArgument("Process terminated.");

        protected static readonly TestFrame Frame13 = new TestFrame(13) {
            EcuId = "TCB",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Temperature measurement")
        .AddArgument("S_UINT8", 1)
        .AddArgument("S_FLOA32", 4);

        protected static readonly TestFrame Frame14 = new TestFrame(14) {
            EcuId = "TCB",
            ApplicationId = "LAT",
            ContextId = "NV",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Build ID:")
        .AddArgument("S_STRG_ASCII", 0);

        protected static readonly Dictionary<int, IFrame> DefaultFrames = new Dictionary<int, IFrame>() {
            {10, Frame10 },
            {11, Frame11 },
            {12, Frame12 },
            {13, Frame13 },
            {14, Frame14 }
        };

        protected static readonly TestFrame Frame10NoEcu = new TestFrame(10) {
            ApplicationId = "TEST",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("DLT non verbose test message.");

        protected static readonly TestFrame Frame11NoEcu = new TestFrame(11) {
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_WARN
        }
        .AddArgument("Buffer near limit. Free size:")
        .AddArgument("S_UINT16", 2);

        protected static readonly TestFrame Frame12NoEcu = new TestFrame(12) {
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_ERROR
        }
        .AddArgument("Buffer size exceeded.")
        .AddArgument("S_UINT32", 4)
        .AddArgument("S_UINT32", 4)
        .AddArgument("Process terminated.");

        protected static readonly TestFrame Frame13NoEcu = new TestFrame(13) {
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Temperature measurement")
        .AddArgument("S_UINT8", 1)
        .AddArgument("S_FLOA32", 4);

        protected static readonly TestFrame Frame14NoEcu = new TestFrame(14) {
            ApplicationId = "LAT",
            ContextId = "NV",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Build ID:")
        .AddArgument("S_STRG_ASCII", 0);

        protected static readonly Dictionary<int, IFrame> DefaultFramesNoEcu = new Dictionary<int, IFrame>() {
            {10, Frame10NoEcu },
            {11, Frame11NoEcu },
            {12, Frame12NoEcu },
            {13, Frame13NoEcu },
            {14, Frame14NoEcu }
        };

        protected static readonly TestFrame Frame10TCB2 = new TestFrame(10) {
            EcuId = "TCB2",
            ApplicationId = "TEST",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("DLT non verbose test message.");

        protected static readonly TestFrame Frame11TCB2 = new TestFrame(11) {
            EcuId = "TCB2",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_WARN
        }
        .AddArgument("Buffer near limit. Free size:")
        .AddArgument("S_UINT16", 2);

        protected static readonly TestFrame Frame12TCB2 = new TestFrame(12) {
            EcuId = "TCB2",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_ERROR
        }
        .AddArgument("Buffer size exceeded.")
        .AddArgument("S_UINT32", 4)
        .AddArgument("S_UINT32", 4)
        .AddArgument("Process terminated.");

        protected static readonly TestFrame Frame13TCB2 = new TestFrame(13) {
            EcuId = "TCB2",
            ApplicationId = "APP1",
            ContextId = "CON1",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Temperature measurement")
        .AddArgument("S_UINT8", 1)
        .AddArgument("S_FLOA32", 4);

        protected static readonly TestFrame Frame14TCB2 = new TestFrame(14) {
            EcuId = "TCB2",
            ApplicationId = "LAT",
            ContextId = "NV",
            MessageType = DltType.LOG_INFO
        }
        .AddArgument("Build ID:")
        .AddArgument("S_STRG_ASCII", 0);

        protected static readonly Dictionary<int, IFrame> DefaultFramesTCB2 = new Dictionary<int, IFrame>() {
            {10, Frame10TCB2 },
            {11, Frame11TCB2 },
            {12, Frame12TCB2 },
            {13, Frame13TCB2 },
            {14, Frame14TCB2 }
        };
        #endregion

        protected static XmlDocument GetDocument(string fileName)
        {
            XmlDocument fibexDoc = new XmlDocument();
            fibexDoc.Load(fileName);
            return fibexDoc;
        }

        protected static XmlNamespaceManager GetNsMgr(XmlDocument doc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("", "");
            nsmgr.AddNamespace("fx", "http://www.asam.net/xml/fbx");
            nsmgr.AddNamespace("ho", "http://www.asam.net/xml");
            return nsmgr;
        }

        protected static Stream GetStream(XmlDocument doc)
        {
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            return stream;
        }

        protected static void CompareFrames(IFrameMap fibex, Dictionary<int, IFrame> frames)
        {
            foreach (var frame in frames) {
                Assert.That(fibex.GetFrame(frame.Key, null, null, null),
                    Is.EqualTo(frame.Value).Using(FrameComparer.Comparer));
            }
        }

        protected static void CompareFrames(IFrameMap fibex, string ecu, Dictionary<int, IFrame> frames)
        {
            foreach (var frame in frames) {
                Assert.That(fibex.GetFrame(frame.Key, null, null, ecu),
                    Is.EqualTo(frame.Value).Using(FrameComparer.Comparer));
            }
        }
    }
}
