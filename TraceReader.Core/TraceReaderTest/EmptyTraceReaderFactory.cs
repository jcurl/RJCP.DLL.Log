namespace RJCP.Diagnostics.Log
{
    using Decoder;

    public class EmptyTraceReaderFactory : TraceReaderFactory<ITraceLine>
    {
        private class EmptyTraceDecoderFactory : ITraceDecoderFactory<ITraceLine>
        {
            public ITraceDecoder<ITraceLine> Create()
            {
                return new EmptyTraceDecoder();
            }
        }

        public EmptyTraceReaderFactory() : base(new EmptyTraceDecoderFactory()) { }
    }
}
