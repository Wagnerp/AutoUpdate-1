using System;
using System.IO;
using System.Linq;

namespace Forge.AutoUpdate.Tools
{
    public static class VersionHelper
    {
        public static Version[] ParseVersionsFromSubDirectoryNamesOf(string path)
        {
            return Directory.Exists(path) ?
                new DirectoryInfo(path)
                    .GetDirectories()
                    .Select(d => TryParseVersion(d.Name))
                    .Where(v => v != null)
                    .ToArray() :
                new Version[0];
        }

        static Version TryParseVersion(string input)
        {
            Version version;
            return Version.TryParse(input, out version) ?
                version :
                null;
        }
    }
}