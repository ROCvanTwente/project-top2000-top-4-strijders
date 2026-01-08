using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTop2000EntriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GetTop2000EntriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Top2000Entry>>> GetYearEntries(int year)
        {
			try
			{
                if (year >= 1999 && year < 2025)
                {
                    List<Top2000Entry> allEntries = await _context.Top2000Entry
                        .Where(s => s.Year == year)
                        .Include(s => s.Songs)
                        .ThenInclude(s => s.Artist)
                        .OrderBy(s => s.Position)
                        .ToListAsync(); 
                    return Ok(allEntries);
                }
                else
                {
                    Console.WriteLine("jaar moet tussen de 1999 en 2024 zijn");
                    return BadRequest("jaar moet tussen de 1999 en 2024 zijn");
                }
			}
            catch (Exception ex)
            {
                Console.WriteLine("Probleem met de database");
                return BadRequest(ex);
            }
        }
    }
}