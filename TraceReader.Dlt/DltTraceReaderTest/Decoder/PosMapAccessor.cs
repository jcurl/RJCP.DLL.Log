namespace RJCP.Diagnostics.Log.Decoder
{
    using RJCP.CodeQuality;

    public class PosMapAccessor : AccessorBase
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Decoder.DltTraceDecoderBase";

        private static PrivateType GetNestedPosMapType()
        {
            PrivateType parent = new PrivateType(AssemblyName, TypeName);
            return parent.GetNestedType("PosMap");
        }

        private static readonly PrivateType AccType = GetNestedPosMapType();

        public PosMapAccessor() : base(AccType) { }

        public PosMapAccessor(int size) : base(AccType, size) { }

        public long Position
        {
            get { return (long)GetFieldOrProperty(nameof(Position)); }
        }

        public int Length
        {
            get { return (int)GetFieldOrProperty(nameof(Length)); }
        }

        public void Append(long position, int length)
        {
            Invoke(nameof(Append), position, length);
        }

        public void Consume(int length)
        {
            Invoke(nameof(Consume), length);
        }

        public void Reset()
        {
            Invoke(nameof(Reset));
        }
    }
}
