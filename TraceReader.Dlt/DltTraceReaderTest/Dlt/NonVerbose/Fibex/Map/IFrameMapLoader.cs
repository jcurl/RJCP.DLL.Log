namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    /// <summary>
    /// An extension interface to initialise the map.
    /// </summary>
    /// <remarks>
    /// This is a copy from the internal structure <c>IFrameMapLoader</c> of the assembly being tested. It is duplicated
    /// here as the accessibility must be public. It's required that we can implement common tests.
    /// </remarks>
    public interface IFrameMapLoader : IFrameMap
    {
        /// <summary>
        /// Tries to add the frame to the map.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">The application identifier. May not be <see langword="null"/>.</param>
        /// <param name="ctxId">The context identifier. May not be <see langword="null"/>.</param>
        /// <param name="ecuId">The ECU identifier. May be <see langword="null"/> if no ECU ID is known.</param>
        /// <param name="frame">The frame that maps to the parameters given.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the message identifier <paramref name="id"/> is unique for the given
        /// parameters, <see langword="false"/> otherwise.
        /// </returns>
        bool TryAddFrame(int id, string appId, string ctxId, string ecuId, IFrame frame);
    }
}
