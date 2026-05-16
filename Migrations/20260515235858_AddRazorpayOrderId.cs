using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class AddRazorpayOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RazorpayOrderId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 15, 23, 58, 56, 497, DateTimeKind.Utc).AddTicks(8998), "$2a$11$Ogn2SNZn54lNOVAjmu3Vjug.pb2KkgqHGp1Z/FY.9e0i9JduI9R5i" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RazorpayOrderId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 15, 22, 53, 5, 712, DateTimeKind.Utc).AddTicks(4587), "$2a$11$kmziYvGaND4W0LQiXWwtkuhKXASL83hDasE6kQAffKBJ2LkEfdVnC" });
        }
    }
}
