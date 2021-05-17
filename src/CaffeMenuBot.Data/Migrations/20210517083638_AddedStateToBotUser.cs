using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffeMenuBot.Data.Migrations
{
    public partial class AddedStateToBotUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_chat_state",
                schema: "public",
                table: "bot_users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e575", "1cce92a1-5095-4ee9-82f0-044250fd8f5a", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e575", 0, "782534cf-92a8-4630-ad52-0ec01737db71", "admin@caffemenubot.com", true, false, null, "ADMIN@CAFFEMENUBOT.COM", "ADMIN", "AQAAAAEAACcQAAAAENxHZTYsgDb9EINSi6vudXtBD6+rc4g3HBcHa/9aRaThToAuJibNzSy1WCkNwMHXhg==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e575", "a18be9c0-aa65-4af8-bd17-00bd9344e575" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e575", "a18be9c0-aa65-4af8-bd17-00bd9344e575" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575");

            migrationBuilder.DropColumn(
                name: "user_chat_state",
                schema: "public",
                table: "bot_users");
        }
    }
}
