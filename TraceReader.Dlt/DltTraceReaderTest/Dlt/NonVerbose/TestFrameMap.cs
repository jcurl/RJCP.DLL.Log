namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Collections.Generic;

    public class TestFrameMap : IFrameMap
    {
        private readonly Dictionary<int, IFrame> m_Map = new Dictionary<int, IFrame>();

        public TestFrameMap Add(int messageId, IPdu pdu)
        {
            if (pdu == null) throw new ArgumentNullException(nameof(pdu));

            TestFrame frame = new TestFrame(messageId).AddArgument(pdu);
            frame.ApplicationId = "APP1";
            frame.ContextId = "CTX1";
            frame.MessageType = DltType.LOG_INFO;
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
