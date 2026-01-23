namespace TemplateJwtProject.Models
{
    public class PlayList
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<PlayListSong> PlayListSongs { get; set; } = new List<PlayListSong>();
    }
}