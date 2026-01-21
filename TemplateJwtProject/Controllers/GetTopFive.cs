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
        public async Task<List<Top2000Entry>> Get()
        {
            try
            {
                var items = await _context.Top2000Entry
                .Where(e => e.Year == 2024)
                .Include(e => e.Songs)
                .ThenInclude(s => s.Artist)
                .OrderBy(e => e.Position)
                .Take(5)
                .ToListAsync();

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Probleem met database");
                return new List<Top2000Entry>();
            }
        }
    }
}