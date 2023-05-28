namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Encodes a <see cref="SwInjectionRequest"/> based on the payload.
    /// </summary>
    public class SwInjectionRequestEncoder : ControlArgEncoderBase
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        /// <remarks>
        /// Encodes the raw payload given by the <see cref="SwInjectionRequest"/>. If you need your own custom encoding
        /// for the injection request, your own custom control request argument <see cref="IControlArg"/> could do the
        /// encoding, or you can create your own specific encoder and inject it.
        /// </remarks>
        protected override Result<int> EncodePayload(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            SwInjectionRequest controlArg = (SwInjectionRequest)arg;
            if (buffer.Length < controlArg.Payload.Length + 4)
                return Result.FromException<int>(new DltEncodeException("'SwInjectionRequestEncoder' insufficient buffer"));

            BitOperations.Copy32Shift(controlArg.Payload.Length, buffer, !msbf);
            controlArg.Payload.AsSpan().CopyTo(buffer[4..]);
            return controlArg.Payload.Length + 4;
        }
    }
}
