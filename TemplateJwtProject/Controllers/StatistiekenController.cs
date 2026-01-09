using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatistiekenController : Controller
{
    private readonly AppDbContext _context;

    public StatistiekenController(AppDbContext context)
    {
        _context = context;
    }
}