using System.Net.Http;
using System.Text.Json;

namespace TemplateJwtProject.Services
{
    public class WikipediaData
    {
        public string? Extract { get; set; }
        public string? WikiUrl { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class WikipediaService
    {
        private readonly HttpClient _httpClient;

        public WikipediaService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // User-Agent required
            if (!_httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(
                "TemplateJwtProject/1.0 (dev@example.com)"))
            {
                throw new Exception("Invalid User-Agent header");
            }
        }

        public async Task<WikipediaData?> GetArtistDataAsync(string artistName)
        {
            if (string.IsNullOrWhiteSpace(artistName))
                return null;

            try
            {
                var urlName = Uri.EscapeDataString(artistName);
                var url = $"https://nl.wikipedia.org/api/rest_v1/page/summary/{urlName}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                string? extract = root.TryGetProperty("extract", out var extractEl) ? extractEl.GetString() : null;
                string? wikiUrl = root.TryGetProperty("content_urls", out var urlsEl) &&
                                   urlsEl.TryGetProperty("desktop", out var desktopEl) &&
                                   desktopEl.TryGetProperty("page", out var pageEl)
                                   ? pageEl.GetString()
                                   : null;
                string? photoUrl = root.TryGetProperty("originalimage", out var imgEl) &&
                                   imgEl.TryGetProperty("source", out var srcEl)
                                   ? srcEl.GetString()
                                   : null;

                return new WikipediaData
                {
                    Extract = extract,
                    WikiUrl = wikiUrl,
                    PhotoUrl = photoUrl
                };
            }
            catch
            {
                return null;
            }
        }
    }
    }
