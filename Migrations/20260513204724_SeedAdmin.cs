using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "Role" },
                values: new object[] { 11, "admin@shopnext.com", "Admin", "$2a$11$SqQcQVuv/pJ/wwUAqP7fYuCpIAhlBIO1dGswD4BejbJArimjHNqgy", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
