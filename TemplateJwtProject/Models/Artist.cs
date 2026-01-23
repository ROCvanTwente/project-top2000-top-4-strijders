using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemplateJwtProject.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Wiki {  get; set; }
        public string? Biography { get; set; }
        public string? WebsiteUrl {  get; set; }
        public string? Photo {  get; set; }

    }
}