namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;

    /// <summary>
    /// Interface for encoding an argument.
    /// </summary>
    public interface IArgEncoder
    {
        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the argument to.</param>
        /// <param name="verbose">If the argument encoding should include the type information.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        int Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg);
    }
}
