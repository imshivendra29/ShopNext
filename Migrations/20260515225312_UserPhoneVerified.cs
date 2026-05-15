using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class UserPhoneVerified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "IsPhoneVerified", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 15, 22, 53, 5, 712, DateTimeKind.Utc).AddTicks(4587), false, "$2a$11$kmziYvGaND4W0LQiXWwtkuhKXASL83hDasE6kQAffKBJ2LkEfdVnC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhoneVerified",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 14, 9, 19, 49, 81, DateTimeKind.Utc).AddTicks(4693), "$2a$11$.7HSpxa2m3cD/T8rY.TkL.5CbUXKVPWy/J1h8.D1Y2dxp354Y2wIK" });
        }
    }
}
