using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class BackToFullUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cover_photo_relative_url",
                schema: "public",
                table: "categories",
                newName: "cover_photo_filename");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cover_photo_filename",
                schema: "public",
                table: "categories",
                newName: "cover_photo_relative_url");
        }
    }
}
