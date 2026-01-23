using Microsoft.AspNetCore.Identity;

namespace TemplateJwtProject.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<PlayList> PlayLists { get; set; } = new List<PlayList>();
}