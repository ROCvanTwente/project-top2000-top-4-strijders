using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;
using TemplateJwtProject.Models.DTOVanPlaylist;

namespace TemplateJwtProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _http;

        public PlayListController(AppDbContext context, IMemoryCache cache, HttpClient http)
        {
            _context = context;
            _cache = cache;
            _http = http;
        }

        [HttpPost("Create")]
        public IActionResult CreatePlaylist([FromBody] string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = new PlayList
            {
                Name = name,
                UserId = userId,
            };

            _context.PlayLists.Add(playlist);
            _context.SaveChanges();

            return Ok(200);
        }

        [HttpPost("add")]
        public IActionResult AddSongToPlaylist([FromBody] AddSongToPlaylistDto dto)
        {
            var songExists = _context.Songs.Any(s => s.SongId == dto.SongId);
            if (!songExists)
                return NotFound("Song not found");

            var playlistExists = _context.PlayLists.Any(p => p.Id == dto.PlaylistId);
            if (!playlistExists)
                return NotFound("Playlist not found");

            var alreadyExists = _context.PlayListSongs.Any(ps =>
                ps.SongId == dto.SongId &&
                ps.PlayListId == dto.PlaylistId);

            if (alreadyExists)
                return BadRequest("Song already in playlist");

            var playlistSong = new PlayListSong
            {
                SongId = dto.SongId,
                PlayListId = dto.PlaylistId
            };

            _context.PlayListSongs.Add(playlistSong);
            _context.SaveChanges();

            return Ok(playlistSong);
        }

        [HttpGet("retrieve")]
        public IActionResult RetrievePlaylist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = _context.PlayLists
                .Where(e => e.UserId == userId)
                .Select(e => new
                {
                    data = e
                });

            return Ok(playlist);
        }

        [HttpGet("{playlistId}/songs")]
        public async Task<IActionResult> GetSongsByPlaylist(int playlistId)
        {
            // 1️⃣ Haal songs + artist info via PlayListSong
            var songs = await _context.PlayListSongs
                .Where(ps => ps.PlayListId == playlistId)
                .Include(ps => ps.Song)           // join naar Song
                    .ThenInclude(s => s.Artist)  // join naar Artist
                .Select(ps => new PlaylistSongDto
                {
                    SongId = ps.Song.SongId,
                    Titel = ps.Song.Titel,
                    ReleaseYear = ps.Song.ReleaseYear,

                    ArtistId = ps.Song.Artist.ArtistId,
                    ArtistName = ps.Song.Artist.Name,
                    ArtistWiki = ps.Song.Artist.Wiki,
                    ArtistPhoto = ps.Song.Artist.Photo,

                    CoverUrl = null // vullen via iTunes
                })
                .ToListAsync();

            // 2️⃣ Parallel fetch voor covers met limiter
            var limiter = new SemaphoreSlim(5);

            var tasks = songs.Select(async s =>
            {
                await limiter.WaitAsync();
                try
                {
                    string cacheKey = $"cover_{s.SongId}";
                    if (!_cache.TryGetValue(cacheKey, out string coverUrl))
                    {
                        coverUrl = await FetchCoverFromITunes(s.Titel, s.ArtistName);
                        _cache.Set(cacheKey, coverUrl, TimeSpan.FromHours(6));
                    }
                    s.CoverUrl = coverUrl;
                    return s;
                }
                finally
                {
                    limiter.Release();
                }
            });

            var result = await Task.WhenAll(tasks);

            return Ok(result);
        }

        private async Task<string> FetchCoverFromITunes(string title, string artist)
        {
            try
            {
                var query = Uri.EscapeDataString($"{artist} {title}");
                var url = $"https://itunes.apple.com/search?term={query}&entity=song&limit=1";

                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("results");

                if (results.GetArrayLength() == 0)
                    return null;

                var artworkUrl = results[0]
                    .GetProperty("artworkUrl100")
                    .GetString()
                    .Replace("100x100", "600x600"); // hogere resolutie

                return artworkUrl;
            }
            catch
            {
                return null; // fallback als er iets misgaat
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveSongFromPlaylist([FromBody] RemoveSongFromPlaylistDto dto)
        {
            if (dto.Type == "playlist")
            {
                var playlist = await _context.PlayLists
                    .FirstOrDefaultAsync(s => s.Id == dto.PlaylistId);

                if (playlist == null)
                    return NotFound("playlist doesnt exist");

                _context.PlayLists.Remove(playlist);
                await _context.SaveChangesAsync();
                return Ok("playlist removed");
            }
            else
            {
                var playlistSong = await _context.PlayListSongs
                    .FirstOrDefaultAsync(ps =>
                        ps.PlayListId == dto.PlaylistId &&
                        ps.SongId == dto.SongId);

                if (playlistSong == null)
                    return NotFound("Song not found in playlist");

                _context.PlayListSongs.Remove(playlistSong);
                await _context.SaveChangesAsync();

                return Ok("Song removed from playlist");
            }
        }
    }
}