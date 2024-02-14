namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using Map;
    using RJCP.Core.Xml;

    /// <summary>
    /// Loads a FIBEX file as described by the AutoSAR DLT standard 4.3.0.
    /// </summary>
    public class FibexFile : IFrameMap
    {
        private static readonly Dictionary<string, string> m_XmlNs = new() {
            ["ho"] = "http://www.asam.net/xml",
            ["fx"] = "http://www.asam.net/xml/fbx",
            [""] = ""
        };

        private readonly IFrameMapLoader m_FrameMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexFile"/> class.
        /// </summary>
        /// <param name="options">The options which determines how to load the FIBEX files.</param>
        public FibexFile(FibexOptions options)
        {
            switch (options) {
            case FibexOptions.None:
                m_FrameMap = new FrameMapDefault();
                break;
            case FibexOptions.WithEcuId:
                m_FrameMap = new FrameMapEcu();
                break;
            case FibexOptions.WithoutExtHeader:
                m_FrameMap = new FrameMapSimple();
                break;
            case FibexOptions.WithEcuId | FibexOptions.WithoutExtHeader:
                m_FrameMap = new FrameMapEcuSimple();
                break;
            default:
                throw new ArgumentException("Unknown FIBEX option", nameof(options));
            }
        }

        /// <summary>
        /// Occurs when an error is detected loading or merging the FIBEX file.
        /// </summary>
        public event EventHandler<FibexLoadErrorEventArgs> LoadErrorEvent;

        /// <summary>
        /// Handles the <see cref="LoadErrorEvent"/> event.
        /// </summary>
        /// <param name="args">The <see cref="FibexLoadErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoadErrorEvent(FibexLoadErrorEventArgs args)
        {
            EventHandler<FibexLoadErrorEventArgs> handler = LoadErrorEvent;
            if (handler is not null) handler(this, args);
        }

        /// <summary>
        /// Loads the file and merges it into the current in-memory representation.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="IOException">An I/O error, such as a disk error, occurred.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is not permitted by the Operating System.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="fileName"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        /// non-NTFS environment.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The file <paramref name="fileName"/> specified by path does not exist.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// </exception>
        public void LoadFile(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            using (FileStream file = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                LoadFile(file);
            }
        }

        /// <summary>
        /// Loads the stream and merges it into the current in-memory representation.
        /// </summary>
        /// <param name="stream">The stream to read as XML and merge.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        public void LoadFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            LoadFileInternal(stream);
        }

        /// <summary>
        /// Gets the frame from the message, app and context identifier.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">
        /// The application identifier. May be <see langword="null"/> if the payload doesn't contain an application
        /// identifier.
        /// </param>
        /// <param name="ctxId">
        /// The context identifier. May be <see langword="null"/> if the payload doesn't contain a context identifier.
        /// </param>
        /// <param name="ecuId">
        /// The ECU identifier. May be <see langword="null"/> if the payload doesn't contain an ECU identifier.
        /// </param>
        /// <returns>
        /// Returns a frame implementing <see cref="IFrame"/>. If no suitable frame can be found, an exception is
        /// raised.
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <remarks>
        /// Searches for the most appropriate frame given the ECU identifier, Application and Context identifier, and
        /// the Message identifier.
        /// </remarks>
        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            return m_FrameMap.GetFrame(id, appId, ctxId, ecuId);
        }

        /// <summary>
        /// Tries to get the frame from the message, app and context identifier.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">
        /// The application identifier. May be <see langword="null"/> if the payload doesn't contain an application
        /// identifier.
        /// </param>
        /// <param name="ctxId">
        /// The context identifier. May be <see langword="null"/> if the payload doesn't contain a context identifier.
        /// </param>
        /// <param name="ecuId">
        /// The ECU identifier. May be <see langword="null"/> if the payload doesn't contain an ECU identifier.
        /// </param>
        /// <param name="frame">The frame that is returned if the message identifier exists.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the message identifier <paramref name="id"/> exists,
        /// <see langword="false"/> otherwise.
        /// </returns>
        /// <remarks>
        /// If the decoded message contains an extended header with the application and context identifier, then the
        /// message identifier may be reused.
        /// </remarks>
        public bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame)
        {
            return m_FrameMap.TryGetFrame(id, appId, ctxId, ecuId, out frame);
        }

        private void LoadFileInternal(Stream stream)
        {
            string ecuId = null;
            Dictionary<string, Pdu> pdus = new();

            // The XML standard says we have to have a unique ID. But it doesn't have to be unique when loading
            // different FIBEX files.
            Dictionary<int, IFrame> frames = new();

            XmlTreeReader xmlReader = new() {
                Nodes = {
                    new XmlTreeNode("fx:FIBEX") {
                        Nodes = {
                            new XmlTreeNode("fx:ELEMENTS") {
                                Nodes = {
                                    new XmlTreeNode("fx:ECUS") {
                                        Nodes = {
                                            new XmlTreeNode("fx:ECU") {
                                                ProcessElement = (n, e) => {
                                                    if (ecuId is not null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.EcusMultipleDefined, e.Reader.GetPosition()));
                                                        return;
                                                    }

                                                    if (frames.Count > 0) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.EcuIdMustBeBeforeFrames, e.Reader.GetPosition()));
                                                    }

                                                    string ecu = e.Reader["ID"];
                                                    if (string.IsNullOrWhiteSpace(ecu)) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.EcuIdMissing, e.Reader.GetPosition()));
                                                        return;
                                                    }
                                                    ecuId = ecu;
                                                }
                                            }
                                        }
                                    },
                                    new XmlTreeNode("fx:PDUS") {
                                        Nodes = {
                                            new XmlTreeNode("fx:PDU") {
                                                ProcessElement = (n, e) => {
                                                    string key = e.Reader["ID"];
                                                    if (string.IsNullOrWhiteSpace(key)) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.PduIdMissing, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    if (pdus.ContainsKey(key)) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.PduIdDuplicate, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    e.UserObject = new KeyValuePair<string, Pdu>(key, new Pdu());
                                                },
                                                Nodes = {
                                                    new XmlTreeNode("fx:BYTE-LENGTH") {
                                                        ProcessTextElement = (n, e) => {
                                                            bool result = int.TryParse(e.Text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int byteLength);
                                                            if (!result || byteLength < 0) {
                                                                OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.PduInvalidByteLength, e.Reader.GetPosition()));
                                                            }
                                                            ((KeyValuePair<string, Pdu>)e.UserObject).Value.PduLength = byteLength;
                                                        },
                                                    },
                                                    new XmlTreeNode("ho:DESC") {
                                                        ProcessTextElement = (n, e) => {
                                                            ((KeyValuePair<string, Pdu>)e.UserObject).Value.Description = e.Text;
                                                        }
                                                    },
                                                    new XmlTreeNode("fx:SIGNAL-INSTANCES") {
                                                        Nodes = {
                                                            new XmlTreeNode("fx:SIGNAL-INSTANCE") {
                                                                Nodes = {
                                                                    new XmlTreeNode("fx:SIGNAL-REF") {
                                                                        ProcessElement = (n, e) => {
                                                                            string signalId = e.Reader["ID-REF"];
                                                                            if (string.IsNullOrWhiteSpace(signalId)) {
                                                                                OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.MissingSignalIdRef, e.Reader.GetPosition()));
                                                                                ((KeyValuePair<string, Pdu>)e.UserObject).Value.PduType = string.Empty;
                                                                                return;
                                                                            }
                                                                            if (((KeyValuePair<string, Pdu>)e.UserObject).Value.PduType is not null) {
                                                                                OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.MultipleSignalsInPduDefined, e.Reader.GetPosition()));
                                                                                return;
                                                                            }
                                                                            ((KeyValuePair<string, Pdu>)e.UserObject).Value.PduType = signalId;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                ProcessEndElement = (n, e) => {
                                                    if (e.UserObject is null) return;
                                                    string key = ((KeyValuePair<string, Pdu>)e.UserObject).Key;
                                                    Pdu value = ((KeyValuePair<string, Pdu>)e.UserObject).Value;

                                                    if (value.Description is null && value.PduType is null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.MissingSignalIdRef, e.Reader.GetPosition()));
                                                    }
                                                    pdus.Add(key, value);
                                                }
                                            }
                                        }
                                    },
                                    new XmlTreeNode("fx:FRAMES") {
                                        Nodes = {
                                            new XmlTreeNode("fx:FRAME") {
                                                ProcessElement = (n, e) => {
                                                    string key = e.Reader["ID"];
                                                    if (string.IsNullOrWhiteSpace(key)) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameIdMissing, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    if (key.Length < 4 || !key.StartsWith("ID_")) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameIdInvalid, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    bool result = int.TryParse(key.AsSpan(3), NumberStyles.None, CultureInfo.InvariantCulture, out int msgId);
                                                    if (!result) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameIdInvalid, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    if (frames.ContainsKey(msgId)) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameIdDuplicate, e.Reader.GetPosition()));
                                                        e.Reader.Skip();  // Don't parse the rest.
                                                        return;
                                                    }
                                                    Frame frame = new() {
                                                        Id = msgId,
                                                        EcuId = ecuId
                                                    };
                                                    e.UserObject = new KeyValuePair<int, Frame>(msgId, frame);
                                                },
                                                Nodes = {
                                                    new XmlTreeNode("fx:MANUFACTURER-EXTENSION") {
                                                        Nodes = {
                                                            new XmlTreeNode("APPLICATION_ID") {
                                                                ProcessTextElement = (n, e) => {
                                                                    ((KeyValuePair<int, Frame>)e.UserObject).Value.ApplicationId = e.Text;
                                                                }
                                                            },
                                                            new XmlTreeNode("CONTEXT_ID") {
                                                                ProcessTextElement = (n, e) => {
                                                                    ((KeyValuePair<int, Frame>)e.UserObject).Value.ContextId = e.Text;
                                                                }
                                                            },
                                                            new XmlTreeNode("MESSAGE_TYPE") {
                                                                ProcessTextElement = (n, e) => {
                                                                    ((KeyValuePair<int, Frame>)e.UserObject).Value.MessageDltType = e.Text.Trim();
                                                                }
                                                            },
                                                            new XmlTreeNode("MESSAGE_INFO") {
                                                                ProcessTextElement = (n, e) => {
                                                                    ((KeyValuePair<int, Frame>)e.UserObject).Value.MessageDltInfo = e.Text.Trim();
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new XmlTreeNode("fx:PDU-INSTANCES") {
                                                        Nodes = {
                                                            new XmlTreeNode("fx:PDU-INSTANCE") {
                                                                Nodes = {
                                                                    new XmlTreeNode("fx:PDU-REF") {
                                                                        ProcessElement = (n, e) => {
                                                                            Frame frame = ((KeyValuePair<int, Frame>)e.UserObject).Value;
                                                                            if (!frame.IsValid) return;

                                                                            string pduref = e.Reader["ID-REF"];
                                                                            if (string.IsNullOrWhiteSpace(pduref)) {
                                                                                OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FramePduRefIdMissing, e.Reader.GetPosition()));
                                                                                frame.IsValid = false;
                                                                                return;
                                                                            }

                                                                            if (!pdus.TryGetValue(pduref, out Pdu pdu)) {
                                                                                OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FramePduRefIdUnknown, e.Reader.GetPosition()));
                                                                                frame.IsValid = false;
                                                                                return;
                                                                            }
                                                                            frame.AddArgument(pdu);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                ProcessEndElement = (n, e) => {
                                                    if (e.UserObject is null) return;

                                                    Frame frame = ((KeyValuePair<int, Frame>)e.UserObject).Value;
                                                    if (!frame.IsValid) return;
                                                    if (frame.ApplicationId is null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameApplicationIdMissing, e.Reader.GetPosition()));
                                                        frame.IsValid = false;
                                                    }
                                                    if (frame.ContextId is null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameContextIdMissing, e.Reader.GetPosition()));
                                                        frame.IsValid = false;
                                                    }
                                                    if (frame.MessageDltType is null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameMessageTypeMissing, e.Reader.GetPosition()));
                                                        frame.IsValid = false;
                                                    }
                                                    if (frame.MessageDltInfo is null) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameMessageInfoMissing, e.Reader.GetPosition()));
                                                        frame.IsValid = false;
                                                    }
                                                    if (!frame.IsValid) return;

                                                    frame.MessageType = GetMessageType(frame.MessageDltType, frame.MessageDltInfo);
                                                    if (frame.MessageType == DltType.UNKNOWN) {
                                                        OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.FrameMessageTypeInvalid, e.Reader.GetPosition()));
                                                    }

                                                    // Remove references, which allows garbage collection.
                                                    frame.MessageDltType = null;
                                                    frame.MessageDltInfo = null;

                                                    int key = ((KeyValuePair<int, Frame>)e.UserObject).Key;
                                                    frames.Add(key, frame);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            xmlReader.Read(stream, m_XmlNs);
            foreach (var frame in frames.Values) {
                if (!m_FrameMap.TryAddFrame(frame.Id, frame.ApplicationId, frame.ContextId, ecuId, frame)) {
                    OnLoadErrorEvent(new FibexLoadErrorEventArgs(FibexWarnings.DuplicateEntry, null, frame.Id, frame.ApplicationId, frame.ContextId, ecuId));
                }
            }
        }

        private static DltType GetMessageType(string dltType, string dltInfo)
        {
            if (dltType == "DLT_TYPE_LOG") {
                if (dltInfo == "DLT_LOG_FATAL") return DltType.LOG_FATAL;
                if (dltInfo == "DLT_LOG_ERROR") return DltType.LOG_ERROR;
                if (dltInfo == "DLT_LOG_WARN") return DltType.LOG_WARN;
                if (dltInfo == "DLT_LOG_INFO") return DltType.LOG_INFO;
                if (dltInfo == "DLT_LOG_DEBUG") return DltType.LOG_DEBUG;
                if (dltInfo == "DLT_LOG_VERBOSE") return DltType.LOG_VERBOSE;
                return DltConstants.MessageType.DltTypeLog;
            } else if (dltType == "DLT_TYPE_APP_TRACE") {
                if (dltInfo == "DLT_TRACE_VARIABLE") return DltType.APP_TRACE_VARIABLE;
                if (dltInfo == "DLT_TRACE_FUNCTION_IN") return DltType.APP_TRACE_FUNCTION_IN;
                if (dltInfo == "DLT_TRACE_FUNCTION_OUT") return DltType.APP_TRACE_FUNCTION_OUT;
                if (dltInfo == "DLT_TRACE_STATE") return DltType.APP_TRACE_STATE;
                if (dltInfo == "DLT_TRACE_VFB") return DltType.APP_TRACE_VFB;
                return (DltType)DltConstants.MessageType.DltTypeAppTrace;
            } else if (dltType == "DLT_TYPE_NW_TRACE") {
                if (dltInfo == "DLT_NW_TRACE_IPC") return DltType.NW_TRACE_IPC;
                if (dltInfo == "DLT_NW_TRACE_CAN") return DltType.NW_TRACE_CAN;
                if (dltInfo == "DLT_NW_TRACE_FLEXRAY") return DltType.NW_TRACE_FLEXRAY;
                if (dltInfo == "DLT_NW_TRACE_MOST") return DltType.NW_TRACE_MOST;
                if (dltInfo == "DLT_NW_TRACE_ETHERNET") return DltType.NW_TRACE_ETHERNET;
                if (dltInfo == "DLT_NW_TRACE_SOMEIP") return DltType.NW_TRACE_SOMEIP;
                return (DltType)DltConstants.MessageType.DltTypeNwTrace;
            } else {
                return DltType.UNKNOWN;
            }
        }
    }
}
