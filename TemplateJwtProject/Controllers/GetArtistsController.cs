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

    [HttpGet]
    public async Task<ActionResult<List<Artist>>> GetArtists()
    {
        List<Artist> allArtist = await _context.Artists.ToListAsync();
        return Ok(allArtist);
    }
}