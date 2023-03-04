namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using Native.Win32;
    using Resources;
    using RJCP.Core.Environment;

    public sealed class DetailedFileInfo : IEquatable<DetailedFileInfo>
    {
        private enum InfoType
        {
            None,
            WindowsExtended,
            WindowsDefault,
            Other
        }

        private readonly InfoType m_InfoType;
        private readonly ulong m_VolumeSerialNumber;
        private readonly ulong m_FileIdLow;
        private readonly ulong m_FileIdHigh;
        private readonly int m_HashCode;

        public DetailedFileInfo(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(AppResources.FileOpenError_EmptyName, nameof(fileName));

            FileName = Path.GetFullPath(fileName);
            if (!File.Exists(FileName))
                throw new FileNotFoundException(AppResources.InfraFileNotFound, FileName);

            if (Platform.IsWinNT()) {
                SafeFileHandle file = Kernel32.CreateFile(fileName, 0,
                    Kernel32.FileShare.FILE_SHARE_READ | Kernel32.FileShare.FILE_SHARE_WRITE | Kernel32.FileShare.FILE_SHARE_DELETE,
                    IntPtr.Zero, Kernel32.CreationDisposition.OPEN_EXISTING,
                    Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
                if (file.IsInvalid) {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    throw new InvalidOperationException();
                }

                try {
                    bool result;

                    result = Kernel32.GetFileInformationByHandleEx(file, Kernel32.FILE_INFO_BY_HANDLE_CLASS.FileIdInfo,
                        out Kernel32.FILE_ID_INFO fileInfoEx, Marshal.SizeOf<Kernel32.FILE_ID_INFO>());
                    if (result) {
                        m_InfoType = InfoType.WindowsExtended;
                        m_VolumeSerialNumber = fileInfoEx.VolumeSerialNumber;
                        m_FileIdHigh = fileInfoEx.FileIdHigh;
                        m_FileIdLow = fileInfoEx.FileIdLow;
                        m_HashCode = unchecked(
                            (int)fileInfoEx.VolumeSerialNumber ^
                            (int)fileInfoEx.FileIdHigh ^
                            (int)fileInfoEx.FileIdLow
                        );
                        return;
                    }

                    result = Kernel32.GetFileInformationByHandle(file, out Kernel32.BY_HANDLE_FILE_INFORMATION fileInfoByHandle);
                    if (result) {
                        m_InfoType = InfoType.WindowsDefault;
                        m_VolumeSerialNumber = fileInfoByHandle.VolumeSerialNumber;
                        m_FileIdHigh = fileInfoByHandle.FileIndexHigh;
                        m_FileIdLow = fileInfoByHandle.FileIndexLow;
                        m_HashCode = unchecked(
                            (int)fileInfoByHandle.VolumeSerialNumber ^
                            (int)fileInfoByHandle.FileIndexHigh ^
                            (int)fileInfoByHandle.FileIndexLow
                        );
                        return;
                    }

                    m_InfoType = InfoType.Other;
                    m_HashCode = FileName.GetHashCode();
                } finally {
                    file.Close();
                }
            } else {
                // We assume that the FileName is to be used for comparison. This is not as robust as checking the
                // INodes.
                m_InfoType = InfoType.Other;
                m_HashCode = FileName.GetHashCode();
            }
        }

        public string FileName { get; }

        public bool Equals(DetailedFileInfo other)
        {
            if (other == null) return false;

            // The InfoType.None should never be set, unless there is a bug in this module.
            if (m_InfoType == InfoType.None || other.m_InfoType == InfoType.None) {
                throw new InvalidOperationException(AppResources.InfraIoFileInfoInvalidState);
            }

            if (m_InfoType != other.m_InfoType) return false;

            switch (m_InfoType) {
            case InfoType.WindowsDefault:
            case InfoType.WindowsExtended:
                return m_VolumeSerialNumber == other.m_VolumeSerialNumber &&
                    m_FileIdHigh == other.m_FileIdHigh &&
                    m_FileIdLow == other.m_FileIdLow;
            case InfoType.Other:
                return FileName.Equals(other.FileName, StringComparison.Ordinal);
            default:
                // Should never get here.
                throw new InvalidOperationException(AppResources.InfraIoFileInfoInvalidState);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            DetailedFileInfo fileInfo = obj as DetailedFileInfo;
            if (fileInfo == null) return false;
            return Equals(fileInfo);
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }

        public static bool operator ==(DetailedFileInfo info1, DetailedFileInfo info2)
        {
            if (((object)info1) == null || ((object)info2) == null)
                return object.Equals(info1, info2);

            return info1.Equals(info2);
        }

        public static bool operator !=(DetailedFileInfo info1, DetailedFileInfo info2)
        {
            if (((object)info1) == null || ((object)info2) == null)
                return !object.Equals(info1, info2);

            return !info1.Equals(info2);
        }
    }
}
