namespace RJCP.App.DltDump.Domain.Dlt
{
    using System.Reflection;
    using RJCP.CodeQuality;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    public class TraceReaderAccessor<T> : AccessorBase where T : DltTraceLineBase
    {
        public TraceReaderAccessor(ITraceReader<T> reader)
            : base(new PrivateObject(reader, new PrivateType(typeof(TraceReader<T>))))
        {
            BindingFlags |= BindingFlags.NonPublic;
        }

        public ITraceDecoder<T> Decoder
        {
            get { return (ITraceDecoder<T>)GetFieldOrProperty("m_Decoder"); }
        }
    }
}
