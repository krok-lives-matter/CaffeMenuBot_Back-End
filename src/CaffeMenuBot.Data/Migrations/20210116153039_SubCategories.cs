using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class SubCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                schema: "public",
                table: "categories",
                type: "integer",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ParentCategoryId",
                schema: "public",
                table: "categories");
        }
    }
}
