namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;

    /// <summary>
    /// Base class promoting reusable code for decoder checks.
    /// </summary>
    public abstract class VerboseArgDecoderBase : IVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public abstract int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg);

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="message">The message that is part of the decode error.</param>
        /// <param name="arg">The <see cref="DltArgError"/> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(string message, out IDltArg arg)
        {
            arg = new DltArgError(message);
            return -1;
        }

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="format">The format message for the decode error.</param>
        /// <param name="arg1">The first parameter.</param>
        /// <param name="arg">The <see cref="DltArgError" /> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(string format, object arg1, out IDltArg arg)
        {
            arg = new DltArgError(string.Format(format, arg1));
            return -1;
        }

        /// <summary>
        /// Indicates a decoder error occurred, get the appropriate object and return a decoder error.
        /// </summary>
        /// <param name="format">The format message for the decode error.</param>
        /// <param name="arg1">The first parameter.</param>
        /// <param name="arg2">The second parameter.</param>
        /// <param name="arg">The <see cref="DltArgError" /> object indicating an error.</param>
        /// <returns>A decoder error, which is -1.</returns>
        protected static int DecodeError(string format, object arg1, object arg2, out IDltArg arg)
        {
            arg = new DltArgError(string.Format(format, arg1, arg2));
            return -1;
        }
    }
}
