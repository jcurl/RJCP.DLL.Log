﻿namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;

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
        /// Gets the name of the input file, associated with the connection string.
        /// </summary>
        /// <value>The name of the input file.</value>
        /// <remarks>
        /// The input file name can be used to calculate the output file name, given the input is loaded from a file
        /// system like URI. If the connection string is not file based, the result may be empty or a
        /// <see langword="null"/> string.
        /// </remarks>
        string InputFileName { get; }

        /// <summary>
        /// Gets a value indicating if this input stream requires a connection.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, this stream requires a connection; otherwise, <see langword="false"/>.
        /// </value>
        bool RequiresConnection { get; }

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

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>
        /// The input stream. If this object isn't opened, or it doesn't support streams, then <see langword="null"/> is
        /// set.
        /// </value>
        Stream InputStream { get; }

        /// <summary>
        /// Gets the input packet provider.
        /// </summary>
        /// <value>
        /// The input packet provider. If this object isn't open, or it doesn't support packets, then
        /// <see langword="null"/> is set.
        /// </value>
        IPacket InputPacket { get; }

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
        /// Closes this stream, but does not dispose, so it can be reopened.
        /// </summary>
        void Close();
    }
}
