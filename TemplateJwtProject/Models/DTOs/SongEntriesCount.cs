namespace TemplateJwtProject.Models.DTOs
{
    public class SongEntriesCount
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int TimesInTop2000 { get; set; }
    }
}
