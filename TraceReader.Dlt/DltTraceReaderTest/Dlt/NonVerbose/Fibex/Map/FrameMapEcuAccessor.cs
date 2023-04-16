namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using RJCP.CodeQuality;

    public class FrameMapEcuAccessor : AccessorBase, IFrameMapLoader
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map.FrameMapEcu";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public FrameMapEcuAccessor() : base(AccType) { }

        public bool TryAddFrame(int id, string appId, string ctxId, string ecuId, IFrame frame)
        {
            return (bool)Invoke(nameof(TryAddFrame), id, appId, ctxId, ecuId, frame);
        }

        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            return (IFrame)Invoke(nameof(GetFrame), id, appId, ctxId, ecuId);
        }

        public bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame)
        {
            object[] args = new object[] { id, appId, ctxId, ecuId, null };
            bool result = (bool)Invoke(nameof(TryGetFrame),
                new Type[] { typeof(int), typeof(string), typeof(string), typeof(string), typeof(IFrame).MakeByRefType() }, args);
            frame = (IFrame)args[4];
            return result;
        }
    }
}
