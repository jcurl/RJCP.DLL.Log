namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RJCP.Diagnostics.Log;

    public sealed class LineTraceReader<T> : ITraceReader<T> where T : ITraceLine
    {
        private readonly IEnumerator<T> m_LineEnumerable;

        public LineTraceReader(IEnumerable<T> lines)
        {
            List<T> list = new(lines);
            m_LineEnumerable = list.GetEnumerator();
        }

        public event EventHandler<LineEventArgs<T>> GetLineEvent;

        public Task<T> GetLineAsync()
        {
            T line;
            if (m_LineEnumerable.MoveNext()) {
                line = OnGetLineEvent(this, new LineEventArgs<T>(m_LineEnumerable.Current));
            } else {
                line = OnGetLineEvent(this, new LineEventArgs<T>(default));
            }
            return Task.FromResult(line);
        }

        private T OnGetLineEvent(object sender, LineEventArgs<T> args)
        {
            EventHandler<LineEventArgs<T>> handler = GetLineEvent;
            if (handler is not null) handler(sender, args);
            return args.Line;
        }

        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
