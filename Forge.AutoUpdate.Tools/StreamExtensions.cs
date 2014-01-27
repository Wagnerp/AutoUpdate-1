using System.IO;

namespace Forge.AutoUpdate.Tools
{
    //TODO add XML doc
    public static class StreamExtensions
    {
        public static void Rewind(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void WriteAll(this Stream stream, byte[] bytes)
        {
            if (stream.CanWrite)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public static int ReadFull(this Stream stream, byte[] bytes)
        {
            return stream.CanRead ?
                stream.Read(bytes, 0, bytes.Length) :
                default(int);
        }
    }
}