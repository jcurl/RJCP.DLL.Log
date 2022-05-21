namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using RJCP.Diagnostics.Log;

    public sealed class LineEventArgs<T> : EventArgs where T : ITraceLine
    {

        public LineEventArgs(T line)
        {
            Line = line;
        }

        public T Line { get; }
    }
}
