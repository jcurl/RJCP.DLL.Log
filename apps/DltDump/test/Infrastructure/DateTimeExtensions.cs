namespace RJCP.App.DltDump.Infrastructure
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime AddNanoSeconds(this DateTime dateTime, long nanoSeconds)
        {
            ThrowHelper.ThrowIfNotBetween(nanoSeconds, -999_999_999, 999_999_999);
            return dateTime.AddTicks(nanoSeconds * TimeSpan.TicksPerSecond / 1_000_000_000);
        }

        public static TimeSpan AddNanoSeconds(this TimeSpan timeSpan, long nanoSeconds)
        {
            ThrowHelper.ThrowIfNotBetween(nanoSeconds, -999_999_999, 999_999_999);
            return new TimeSpan(timeSpan.Ticks + nanoSeconds * TimeSpan.TicksPerSecond / 1_000_000_000);
        }
    }
}
