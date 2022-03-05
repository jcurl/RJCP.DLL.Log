namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for parsing an input stream.
    /// </summary>
    public interface IInputStream : IDisposable
    {
        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        Stream InputStream { get; }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>Returns if the input stream was connected.</returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Gets a value indicating whether this instance is live stream.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is live stream; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// A live stream generally indicates that time stamps should be based on the PC clock, and is not part of the
        /// input stream.
        /// </remarks>
        bool IsLiveStream { get; }

        /// <summary>
        /// Gets the suggested format that should be used for instantiating a decoder.
        /// </summary>
        /// <value>The suggested format.</value>
        InputFormat SuggestedFormat { get; }
    }
}
