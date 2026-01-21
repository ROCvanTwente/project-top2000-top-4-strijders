namespace TemplateJwtProject.Models.DTOVanStatistieken;

public class ArtistWithMostSongsOnYearDto
{
    public int ArtistId { get; set; }
    public string ArtistName { get; set; }
    public int TotalSongs { get; set; }
    public int Average { get; set; }
    public int Highest { get; set; }
}