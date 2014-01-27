using System.IO;
using Forge.AutoUpdate.Scheduler.DistributionService;
using Forge.AutoUpdate.Scheduler.Properties;
using Forge.AutoUpdate.Tools;

namespace Forge.AutoUpdate.Scheduler
{
    //TODO provide progress information
    //TODO add strategy: download of diff between current and latest version?
    //TODO enable download of multiple products
    public sealed class UpdateDownloader
    {
        readonly UpdateDistributorClient client;

        public UpdateDownloader()
        {
            client = new UpdateDistributorClient();
            Directory.CreateDirectory(GetProductDownloadFolder());
        }

        public void DownloadLatestVersion()
        {
            var latestVersion = client
                .GetAvailableVersions(Settings.Default.ProductName)
                .GetLatest();

            if (latestVersion == null)
            {
                return;
            }

            var productDownloadFolder = GetProductDownloadFolder();
            var latestDownloadedVersion = VersionDirectoryParser
                .ParseVersionsFromSubDirectoryNamesOf(productDownloadFolder)
                .GetLatest();

            if (latestDownloadedVersion == null || latestDownloadedVersion < latestVersion)
            {
                using (var downloadStream = client.GetVersion(Settings.Default.ProductName, latestVersion))
                {
                    DirectoryZipper.DecompressToDirectory(
                        downloadStream,
                        Path.Combine(
                            productDownloadFolder,
                            latestVersion.ToString()));
                }
            }
        }

        static string GetProductDownloadFolder()
        {
            return Path.Combine(
                Settings.Default.DownloadFolder,
                Settings.Default.ProductName);
        }
    }
}