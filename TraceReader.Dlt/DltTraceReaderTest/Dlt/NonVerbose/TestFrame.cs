namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class TestFrame : IFrame
    {
        public TestFrame() { }

        public TestFrame(int id)
        {
            Id = id;
        }

        public string EcuId { get; set; }

        public int Id { get; set; }

        public string ApplicationId { get; set; }

        public string ContextId { get; set; }

        public DltType MessageType { get; set; }

        private readonly List<TestPdu> m_Pdus = new List<TestPdu>();

        public IReadOnlyList<IPdu> Arguments { get { return m_Pdus; } }

        public TestFrame AddArgument(IPdu pdu)
        {
            if (pdu == null) throw new ArgumentNullException(nameof(pdu));

            if (pdu.Description != null) {
                m_Pdus.Add(new TestPdu(pdu.Description));
            } else {
                m_Pdus.Add(new TestPdu(pdu.PduType, pdu.PduLength));
            }
            return this;
        }

        public TestFrame AddArgument(string description)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));

            m_Pdus.Add(new TestPdu(description));
            return this;
        }

        public TestFrame AddArgument(string pduType, int pduLength)
        {
            if (pduType == null) throw new ArgumentNullException(nameof(pduType));

            m_Pdus.Add(new TestPdu(pduType, pduLength));
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"EcuID={EcuId} AppId={ApplicationId} CtxId={ContextId} {Id} ({MessageType})");

            if (m_Pdus.Count > 0) {
                foreach (TestPdu pdu in m_Pdus) {
                    sb.Append(' ').Append(pdu.ToString());
                }
            }
            return sb.ToString();
        }
    }

    public sealed class FrameComparer : IEqualityComparer<IFrame>
    {
        public static readonly FrameComparer Comparer = new FrameComparer();

        public bool Equals(IFrame x, IFrame y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            if (x.EcuId != y.EcuId ||
                x.ApplicationId != y.ApplicationId ||
                x.ContextId != y.ContextId ||
                x.Id != y.Id ||
                x.MessageType != y.MessageType) return false;

            if (x.Arguments == null && y.Arguments != null) return false;
            if (x.Arguments != null && y.Arguments == null) return false;
            if (x.Arguments != null && y.Arguments != null) {
                if (x.Arguments.Count != y.Arguments.Count) return false;

                for (int i = 0; i < x.Arguments.Count; i++) {
                    if (!PduComparer.Comparer.Equals(x.Arguments[i], y.Arguments[i])) return false;
                }
            }
            return true;
        }

        public int GetHashCode(IFrame obj)
        {
            int hash = 0;

            if (obj.EcuId != null) hash ^= obj.EcuId.GetHashCode();
            if (obj.ApplicationId != null) hash ^= obj.ApplicationId.GetHashCode();
            if (obj.ContextId != null) hash ^= obj.ContextId.GetHashCode();
            hash ^= (int)obj.MessageType << 16;
            hash ^= obj.Id;

            if (obj.Arguments != null) {
                foreach (IPdu pdu in obj.Arguments) {
                    hash ^= PduComparer.Comparer.GetHashCode(pdu);
                }
            }

            return hash;
        }
    }
}
