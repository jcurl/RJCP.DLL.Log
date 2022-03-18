namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    public class TraceLineTimeStamp : TraceLine, IDeviceTimeStamp
    {
        public TraceLineTimeStamp(string text, TimeSpan timeStamp) : base(text)
        {
            DeviceTimestamp = timeStamp;
        }

        public TimeSpan DeviceTimestamp { get; private set; }
    }
}
