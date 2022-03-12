namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;

    /// <summary>
    /// Interface for parsing an input stream.
    /// </summary>
    public interface IInputStream : IDisposable
    {
        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        string Scheme { get; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string Connection { get; }

        /// <summary>
        /// Gets a value indicating if this input stream requires a connection.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, this stream requires a connection; otherwise, <see langword="false"/>.
        /// </value>
        bool RequiresConnection { get; }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        Stream InputStream { get; }

        /// <summary>
        /// Opens the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        void Open();

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
