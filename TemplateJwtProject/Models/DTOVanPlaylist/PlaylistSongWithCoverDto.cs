namespace TemplateJwtProject.Models.DTOVanPlaylist
{
    public class PlaylistSongDto
    {
        public int SongId { get; set; }
        public string Titel { get; set; }
        public int ReleaseYear { get; set; }

        // Artist info
        public int ArtistId { get; set; }

        public string ArtistName { get; set; }
        public string ArtistWiki { get; set; }
        public string ArtistPhoto { get; set; }  // optioneel

        public string CoverUrl { get; set; } // null voorlopig
    }
}