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

        private readonly List<TestPdu> m_Pdus = new();

        public IReadOnlyList<IPdu> Arguments { get { return m_Pdus; } }

        public TestFrame AddArgument(IPdu pdu)
        {
            ArgumentNullException.ThrowIfNull(pdu);

            if (pdu.Description is not null) {
                m_Pdus.Add(new TestPdu(pdu.Description));
            } else {
                m_Pdus.Add(new TestPdu(pdu.PduType, pdu.PduLength));
            }
            return this;
        }

        public TestFrame AddArgument(string description)
        {
            ArgumentNullException.ThrowIfNull(description);

            m_Pdus.Add(new TestPdu(description));
            return this;
        }

        public TestFrame AddArgument(string pduType, int pduLength)
        {
            ArgumentNullException.ThrowIfNull(pduType);

            m_Pdus.Add(new TestPdu(pduType, pduLength));
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new($"EcuID={EcuId} AppId={ApplicationId} CtxId={ContextId} {Id} ({MessageType})");

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
        public static readonly FrameComparer Comparer = new();

        public bool Equals(IFrame x, IFrame y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            if (x.EcuId != y.EcuId ||
                x.ApplicationId != y.ApplicationId ||
                x.ContextId != y.ContextId ||
                x.Id != y.Id ||
                x.MessageType != y.MessageType) return false;

            if (x.Arguments is null && y.Arguments is not null) return false;
            if (x.Arguments is not null && y.Arguments is null) return false;
            if (x.Arguments is not null && y.Arguments is not null) {
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

            if (obj.EcuId is not null) hash ^= obj.EcuId.GetHashCode();
            if (obj.ApplicationId is not null) hash ^= obj.ApplicationId.GetHashCode();
            if (obj.ContextId is not null) hash ^= obj.ContextId.GetHashCode();
            hash ^= (int)obj.MessageType << 16;
            hash ^= obj.Id;

            if (obj.Arguments is not null) {
                foreach (IPdu pdu in obj.Arguments) {
                    hash ^= PduComparer.Comparer.GetHashCode(pdu);
                }
            }

            return hash;
        }
    }
}
