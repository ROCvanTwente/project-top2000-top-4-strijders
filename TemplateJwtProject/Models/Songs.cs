using System.ComponentModel.DataAnnotations;

namespace TemplateJwtProject.Models
{
    public class Songs
    {
        [Key]
        public int SongId { get; set; }

        [Required]
        public string Titel { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        public string? ImgUrl { get; set; }
        public string? Lyrics { get; set; }
        public string? Youtube { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }

        public ICollection<PlayListSong> PlayListSongs { get; set; } = new List<PlayListSong>();
    }
}