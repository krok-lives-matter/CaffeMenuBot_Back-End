using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class PhotosFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPhotoUrl",
                schema: "public",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "cover_photo_filename",
                schema: "public",
                table: "categories",
                newName: "cover_photo_relative_url");

            migrationBuilder.RenameColumn(
                name: "profile_photo_filename",
                table: "AspNetUsers",
                newName: "profile_photo_relative_url");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "590c5b5e-d5e6-4b58-8711-855422b21035");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "profile_photo_relative_url" },
                values: new object[] { "57b0948b-fbbd-4795-9b50-fec7b055c96a", "AQAAAAEAACcQAAAAEOUxPeFGxI10VSmrte7P8nu205iUfxMzChIeiBkAK6tLVURJcLzhPCGQj1EOO6cU8Q==", "profile_photos\\blank.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cover_photo_relative_url",
                schema: "public",
                table: "categories",
                newName: "cover_photo_filename");

            migrationBuilder.RenameColumn(
                name: "profile_photo_relative_url",
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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "profile_photo_filename" },
                values: new object[] { "ebbfc17a-bdf2-4173-b7be-cebdfa59f740", "AQAAAAEAACcQAAAAEBQbVHZwLTL/3HHo694zQRNHjoHhGvI2y3QFO1HOWVg6ZuqSkBFl1429qZD3S9KN3w==", "blank.jpg" });
        }
    }
}
