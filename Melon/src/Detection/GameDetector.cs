using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    /// <summary>
    /// Detects supported Unity games and their IL2CPP/Mono type.
    /// Supports melonloader btd6, melonloader schedule 1, melonloader gorilla tag, melonloader bonelab.
    /// </summary>
    public static class GameDetector
    {
        private static readonly (string name, string exe, string framework)[] KNOWN_GAMES =
        {
            ("BTD6",          "BloonsTD6.exe",       "IL2CPP"),
            ("Schedule 1",    "Schedule I.exe",       "IL2CPP"),
            ("Gorilla Tag",   "Gorilla Tag.exe",      "IL2CPP"),
            ("Bonelab",       "BONELAB.exe",          "IL2CPP"),
            ("VRChat",        "VRChat.exe",            "IL2CPP"),
            ("Among Us",      "Among Us.exe",          "IL2CPP"),
            ("Boneworks",     "BONEWORKS.exe",         "IL2CPP"),
        };

        public static IEnumerable<(string name, string path, string framework)> FindInstalledGames()
        {
            var steamPath = GetSteamPath();
            if (steamPath == null) yield break;

            foreach (var lib in GetSteamLibraries(steamPath))
            {
                var common = Path.Combine(lib, "steamapps", "common");
                if (!Directory.Exists(common)) continue;

                foreach (var gameDir in Directory.GetDirectories(common))
                {
                    foreach (var (name, exe, fw) in KNOWN_GAMES)
                    {
                        if (File.Exists(Path.Combine(gameDir, exe)))
                            yield return (name, gameDir, fw);
                    }
                }
            }
        }

        private static string? GetSteamPath()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
                return key?.GetValue("InstallPath") as string;
            }
            catch { return null; }
        }

        private static IEnumerable<string> GetSteamLibraries(string steamPath)
        {
            yield return steamPath;
            var vdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) yield break;
            foreach (var line in File.ReadAllLines(vdf))
            {
                var t = line.Trim().Trim('"');
                if (Directory.Exists(t)) yield return t;
            }
        }
    }
}