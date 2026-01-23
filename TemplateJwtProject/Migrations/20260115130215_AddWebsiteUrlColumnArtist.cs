using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateJwtProject.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteUrlColumnArtist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Artist",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Artist");
        }
    }
}
