using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateJwtProject.Migrations
{
    public partial class StijgersStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE dbo.Stijgers
                @Year INT
            AS
            BEGIN
                SET NOCOUNT ON

                SELECT
                    e.SongId,
                    s.Titel        AS Title,
                    a.Name         AS ArtistName,
                    s.ReleaseYear,
                    e.Position,
                    ePrev.Position AS PositionYearBefore,
                    (ePrev.Position - e.Position) AS Gestegen
                FROM Top2000Entry e
                INNER JOIN Top2000Entry ePrev
                    ON e.SongId = ePrev.SongId
                    AND ePrev.Year = 2020 - 1
                INNER JOIN Songs s
                    ON e.SongId = s.SongId
                INNER JOIN Artists a
                    ON s.ArtistId = a.ArtistId
                WHERE e.Year = 2020
                  AND e.Position < ePrev.Position
                ORDER BY Gestegen DESC;
            END
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS dbo.Stijgers");
        }
    }
}
