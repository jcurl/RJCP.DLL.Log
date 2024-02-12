namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Collections.Generic;

    public class TestFrameMap : IFrameMap
    {
        private readonly Dictionary<int, IFrame> m_Map = new Dictionary<int, IFrame>();

        public TestFrameMap Add(int messageId, IPdu pdu)
        {
            return Add(messageId, null, "APP1", "CTX1", DltType.LOG_INFO, pdu);
        }

        public TestFrameMap Add(int messageId, string ecuId, string appId, string ctxId, DltType msgType, IPdu pdu)
        {
            ArgumentNullException.ThrowIfNull(pdu);

            TestFrame frame = new TestFrame(messageId).AddArgument(pdu);
            frame.EcuId = ecuId;
            frame.ApplicationId = appId;
            frame.ContextId = ctxId;
            frame.MessageType = msgType;
            m_Map.Add(messageId, frame);

            return this;
        }

        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            return m_Map[id];
        }

        public bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame)
        {
            return m_Map.TryGetValue(id, out frame);
        }
    }
}
