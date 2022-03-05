namespace RJCP.App.DltDump.Domain
{
    using System;

    /// <summary>
    /// An Input Stream Factory.
    /// </summary>
    public interface IInputStreamFactory
    {
        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        IInputStream Create(string uri);

        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        IInputStream Create(Uri uri);
    }
}
