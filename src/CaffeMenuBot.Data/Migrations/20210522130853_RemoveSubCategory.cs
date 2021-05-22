using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class RemoveSubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                schema: "public",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentCategoryId",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "photo_url",
                schema: "public",
                table: "dishes");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                schema: "public",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "cover_photo_url",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_visible",
                schema: "public",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "f0e8006a-f33b-4624-898f-4f14caaf8f1b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f14a588c-bcae-4466-a0da-398c116e54f9", "AQAAAAEAACcQAAAAEBnEYmB9JswRIU4jP1+igbsHmZ2e+K6l6gY6PB7m+S8UwmajFFIf6CqXh3+WKIRTtQ==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cover_photo_url",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "is_visible",
                schema: "public",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                schema: "public",
                table: "dishes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                schema: "public",
                table: "categories",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "b8b9cca2-b6b0-4f78-9984-dbec49a2ac79");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "727a549f-3d4a-4cae-b7f4-52805d086c88", "AQAAAAEAACcQAAAAEBYUIdVzZ+2v/8OjLY5beMvTwHlU1QRtRLZvII+Cb1O3VMBw4BP2u1CE6+Zw9GLhnA==" });

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentCategoryId",
                schema: "public",
                table: "categories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                schema: "public",
                table: "categories",
                column: "ParentCategoryId",
                principalSchema: "public",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
