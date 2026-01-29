namespace TemplateJwtProject.Models
{
    public class PlayListSong
    {
        public int PlayListId { get; set; }
        public PlayList PlayList { get; set; }

        public int SongId { get; set; }
        public Songs Song { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
