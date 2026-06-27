using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class AddRazorpayPaymentIdToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RazorpayPaymentId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 17, 2, 10, 733, DateTimeKind.Utc).AddTicks(625), "$2a$11$OTUtse9nHAmHy3oLnB1Nn.CYQU8CelGq3nFCmPfuTCdm0KXCdM9pe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RazorpayPaymentId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 10, 36, 15, 485, DateTimeKind.Utc).AddTicks(5500), "$2a$11$1fg89rvWYiEeI5j88oYJXuq0L4E4cylO2yuQY8YQ.BRdKfn8Q/eD6" });
        }
    }
}
