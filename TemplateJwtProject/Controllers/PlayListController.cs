using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public IActionResult CreatePlaylist([FromBody] string name)
        {
            var playlist = new PlayList
            {
                Name = name,
                UserId = "097E07E0-44B8-43FC-B049-7B92AF5FA34F"
            };

            _context.PlayLists.Add(playlist);
            _context.SaveChanges();

            return Ok(playlist);
        }
    }
}