namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Globalization;

    public static class DltTime
    {
        public static readonly DateTime Default = new DateTime(1970, 1, 1, 0, 0, 0);

        public static TimeSpan DeviceTime(double seconds)
        {
            // Multiply seconds by a smaller number than TicksPerSecond to avoid quantization rounding in double.
            return new TimeSpan((long)(seconds * 10000) * (TimeSpan.TicksPerSecond / 10000));
        }

        public static DateTime FileTime(int year, int month, int day, int hour, int minute, double seconds)
        {
            // The method AddSeconds truncates to milliseconds, so we must use AddTicks() to get the number of
            // microseconds. As per docs, "The value parameter is rounded to the nearest millisecond".

            // Multiply seconds by a smaller number than TicksPerSecond to avoid quantization rounding in double.
            return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc)
                .AddTicks((long)(seconds * 10000) * (TimeSpan.TicksPerSecond / 10000));
        }

        public static string LocalTime(DateTime time)
        {
            // The default time, if no time stamp is available, is 1/1/1970 local time. Else it's decoded as UTC time.
            if (time.Equals(new DateTime(1970, 1, 1))) {
                return time.ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
            }
            return time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
        }
    }
}
