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
        public async Task<ActionResult<List<Top2000Entry>>> Get100Entries(int year, int pagenition, string? artistName, bool allEntryOfYear = false)
        {
            int start = 1;
            int end = 2000;
			try
			{
                if (year >= 1999 && year < 2025)
                {
                    List<Top2000Entry> allEntries = new List<Top2000Entry>();
                    
                    if (allEntryOfYear)
                    {
                        allEntries = await _context.Top2000Entry
                            .Where(s => s.Year == year)
                            .Include(s => s.Songs)
                            .ThenInclude(s => s.Artist)
                            .OrderBy(s => s.Position)
                            .ToListAsync(); 
                        return Ok(allEntries);
                    }
                    
                    if (pagenition <= 1)
                    {
                        start = 1;
                        end = 100;
                    }
                    else if (pagenition <= 20)
                    {
                        start = (pagenition - 1) * 100;
                        end = pagenition * 100;
                    }
                    else
                    {
                        Console.WriteLine("mag niet meer dan 20");
                        return BadRequest("mag niet meer dan 20");
                    }
                    
                    if (artistName == null)
                    {
                        allEntries = await _context.Top2000Entry
                            .Where(s => s.Year == year && (s.Position >= start && s.Position <= end))
                            .Include(s => s.Songs)
                            .ThenInclude(s => s.Artist)
                            .OrderBy(s => s.Position)
                            .ToListAsync();
                    }
                    else
                    {
                        allEntries = await _context.Top2000Entry
                            .Include(s => s.Songs)
                            .ThenInclude(s => s.Artist)
                            .Where(s => s.Year == year && s.Songs.Artist.Name == artistName)
                            .OrderBy(s => s.Position)
                            .ToListAsync();
                    }
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