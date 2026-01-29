using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;

namespace TemplateJwtProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GetSongsController : Controller
{
    private readonly AppDbContext _context;

    public GetSongsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Songs>>> GetSongs()
    {
        List<Songs> allSongs = await _context.Songs
            .Include(s => s.Artist)
            .ToListAsync();
        return Ok(allSongs);
    }

    //api/GetSongs/artist/{artistId}/entriescount
    [HttpGet("artist/{artistId}/entriescount")]
    public async Task<ActionResult<List<SongEntriesCount>>> GetSongTop2000CountsByArtist(int artistId)
    {
        var artistExists = await _context.Artists.AnyAsync(a => a.ArtistId == artistId);
        if (!artistExists)
        {
            return NotFound("Artist not found");
        }

        var result = await _context.Top2000Entry
            .Where(te => te.Songs.ArtistId == artistId)
            .GroupBy(te => new
            {
                te.SongId,
                te.Songs.Titel,
                te.Songs.ReleaseYear
            })
            .Select(g => new SongEntriesCount
            {
                SongId = g.Key.SongId,
                Titel = g.Key.Titel,
                ReleaseYear = g.Key.ReleaseYear,
                TimesInTop2000 = g.Count()
            })
            .OrderByDescending(x => x.TimesInTop2000)
            .ToListAsync();

        return Ok(result);
    }

    // GET: api/GetSongs/5
    [HttpGet("{artistId}")]
    public async Task<ActionResult<List<Songs>>> GetSongsByArtist(int artistId)
    {
        var songs = await _context.Songs
            .Where(s => s.ArtistId == artistId)
            .Include(s => s.Artist)
            .ToListAsync();

        if (!songs.Any())
            return NotFound();

        return Ok(songs);
    }

}