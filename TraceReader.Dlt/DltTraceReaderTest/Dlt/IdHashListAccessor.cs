namespace RJCP.Diagnostics.Log.Dlt
{
    using RJCP.CodeQuality;

    public class IdHashListAccessor : AccessorBase
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Dlt.IdHashList";

        public static readonly PrivateType AccType = new(AssemblyName, TypeName);

        public IdHashListAccessor() : base(AccType) { }

        public string ParseId(int id)
        {
            return (string)Invoke(nameof(ParseId), id);
        }
    }
}
