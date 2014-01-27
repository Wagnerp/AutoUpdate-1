using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Forge.AutoUpdate.Service.Properties;
using Forge.AutoUpdate.Tools;

namespace Forge.AutoUpdate.Service
{
    [ServiceContract]
    public interface IUpdateDistributor
    {
        [OperationContract]
        Version[] GetAvailableVersions(string productName);
        [OperationContract]
        Stream GetVersion(string productName, Version version);
    }

    public class UpdateDistributor : IUpdateDistributor
    {
        public Version[] GetAvailableVersions(string productName)
        {
            return VersionHelper.ParseVersionsFromSubDirectoryNamesOf(GetProductPackageFolder(productName));
        }

        public Stream GetVersion(string productName, Version version)
        {
            var versionDirectory = Directory
                .GetDirectories(
                    GetProductPackageFolder(productName),
                    version.ToString())
                .SingleOrDefault();

            var output = versionDirectory == null ?
                Stream.Null :
                DirectoryZipper.CompressDirectory(versionDirectory);

            output.Reset();
            return output;
        }

        static string GetProductPackageFolder(string productName)
        {
            return Path.Combine(
                Settings.Default.PackageFolder,
                productName);
        }
    }
}