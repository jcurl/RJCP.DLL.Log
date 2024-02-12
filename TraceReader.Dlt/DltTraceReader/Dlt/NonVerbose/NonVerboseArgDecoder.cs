namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Collections.Generic;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decodes any type of PDU from Non-Verbose.
    /// </summary>
    public class NonVerboseArgDecoder : INonVerboseArgDecoder
    {
        private readonly Dictionary<string, INonVerboseArgDecoder> m_Decoders;
        private readonly INonVerboseArgDecoder m_UnknownArgDecoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonVerboseArgDecoder"/> class.
        /// </summary>
        public NonVerboseArgDecoder()
        {
            m_UnknownArgDecoder = new NonVerboseUnknownArgDecoder();

            m_Decoders = new Dictionary<string, INonVerboseArgDecoder> {
                { "S_BOOL", new NonVerboseBoolArgDecoder()},
                { "S_SINT8", new NonVerboseSignedIntArgDecoder(1) },
                { "S_SINT16", new NonVerboseSignedIntArgDecoder(2) },
                { "S_SINT32", new NonVerboseSignedIntArgDecoder(4) },
                { "S_SINT64", new NonVerboseSignedIntArgDecoder(8) },
                { "S_UINT8", new NonVerboseUnsignedIntArgDecoder(1) },
                { "S_UINT16", new NonVerboseUnsignedIntArgDecoder(2) },
                { "S_UINT32", new NonVerboseUnsignedIntArgDecoder(4) },
                { "S_UINT64", new NonVerboseUnsignedIntArgDecoder(8) },
                { "S_FLOA32", new NonVerboseFloat32ArgDecoder() },
                { "S_FLOA64", new NonVerboseFloat64ArgDecoder() },
                { "S_RAW", new NonVerboseRawArgDecoder() },
                { "S_RAWD", new NonVerboseRawArgDecoder() },
                { "S_STRG_ASCII", new NonVerboseStringArgDecoder(StringEncodingType.Ascii) },
                { "S_STRG_UTF8", new NonVerboseStringArgDecoder(StringEncodingType.Utf8) },
                { "S_UTF8", new NonVerboseStringArgDecoder(StringEncodingType.Utf8) },
                { "S_BIN8", new NonVerboseBinArgDecoder(1) },
                { "S_BIN16", new NonVerboseBinArgDecoder(2) },
                { "S_BIN32", new NonVerboseBinArgDecoder(4) },
                { "S_BIN64", new NonVerboseBinArgDecoder(8) },
                { "S_HEX8", new NonVerboseHexArgDecoder(1) },
                { "S_HEX16", new NonVerboseHexArgDecoder(2) },
                { "S_HEX32", new NonVerboseHexArgDecoder(4) },
                { "S_HEX64", new NonVerboseHexArgDecoder(8) }
            };
        }

        /// <summary>
        /// Registers the specified non verbose argument type decoder.
        /// </summary>
        /// <param name="pduType">The type of the non verbose argument.</param>
        /// <param name="decoder">The decoder for the non verbose argument type.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pduType"/> is <see langword="null"/>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="decoder"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="pduType"/> has already been registered.</exception>
        protected void Register(string pduType, INonVerboseArgDecoder decoder)
        {
            ArgumentNullException.ThrowIfNull(pduType);
            ArgumentNullException.ThrowIfNull(decoder);

            m_Decoders.Add(pduType, decoder);
        }

        /// <summary>
        /// Unregisters the specified non verbose argument type decoder.
        /// </summary>
        /// <param name="pduType">The type of the non verbose argument.</param>
        /// <returns>
        /// <see langword="true"/> if the decoder for the specified non verbose argument type has been successfully
        /// unregistered, <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="pduType"/> is <see langword="null"/>.</exception>
        protected bool Unregister(string pduType)
        {
            ArgumentNullException.ThrowIfNull(pduType);

            return m_Decoders.Remove(pduType);
        }

        /// <summary>
        /// Decodes the DLT non verbose argument.
        /// </summary>
        /// <param name="buffer">The buffer where the encoded DLT non verbose argument can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="pdu">The Packet Data Unit instance representing the argument structure.</param>
        /// <param name="arg">On output, the decoded argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public Result<int> Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (pdu.Description is object) {
                arg = new StringDltArg(pdu.Description);
                return 0;
            }

            if (m_Decoders.TryGetValue(pdu.PduType, out INonVerboseArgDecoder decoder))
                return decoder.Decode(buffer, msbf, pdu, out arg);

            return m_UnknownArgDecoder.Decode(buffer, msbf, pdu, out arg);
        }
    }
}
