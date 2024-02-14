namespace RJCP.App.DltDump.Domain
{
    using System.Collections.Generic;
    using System.IO;
    using FileSystemNodeInfo = RJCP.IO.FileSystemNodeInfo;

    /// <summary>
    /// Defines the complete list of input files.
    /// </summary>
    /// <remarks>
    /// The <see cref="IOutputStreamFactory"/> uses this to give the <see cref="IOutputStream"/>, so that if it is
    /// writing files, it won't overwrite an existing file.
    /// </remarks>
    public class InputFiles
    {
        private readonly HashSet<FileSystemNodeInfo> m_ProtectedFiles = new();

        /// <summary>
        /// Adds a file name that should be protected from writing.
        /// </summary>
        /// <param name="fileName">Name of the file that should be protected.</param>
        /// <remarks>
        /// Any files set here will prevent from being overwritten, even if forced. This could be, for example, the file
        /// is an input file.
        /// </remarks>
        public void AddProtectedFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return;

            FileSystemNodeInfo file = new(fileName);
            m_ProtectedFiles.Add(file);
        }

        /// <summary>
        /// Checks if the file name given is protected.
        /// </summary>
        /// <param name="fileName">Name of the file to check.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the file is protected and shouldn't be overwritten; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool IsProtectedFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return false;

            FileSystemNodeInfo file = new(fileName);
            return m_ProtectedFiles.Contains(file);
        }

        /// <summary>
        /// Clears the list of protected files.
        /// </summary>
        public void Clear()
        {
            m_ProtectedFiles.Clear();
        }
    }
}
