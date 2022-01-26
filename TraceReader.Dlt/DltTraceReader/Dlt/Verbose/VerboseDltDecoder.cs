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
    public class VerboseDltDecoder : IVerboseDltDecoder
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
            if (argDecoder == null) throw new ArgumentNullException(nameof(argDecoder));
            m_ArgDecoder = argDecoder;
        }

        /// <summary>
        /// Decodes the specified buffer as a verbose payload.
        /// </summary>
        /// <param name="buffer">The buffer that should be decoded.</param>
        /// <param name="lineBuilder">
        /// The line builder providing information from the standard header, and where the decoded packets will be placed.
        /// </param>
        /// <returns>The length of all the decoded verbose arguments in the buffer.</returns>
        /// <remarks>
        /// When calling this decode method, the <paramref name="lineBuilder"/> contains the number of arguments that
        /// should be decoded, and the endianness that should be used for decoding the arguments.
        /// </remarks>
        public int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            try {
                int argCount = 0;
                int payloadLength = 0;

                do {
                    if (buffer.Length < 4) {
                        lineBuilder.SetErrorMessage(
                            "Verbose message with insufficient buffer length decoding arg {0} of {1}",
                            argCount + 1, lineBuilder.NumberOfArgs);
                        return -1;
                    }

                    int typeInfo = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    int argLength = m_ArgDecoder.Decode(typeInfo, buffer, lineBuilder.BigEndian, out IDltArg argument);
                    if (argLength < 0) {
                        if (argument is DltArgError argError) {
                            lineBuilder.SetErrorMessage(
                                "Verbose Message 0x{0:x} arg {1} of {2}, {3}",
                                typeInfo, argCount + 1, lineBuilder.NumberOfArgs, argError.Message);
                        } else {
                            lineBuilder.SetErrorMessage(
                                "Verbose Message 0x{0:x} arg {1} of {2} decoding error",
                                typeInfo, argCount + 1, lineBuilder.NumberOfArgs);
                        }
                        return -1;
                    }

                    lineBuilder.AddArgument(argument);
                    buffer = buffer[argLength..];
                    argCount++;
                    payloadLength += argLength;
                } while (argCount < lineBuilder.NumberOfArgs);
                return payloadLength;
            } catch (Exception ex) {
                Log.Dlt.TraceException(ex, nameof(Decode), "Verbose decoding exception");
                return -1;
            }
        }
    }
}
