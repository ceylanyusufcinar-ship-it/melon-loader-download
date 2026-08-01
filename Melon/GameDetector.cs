// melonloader btd6, gorilla tag, schedule 1, bonelab — game auto-detection
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace MelonLoaderDownloader
{
    public class DetectedGame
    {
        public string Name { get; set; } = string.Empty;
        public string ExePath { get; set; } = string.Empty;
        public bool IsIl2Cpp { get; set; }
        public string Backend => IsIl2Cpp ? "IL2CPP" : "Mono";
    }

    /// <summary>
    /// Scans Steam/Epic libraries for known MelonLoader-compatible Unity games.
    /// melonloader gorilla tag, btd6, schedule 1, bonelab detection.
    /// </summary>
    public class GameDetector
    {
        private static readonly Dictionary<string, string> KnownGames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Gorilla Tag"] = "Gorilla Tag/GorillaTag.exe",
            ["BTD6"] = "BloonsTD6/BloonsTD6.exe",
            ["Bonelab"] = "BONELAB/BONELAB.exe",
            ["Schedule 1"] = "Schedule1/Schedule1.exe",
            ["VRChat"] = "VRChat/VRChat.exe",
            ["Among Us"] = "Among Us/Among Us.exe",
            ["Boneworks"] = "BONEWORKS/BONEWORKS.exe",
        };

        public List<DetectedGame> Detect()
        {
            var found = new List<DetectedGame>();

            foreach (var steamLib in GetSteamLibraries())
            {
                foreach (var (name, rel) in KnownGames)
                {
                    var full = Path.Combine(steamLib, "steamapps", "common", rel);
                    if (File.Exists(full))
                        found.Add(new DetectedGame
                        {
                            Name = name,
                            ExePath = full,
                            IsIl2Cpp = Installer.IsIl2CppGame(Path.GetDirectoryName(full)!)
                        });
                }
            }

            return found;
        }

        private static IEnumerable<string> GetSteamLibraries()
        {
            var roots = new List<string>();
            var steamKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            var steamPath = steamKey?.GetValue("SteamPath")?.ToString();
            if (steamPath != null)
            {
                roots.Add(steamPath);
                var libraryFolders = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(libraryFolders))
                {
                    foreach (var line in File.ReadAllLines(libraryFolders))
                    {
                        var trimmed = line.Trim().Trim('"');
                        if (Directory.Exists(trimmed)) roots.Add(trimmed);
                    }
                }
            }
            return roots;
        }
    }
}
