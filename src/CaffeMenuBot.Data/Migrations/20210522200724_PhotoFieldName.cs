using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class PhotoFieldName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cover_photo_url",
                schema: "public",
                table: "categories",
                newName: "cover_photo_filename");

            migrationBuilder.RenameColumn(
                name: "ProfilePhotoFileName",
                table: "AspNetUsers",
                newName: "profile_photo_filename");

            migrationBuilder.AddColumn<string>(
                name: "CoverPhotoUrl",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "465e3dde-bc81-4650-9a9b-c00a072194f7");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ebbfc17a-bdf2-4173-b7be-cebdfa59f740", "AQAAAAEAACcQAAAAEBQbVHZwLTL/3HHo694zQRNHjoHhGvI2y3QFO1HOWVg6ZuqSkBFl1429qZD3S9KN3w==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPhotoUrl",
                schema: "public",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "cover_photo_filename",
                schema: "public",
                table: "categories",
                newName: "cover_photo_url");

            migrationBuilder.RenameColumn(
                name: "profile_photo_filename",
                table: "AspNetUsers",
                newName: "ProfilePhotoFileName");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "903f67e5-65b6-4ed7-afe3-fc9cb9744a05");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bbf1f3af-dce1-47f9-bcc7-992e95dc01f1", "AQAAAAEAACcQAAAAEIHCb2C5ZDrVJ4a5EmpJHAITODkqH/dKIBCYibYDwqke99Sfm+9pjXh2nr1ytHQadA==" });
        }
    }
}
