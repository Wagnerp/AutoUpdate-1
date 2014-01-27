using System;

namespace Forge.AutoUpdate.Tools
{
    /// <summary>
    /// Helper for encoding-ignorant string-to-byte and byte-to-string conversion.
    /// See http://stackoverflow.com/a/10380166 .
    /// </summary>
    static class ByteStringConverter
    {
        internal static byte[] GetBytes(string intput)
        {
            var bytes = new byte[intput.Length * sizeof(char)];
            Buffer.BlockCopy(intput.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        internal static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}