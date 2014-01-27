using System;
using System.IO;
using System.IO.Compression;

namespace Forge.AutoUpdate.Tools
{
    /// <summary>
    /// (De)compresses a directory using the GZip algorithm.
    /// Original from http://www.codeproject.com/Tips/319438/How-to-Compress-Decompress-directories .
    /// </summary>
    public class DirectoryZipper
    {
        public delegate void ProgressDelegate(string sMessage);

        static void CompressFile(string directory, string relativePath, GZipStream zipStream)
        {
            CompressRelativePath(relativePath, zipStream);
            CompressFileContent(directory, relativePath, zipStream);
        }

        static void CompressRelativePath(string relativePath, GZipStream zipStream)
        {
            //store length of relative path for decompression
            zipStream.WriteAll(BitConverter.GetBytes(relativePath.Length));
            zipStream.WriteAll(ByteStringConverter.GetBytes(relativePath));
        }

        static void CompressFileContent(string directory, string relativePath, GZipStream zipStream)
        {
            var bytes = File.ReadAllBytes(Path.Combine(directory, relativePath));
            //store content length for decompression
            zipStream.WriteAll(BitConverter.GetBytes(bytes.Length));
            zipStream.WriteAll(bytes);
        }

        static bool DecompressFile(string directory, GZipStream zipStream, ProgressDelegate progress)
        {
            //Decompress file name
            var relativePathLengthBytes = new byte[sizeof(int)];
            var readBytes = zipStream.ReadFull(relativePathLengthBytes);
            if (readBytes < sizeof(int))
                return false;
            var relativePathLength = BitConverter.ToInt32(relativePathLengthBytes, 0);

            var relativePathBytes = new byte[relativePathLength * sizeof(char)];
            zipStream.ReadFull(relativePathBytes);
            var relativePath = ByteStringConverter.GetString(relativePathBytes);

            if (progress != null)
                progress(relativePath);

            //Decompress file content
            var contentLengthBytes = new byte[sizeof(int)];
            zipStream.ReadFull(contentLengthBytes);

            var contentBytes = new byte[BitConverter.ToInt32(contentLengthBytes, 0)];
            zipStream.ReadFull(contentBytes);

            var fullPath = Path.Combine(directory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (var newFile = File.Create(fullPath))
            {
                newFile.WriteAll(contentBytes);
            }

            return true;
        }

        /// <summary>
        /// Returns the relative path of fromPath to toPath (inside fromPath).
        /// Originates from http://stackoverflow.com/a/5891810 .
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        static string MakeRelativePath(string fromPath, string toPath)
        {
            // use Path.GetFullPath to canonicalise the paths (deal with multiple directory seperators, etc)
            return Path.GetFullPath(toPath).Substring(Path.GetFullPath(fromPath).Length + 1);
        }

        public static Stream CompressDirectory(string directory, ProgressDelegate progress = null)
        {
            var output = new MemoryStream();

            using (var zipStream = new GZipStream(output, CompressionMode.Compress, true))
            {
                foreach (var filePath in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
                {
                    var relativePath = MakeRelativePath(directory, filePath);

                    if (progress != null)
                    {
                        progress(relativePath);
                    }

                    CompressFile(directory, relativePath, zipStream);
                }
            }

            return output;
        }

        public static void DecompressToDirectory(Stream input, string sDir, ProgressDelegate progress = null)
        {
            using (var zipStream = new GZipStream(input, CompressionMode.Decompress, true))
            {
                while (DecompressFile(sDir, zipStream, progress)) ;
            }
        }
    }
}