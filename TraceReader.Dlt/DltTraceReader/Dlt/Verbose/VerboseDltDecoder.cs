namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// The <see cref="VerboseDltDecoder"/> is responsible for decoding all arguments in a verbose payload.
    /// </summary>
    /// <remarks>
    /// The <see cref="VerboseDltDecoder"/> decodes all verbose arguments in a payload, adding it to the line to be
    /// constructed by a <see cref="IDltLineBuilder"/>.
    /// </remarks>
    public sealed class VerboseDltDecoder : IVerboseDltDecoder
    {
        private readonly IVerboseArgDecoder m_ArgDecoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseDltDecoder"/> class.
        /// </summary>
        /// <param name="argDecoder">The argument decoder that knows how to decode an individual verbose argument.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argDecoder"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The <paramref name="argDecoder"/> is used to decode any verbose argument, given that the buffer starts at
        /// the verbose payload.
        /// </remarks>
        public VerboseDltDecoder(IVerboseArgDecoder argDecoder)
        {
            ArgumentNullException.ThrowIfNull(argDecoder);
            m_ArgDecoder = argDecoder;
        }

        /// <summary>
        /// Decodes the specified buffer as a verbose payload.
        /// </summary>
        /// <param name="buffer">
        /// The buffer that should be decoded. The packet starts where the verbose payload starts.
        /// </param>
        /// <param name="lineBuilder">
        /// The line builder providing information from the standard header, and where the decoded packets will be
        /// placed.
        /// </param>
        /// <returns>The length of all the decoded verbose arguments in the buffer.</returns>
        /// <remarks>
        /// When calling this decode method, the <paramref name="lineBuilder"/> contains the number of arguments that
        /// should be decoded, and the endianness that should be used for decoding the arguments.
        /// </remarks>
        public Result<int> Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            try {
                int payloadLength = 0;

                for (int argCount = 0; argCount < lineBuilder.NumberOfArgs; argCount++) {
                    if (buffer.Length < 4) {
                        string message = $"Verbose message with insufficient buffer length decoding arg {argCount + 1} of {lineBuilder.NumberOfArgs}";
                        lineBuilder.SetErrorMessage(message);
                        return Result.FromException<int>(new DltDecodeException(message));
                    }

                    int typeInfo = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    Result<int> argLength = m_ArgDecoder.Decode(typeInfo, buffer, lineBuilder.BigEndian, out IDltArg argument);
                    if (!argLength.TryGet(out int length)) {
                        string message = $"Verbose Message 0x{typeInfo:x} arg {argCount + 1} of {lineBuilder.NumberOfArgs}, {argLength.Error.Message}";
                        lineBuilder.SetErrorMessage(message);
                        return Result.FromException<int>(new DltDecodeException(message, argLength.Error));
                    }
                    lineBuilder.AddArgument(argument);
                    buffer = buffer[length..];
                    payloadLength += length;
                }

                return payloadLength;
            } catch (Exception ex) {
                Log.Dlt.TraceException(ex, nameof(Decode), "Verbose decoding exception");
                return Result.FromException<int>(ex);
            }
        }
    }
}
