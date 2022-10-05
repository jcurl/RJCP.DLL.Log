namespace RJCP.App.DltDump.Domain.Dlt
{
    using System.Reflection;
    using RJCP.CodeQuality;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    public class TracePacketReaderAccessor : AccessorBase
    {
        public TracePacketReaderAccessor(ITraceReader<DltTraceLineBase> reader)
            : base(new PrivateObject(reader, new PrivateType(typeof(TracePacketReader))))
        {
            BindingFlags |= BindingFlags.NonPublic;
        }

        public ITraceDecoder<DltTraceLineBase> CreateDecoder()
        {
            // We want to get the ITraceDecoderFactory<DltTraceLineBase> m_DecoderFactory

            // Then we just instantiate one, to know the type it will generate. The actual
            // decoder might not have been used yet.
            ITraceDecoderFactory<DltTraceLineBase> factory =
                (ITraceDecoderFactory<DltTraceLineBase>)GetFieldOrProperty("m_DecoderFactory");

            return factory.Create();
        }
    }
}
