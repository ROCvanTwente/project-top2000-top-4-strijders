using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using TemplateJwtProject.Constants;

namespace TemplateJwtProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EditController : Controller
{
    private readonly AppDbContext _context;

    public EditController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("EditSong")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> EditSong([FromBody] SongDto? songDto)
    {
        try
        {
            if (songDto != null)
            {
                Songs? song = await _context.Songs.FirstOrDefaultAsync(song => song.SongId == songDto.SongId);
                if (song != null)
                {
                    song.ReleaseYear = songDto.ReleaseYear;
                    song.Lyrics = songDto.Lyrics;
                    song.ImgUrl = songDto.ImgUrl;
                    song.Youtube = songDto.Youtube;
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(new {
                message = "Succesvol aangepast",
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("EditArtist")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> EditArtist([FromBody] ArtistDto? artistDto)
    {
        try
        {
            if (artistDto != null)
            {
                Artist? artist = await _context.Artists.FirstOrDefaultAsync(artist => artist.ArtistId == artistDto.ArtistId);
                if (artist != null)
                {
                    artist.Wiki = artistDto.Wiki;
                    artist.Biography = artistDto.Biography;
                    artist.WebsiteUrl = artistDto.WebsiteUrl;
                    artist.Photo = artistDto.Photo;
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(new {
                message = "Succesvol aangepast",
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}