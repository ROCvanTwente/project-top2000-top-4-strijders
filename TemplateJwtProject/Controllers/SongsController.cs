using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        public SongsController(AppDbContext db, IConfiguration config, HttpClient http)
        {
            _context = db;
            _config = config;
            _http = http;
        }

        [HttpGet("GetSongEntries")]
        public async Task<IActionResult> GetSongEntries(int id)
        {
            var entries = await _context.Top2000Entry
                .Where(e => e.SongId == id)
                .OrderBy(e => e.Position)
                .ToListAsync();

            var amountOfEntries = entries.Count; // number of entries

            return Ok(new { entries, amountOfEntries }); // return both if you want
        }

        [HttpGet("GetYoutubeVideoId")]
        public async Task<IActionResult> GetYoutubeVideoId(
        [FromQuery] string title,
        [FromQuery] string artist)
        {
            var apiKey = _config["YouTube:ApiKey"];
            var query = Uri.EscapeDataString($"{artist} {title}");

            var url =
                $"https://www.googleapis.com/youtube/v3/search" +
                $"?part=snippet&type=video&maxResults=1&q={query}&key={apiKey}";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return BadRequest("YouTube API error");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<YouTubeSearchResponse>(json);

            var videoId = data?.Items?.FirstOrDefault()?.Id?.VideoId;

            if (string.IsNullOrEmpty(videoId))
                return NotFound("No video found");

            return Ok(videoId);
        }

        [HttpGet("GetAlbumCover")]
        public async Task<IActionResult> GetAlbumCover(string title, string artist)
        {
            var query = Uri.EscapeDataString($"{artist} {title}");
            var url = $"https://itunes.apple.com/search?term={query}&entity=song&limit=1";

            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");

            if (results.GetArrayLength() == 0)
                return NotFound();

            var artworkUrl = results[0]
                .GetProperty("artworkUrl100")
                .GetString()
                .Replace("100x100", "600x600"); // hogere resolutie

            return Ok(artworkUrl);
        }

        [HttpGet("GetPositionsPerYear")]
        public async Task<IActionResult> GetPositionsPerYear(int songId)
        {
            var data = await _context.Top2000Entry
                .Where(e => e.SongId == songId)
                .OrderBy(e => e.Year)
                .Select(e => new
                {
                    year = e.Year,
                    position = e.Position
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}