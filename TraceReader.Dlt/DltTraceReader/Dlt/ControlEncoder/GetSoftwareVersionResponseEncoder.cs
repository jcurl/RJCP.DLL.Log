namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using RJCP.Core;
    using Text;

    /// <summary>
    /// Encode a <see cref="GetSoftwareVersionResponse"/>.
    /// </summary>
    public sealed class GetSoftwareVersionResponseEncoder : ControlArgResponseEncoder
    {
        /// <summary>
        /// Encodes the payload, immediately after the service identifier and the status.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        protected override int EncodePayload(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            if (arg.Status != ControlResponse.StatusOk) return 0;
            if (buffer.Length < 5) return -1;

            GetSoftwareVersionResponse controlArg = (GetSoftwareVersionResponse)arg;
            int bu = Iso8859_1.Convert(controlArg.SwVersion, buffer[4..]);
            if (bu >= buffer.Length - 4) return -1;

            BitOperations.Copy32Shift(bu + 1, buffer[0..4], !msbf);
            buffer[bu + 4] = 0;
            return 5 + bu;
        }
    }
}
