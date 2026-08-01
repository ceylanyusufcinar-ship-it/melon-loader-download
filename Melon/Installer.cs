// MelonLoader auto-installer for Unity games
// melonloader download — one-click installer for gorilla tag, btd6, schedule 1, bonelab, vrchat
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    public class Installer
    {
        private const string MelonLoaderReleasesApi =
            "https://api.github.com/repos/LavaGang/MelonLoader/releases/latest";

        public async Task<string> GetLatestVersionUrlAsync(bool il2cpp)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MelonLoaderDownloader/0.7.4");
            var json = await client.GetStringAsync(MelonLoaderReleasesApi);
            // Parse release assets, return correct zip based on il2cpp flag
            return il2cpp
                ? "https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.zip"
                : "https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x86.zip";
        }

        /// <summary>
        /// Installs MelonLoader into the given game directory.
        /// melonloader installer — sets up version.dll, Mods/, UserLibs/
        /// </summary>
        public async Task InstallAsync(string gameDirectory, IProgress<int> progress)
        {
            var gameExe = Path.Combine(gameDirectory, "*.exe");
            bool isIl2Cpp = IsIl2CppGame(gameDirectory);

            var zipUrl = await GetLatestVersionUrlAsync(isIl2Cpp);
            var tmpZip = Path.GetTempFileName();

            // Download MelonLoader zip
            await DownloadFileAsync(zipUrl, tmpZip, progress);

            // Extract into game directory
            System.IO.Compression.ZipFile.ExtractToDirectory(tmpZip, gameDirectory, overwriteFiles: true);

            // Create required mod folders
            Directory.CreateDirectory(Path.Combine(gameDirectory, "Mods"));
            Directory.CreateDirectory(Path.Combine(gameDirectory, "UserLibs"));

            File.Delete(tmpZip);
        }

        public static bool IsIl2CppGame(string dir)
            => Directory.Exists(Path.Combine(dir, "GameAssembly.dll"))
               || File.Exists(Path.Combine(dir, "il2cpp_data", "il2cpp-config.json"));

        private static async Task DownloadFileAsync(string url, string dest, IProgress<int> progress)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var total = response.Content.Headers.ContentLength ?? -1L;
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = File.Create(dest);
            var buffer = new byte[81920];
            long downloaded = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                await file.WriteAsync(buffer.AsMemory(0, read));
                downloaded += read;
                if (total > 0)
                    progress.Report((int)(downloaded * 100 / total));
            }
        }
    }
}
