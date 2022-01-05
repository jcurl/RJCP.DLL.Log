namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Interface for decoders that know how to decode a specific service identifier.
    /// </summary>
    public interface IControlArgDecoder
    {
        /// <summary>
        /// Decodes the control message for the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="service">The control message.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service);
    }
}
