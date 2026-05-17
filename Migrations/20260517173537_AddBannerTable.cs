using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNext.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 17, 17, 35, 29, 719, DateTimeKind.Utc).AddTicks(2681), "$2a$11$hekEhvupVMGa3n/6sGKzJOSdGn6zLXDYr9.JT1pWN/dJW6ijs4kGe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 16, 2, 52, 24, 181, DateTimeKind.Utc).AddTicks(3868), "$2a$11$LKJ9q5pSk46dAn8sGVtKf.WG3wH5GwdKS.flNDM.1JT.ztph94Bsu" });
        }
    }
}
