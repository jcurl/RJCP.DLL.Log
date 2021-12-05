namespace RJCP.Diagnostics.Log
{
    using System;

    public static class DltTime
    {
        public static readonly DateTime Default = new DateTime(1970, 1, 1, 0, 0, 0);

        public static TimeSpan DeviceTime(double seconds)
        {
            return new TimeSpan((long)(seconds * TimeSpan.TicksPerSecond));
        }

        public static DateTime LogTime(int year, int month, int day, int hour, int minute, int second, double milliseconds)
        {
            return new DateTime(year, month, day, hour, minute, second).AddTicks((long)(milliseconds * TimeSpan.TicksPerMillisecond));
        }
    }
}
