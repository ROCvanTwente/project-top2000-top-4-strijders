namespace TemplateJwtProject.Models.DTOVanPlaylist
{
    public class RemoveSongFromPlaylistDto
    {
        public int PlaylistId { get; set; }
        public int? SongId { get; set; }
        public string Type { get; set; }
    }
}
