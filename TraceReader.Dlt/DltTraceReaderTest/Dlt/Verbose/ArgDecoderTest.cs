namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    internal static class ArgDecoderTest
    {

        public static void DecodeUnknown<T>(byte[] buffer, bool msbf, int encoding, int argType) where T : IVerboseArgDecoder
        {
            // Important is, even though .NET doesn't have a 128-bit integer type, we know how to decode it and present
            // it to the user.
            T decoder = Activator.CreateInstance<T>();
            int typeInfo = BitOperations.To32Shift(buffer, !msbf);
            int length = decoder.Decode(typeInfo, buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<UnknownVerboseDltArg>());

            UnknownVerboseDltArg unknownArg = (UnknownVerboseDltArg)arg;
            Assert.That(unknownArg.Data.Length, Is.EqualTo(buffer.Length - 4));
            Assert.That(unknownArg.Data, Is.EqualTo(buffer[4..]));
            Assert.That(unknownArg.IsBigEndian, Is.EqualTo(msbf));
            Assert.That(unknownArg.TypeInfo.Bytes, Is.EqualTo(buffer[0..4]));
            Assert.That(unknownArg.TypeInfo.Encoding, Is.EqualTo(encoding));
            Assert.That(unknownArg.TypeInfo.IsFixedPoint, Is.False);
            Assert.That(unknownArg.TypeInfo.IsVariableInfo, Is.False);
            Assert.That(unknownArg.TypeInfo.ArgumentType, Is.EqualTo(argType));
        }
    }
}
