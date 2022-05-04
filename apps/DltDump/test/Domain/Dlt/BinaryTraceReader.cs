namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Threading.Tasks;
    using RJCP.Diagnostics.Log;

    public sealed class BinaryTraceReader<T> : ITraceReader<T> where T : ITraceLine
    {
        public BinaryTraceReader(IOutputStream outputStream)
        {
            if (!outputStream.SupportsBinary)
                throw new ArgumentException("Must support binary", nameof(outputStream));
        }

        public Task<T> GetLineAsync()
        {
            return Task.FromResult<T>(default);
        }

        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
