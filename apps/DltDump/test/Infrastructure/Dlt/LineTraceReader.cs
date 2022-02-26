namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RJCP.Diagnostics.Log;

    public sealed class LineTraceReader<T> : ITraceReader<T> where T : ITraceLine
    {
        private readonly IEnumerator<T> m_LineEnumerable;

        public LineTraceReader(IEnumerable<T> lines)
        {
            List<T> list = new List<T>(lines);
            m_LineEnumerable = list.GetEnumerator();
        }

        public Task<T> GetLineAsync()
        {
            if (m_LineEnumerable.MoveNext()) {
                return Task.FromResult(m_LineEnumerable.Current);
            }
            return Task.FromResult<T>(default);
        }

        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
