namespace RJCP.App.DltDump.Native.Win32
{
    using System.Runtime.InteropServices;

    internal partial class Kernel32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FILE_ID_BOTH_DIR_INFO
        {
            public uint NextEntryOffset;
            public uint FileIndex;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public FILETIME ChangeTime;
            public LargeInteger EndOfFile;
            public LargeInteger AllocationSize;
            public uint FileAttributes;
            public uint FileNameLength;
            public uint EaSize;
            public char ShortNameLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string ShortName;
            public LargeInteger FileId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string FileName;
        }
    }
}
