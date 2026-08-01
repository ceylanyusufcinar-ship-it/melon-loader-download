using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MelonLoaderDownloader
{
    /// <summary>
    /// Application settings for MelonLoader Downloader.
    /// melonloader installer — saved user preferences.
    /// </summary>
    public class AppSettings
    {
        [JsonPropertyName("lastGamePath")]
        public string LastGamePath { get; set; } = string.Empty;

        [JsonPropertyName("customSteamPaths")]
        public List<string> CustomSteamPaths { get; set; } = new();

        [JsonPropertyName("autoUpdate")]
        public bool AutoUpdate { get; set; } = true;

        [JsonPropertyName("lastInstalledVersion")]
        public string LastInstalledVersion { get; set; } = string.Empty;
    }
}
