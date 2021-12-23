namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Globalization;

    public static class DltTime
    {
        public static readonly DateTime Default = new DateTime(1970, 1, 1, 0, 0, 0);

        public static TimeSpan DeviceTime(double seconds)
        {
            return new TimeSpan((long)(seconds * TimeSpan.TicksPerSecond));
        }

        public static DateTime FileTime(int year, int month, int day, int hour, int minute, double seconds)
        {
            // The method AddSeconds truncates to milliseconds, so we must use AddTicks() to get the number of
            // microseconds. As per docs, "The value parameter is rounded to the nearest millisecond".
            return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc)
                .AddTicks((long)(seconds * TimeSpan.TicksPerSecond));
        }

        public static string LocalTime(DateTime time)
        {
            return time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
        }
    }
}
