using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCodAvailableToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCodAvailable",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 10, 36, 15, 485, DateTimeKind.Utc).AddTicks(5500), "$2a$11$1fg89rvWYiEeI5j88oYJXuq0L4E4cylO2yuQY8YQ.BRdKfn8Q/eD6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCodAvailable",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 19, 11, 27, 13, 24, DateTimeKind.Utc).AddTicks(596), "$2a$11$eGlFwQ6a/asPAyPQgih8vOgtbgmOaFpqI7NJHEQFXlcXaDqv3Fw4." });
        }
    }
}
