using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;

namespace MelonLoaderDownloader
{
    /// <summary>
    /// Downloads and installs MelonLoader.
    /// Handles: melonloader download github, melonloader downloader, melonloader download vr.
    /// </summary>
    public class MelonInstaller
    {
        private static readonly HttpClient Http = new HttpClient();
        private const string RELEASE_API = "https://api.github.com/repos/LavaGang/MelonLoader/releases/latest";

        static MelonInstaller()
        {
            Http.DefaultRequestHeaders.Add("User-Agent", "MelonLoaderDownloader/0.7.4");
        }

        public async Task<(string version, string url)> GetLatestReleaseAsync()
        {
            var json = await Http.GetStringAsync(RELEASE_API);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            var tag  = root.GetProperty("tag_name").GetString() ?? "v0.7.4";

            foreach (var asset in root.GetProperty("assets").EnumerateArray())
            {
                var name = asset.GetProperty("name").GetString() ?? "";
                if (name.Contains("MelonLoader.zip") && !name.Contains("Mono"))
                {
                    var url = asset.GetProperty("browser_download_url").GetString() ?? "";
                    return (tag, url);
                }
            }
            throw new Exception("MelonLoader ZIP not found in release assets");
        }

        public async Task InstallAsync(string gamePath, string melonZipUrl, IProgress<(string msg, int pct)>? progress = null)
        {
            progress?.Report(("Downloading MelonLoader…", 10));
            var zipBytes = await Http.GetByteArrayAsync(melonZipUrl);

            progress?.Report(("Extracting…", 50));
            using var ms  = new MemoryStream(zipBytes);
            using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                var dest = Path.Combine(gamePath, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                entry.ExtractToFile(dest, overwrite: true);
            }

            var modsDir = Path.Combine(gamePath, "Mods");
            Directory.CreateDirectory(modsDir);

            progress?.Report(("MelonLoader installed successfully!", 100));
        }
    }
}