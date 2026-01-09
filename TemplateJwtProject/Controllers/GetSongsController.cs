using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

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
}