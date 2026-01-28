using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTopFive : ControllerBase
    {
        private readonly AppDbContext _context;

        public GetTopFive(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Top2000Entry>>> Get(int year)
        {
            try
            {
                if (year < 1999 || year > 2025)
                {
                    Console.WriteLine("jaar moet tussen de 1999 en 2025 zijn");
                    return BadRequest("jaar moet tussen de 1999 en 2025 zijn");
                }

                var items = await _context.Top2000Entry
                .Where(e => e.Year == year)
                .Include(e => e.Songs)
                .ThenInclude(s => s.Artist)
                .OrderBy(e => e.Position)
                .Take(5)
                .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Probleem met database");
                return StatusCode(500, "Er is een probleem opgetreden bij het ophalen van de data");
            }
        }
    }
}