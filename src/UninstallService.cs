using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    /// <summary>
    /// Handles uninstalling and updating MelonLoader from a game directory.
    /// melonloader installer — remove or upgrade existing installs.
    /// </summary>
    public class UninstallService
    {
        private static readonly string[] MelonFiles =
        {
            "version.dll",
            "MelonLoader",
            "Mods",
            "UserLibs",
            "dobby.dll",
        };

        public static void Uninstall(string gameDirectory)
        {
            foreach (var entry in MelonFiles)
            {
                var full = Path.Combine(gameDirectory, entry);
                if (File.Exists(full)) File.Delete(full);
                else if (Directory.Exists(full)) Directory.Delete(full, recursive: true);
            }
        }

        public static async Task UpdateAsync(string gameDirectory, IProgress<int> progress)
        {
            Uninstall(gameDirectory);
            bool isIl2Cpp = Installer.IsIl2CppGame(gameDirectory);
            await new Installer().InstallAsync(gameDirectory, progress);
        }
    }
}
