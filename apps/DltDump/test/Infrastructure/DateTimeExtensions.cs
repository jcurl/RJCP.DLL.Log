namespace RJCP.App.DltDump.Infrastructure
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime AddNanoSeconds(this DateTime dateTime, long nanoSeconds)
        {
            if (nanoSeconds >= 1_000_000_000) throw new ArgumentOutOfRangeException(nameof(nanoSeconds));
            if (nanoSeconds <= -1_000_000_000) throw new ArgumentOutOfRangeException(nameof(nanoSeconds));

            return dateTime.AddTicks(nanoSeconds * TimeSpan.TicksPerSecond / 1000000000);
        }

        public static TimeSpan AddNanoSeconds(this TimeSpan timeSpan, long nanoSeconds)
        {
            if (nanoSeconds >= 1_000_000_000) throw new ArgumentOutOfRangeException(nameof(nanoSeconds));
            if (nanoSeconds <= -1_000_000_000) throw new ArgumentOutOfRangeException(nameof(nanoSeconds));

            return new TimeSpan(timeSpan.Ticks + nanoSeconds * TimeSpan.TicksPerSecond / 1000000000);
        }
    }
}
