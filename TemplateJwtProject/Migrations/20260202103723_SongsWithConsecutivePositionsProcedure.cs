using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateJwtProject.Migrations
{
    /// <inheritdoc />
    public partial class SongsWithConsecutivePositionsProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE SongsWithConsecutivePositions
                    @Year INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                
                    -- Selecteer songs met opeenvolgende posities voor dezelfde artiest
                    ;WITH OrderedEntries AS
                    (
                        SELECT
                            e.Position,
                            s.SongId,
                            s.Titel AS Title,
                            s.ReleaseYear,
                            a.ArtistId,
                            a.Name AS ArtistName,
                            LEAD(a.ArtistId) OVER (ORDER BY e.Position) AS NextArtistId,
                            LEAD(s.SongId) OVER (ORDER BY e.Position) AS NextSongId,
                            LEAD(s.Titel) OVER (ORDER BY e.Position) AS NextSongTitle,
                            LEAD(s.ReleaseYear) OVER (ORDER BY e.Position) AS NextReleaseYear,
                            LEAD(e.Position) OVER (ORDER BY e.Position) AS NextPosition
                        FROM Top2000Entry e
                        INNER JOIN Songs s ON e.SongId = s.SongId
                        INNER JOIN Artists a ON s.ArtistId = a.ArtistId
                        WHERE e.Year = @Year
                    )
                    SELECT 
                        SongId,
                        Position,
                        ArtistName,
                        Title,
                        ReleaseYear
                    FROM OrderedEntries
                    WHERE ArtistId = NextArtistId
                
                    UNION ALL
                
                    SELECT 
                        NextSongId AS SongId,
                        NextPosition AS Position,
                        ArtistName,
                        NextSongTitle AS Title,
                        NextReleaseYear AS ReleaseYear
                    FROM OrderedEntries
                    WHERE ArtistId = NextArtistId
                    ORDER BY Position;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SongsWithConsecutivePositions;");
        }
    }
}
