namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    /// <summary>
    /// Base class promoting reusable code for decoder checks.
    /// </summary>
    public abstract class ControlArgDecoderBase : IControlArgDecoder
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
        public abstract int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service);

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The default type.</param>
        /// <param name="message">The message that is part of the decode error.</param>
        /// <param name="service">The <see cref="ControlDecodeError"/> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(int serviceId, DltType defaultType, string message, out IControlArg service)
        {
            service = new ControlDecodeError(serviceId, defaultType, message);
            return -1;
        }

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The default type.</param>
        /// <param name="format">The format message that is part of the decode error.</param>
        /// <param name="arg1">The first argument to format.</param>
        /// <param name="service">The <see cref="ControlDecodeError"/> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(int serviceId, DltType defaultType, string format, object arg1, out IControlArg service)
        {
            service = new ControlDecodeError(serviceId, defaultType, string.Format(format, arg1));
            return -1;
        }

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="defaultType">The default type.</param>
        /// <param name="format">The format message that is part of the decode error.</param>
        /// <param name="arg1">The first argument to format.</param>
        /// <param name="arg2">The second argument to format.</param>
        /// <param name="service">The <see cref="ControlDecodeError"/> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(int serviceId, DltType defaultType, string format, object arg1, object arg2, out IControlArg service)
        {
            service = new ControlDecodeError(serviceId, defaultType, string.Format(format, arg1, arg2));
            return -1;
        }
    }
}
