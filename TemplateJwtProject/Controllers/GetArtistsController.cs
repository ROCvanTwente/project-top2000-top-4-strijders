using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GetArtistsController : Controller
{
    private readonly AppDbContext _context;

    public GetArtistsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/GetArtists
    [HttpGet]
    public async Task<ActionResult<List<Artist>>> GetArtists()
    {
        List<Artist> allArtist = await _context.Artists.ToListAsync();
        return Ok(allArtist);
    }

    // GET: api/GetArtists/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Artist>> GetArtistById(int id)
    {
        var artist = await _context.Artists
            .FirstOrDefaultAsync(a => a.ArtistId == id);

        if (artist == null)
            return NotFound();

        return Ok(artist);
    }
}