namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System.Collections.Generic;

    public sealed class TestPdu : IPdu
    {
        public TestPdu(string pduType, int pduLength)
        {
            PduType = pduType;
            PduLength = pduLength;
        }

        public TestPdu(string description)
        {
            Description = description;
        }

        public string PduType { get; private set; }

        public string Description { get; private set; }

        public int PduLength { get; private set; }

        public override string ToString()
        {
            if (Description != null) {
                return $"[{Description}]";
            }
            return $"{PduType} ({PduLength})";
        }
    }

    public sealed class PduComparer : IEqualityComparer<IPdu>
    {
        public static readonly PduComparer Comparer = new PduComparer();

        public bool Equals(IPdu x, IPdu y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.PduType == y.PduType &&
                x.PduLength == y.PduLength &&
                x.Description == y.Description;
        }

        public int GetHashCode(IPdu obj)
        {
            int hash = obj.PduLength * 65532;

            if (obj.Description != null) hash ^= obj.Description.GetHashCode();
            if (obj.PduType != null) hash ^= obj.PduType.GetHashCode();
            return hash;
        }
    }
}
