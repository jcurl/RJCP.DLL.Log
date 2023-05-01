namespace RJCP.App.DltDump.Domain.Dlt
{
    using System.Reflection;
    using RJCP.CodeQuality;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    public class TraceReaderAccessor : AccessorBase
    {
        public TraceReaderAccessor(ITraceReader<DltTraceLineBase> reader)
            : base(new PrivateObject(reader, new PrivateType(typeof(TraceReader<DltTraceLineBase>))))
        {
            BindingFlags |= BindingFlags.NonPublic;
        }

        public ITraceDecoder<DltTraceLineBase> Decoder
        {
            get { return (ITraceDecoder<DltTraceLineBase>)GetFieldOrProperty("m_Decoder"); }
        }
    }
}
