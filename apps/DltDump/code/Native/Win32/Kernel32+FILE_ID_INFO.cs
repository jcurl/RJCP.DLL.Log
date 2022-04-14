namespace RJCP.App.DltDump.Native.Win32
{
    using System.Runtime.InteropServices;

    internal static partial class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct FILE_ID_INFO
        {
            public ulong VolumeSerialNumber;
            public ulong FileIdLow;
            public ulong FileIdHigh;
        }
    }
}
