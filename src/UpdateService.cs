using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    /// <summary>
    /// Checks for updates to the MelonLoader Downloader itself.
    /// melonloader wiki and version management.
    /// </summary>
    public class UpdateService
    {
        private const string ApiUrl =
            "https://api.github.com/repos/melonloader-IL2CPP/melon-loader-download/releases/latest";

        public record ReleaseInfo(string TagName, string DownloadUrl, string Body);

        public static async Task<ReleaseInfo?> GetLatestAsync()
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("MelonLoaderDownloader");
            var json = await http.GetStringAsync(ApiUrl);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var tag = root.GetProperty("tag_name").GetString() ?? string.Empty;
            var body = root.GetProperty("body").GetString() ?? string.Empty;
            var url = root.GetProperty("assets")[0]
                         .GetProperty("browser_download_url").GetString() ?? string.Empty;
            return new ReleaseInfo(tag, url, body);
        }
    }
}
