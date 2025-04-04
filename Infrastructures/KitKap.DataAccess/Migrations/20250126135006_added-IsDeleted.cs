using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitKap.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "IsDeleted" },
                values: new object[] { new DateTime(2025, 1, 26, 16, 50, 4, 4, DateTimeKind.Local).AddTicks(4983), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsDeleted" },
                values: new object[] { new DateTime(2025, 1, 26, 16, 50, 4, 4, DateTimeKind.Local).AddTicks(4999), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "IsDeleted" },
                values: new object[] { new DateTime(2025, 1, 26, 16, 50, 4, 4, DateTimeKind.Local).AddTicks(5000), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Addresses");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 23, 20, 40, 36, 519, DateTimeKind.Local).AddTicks(7835));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 23, 20, 40, 36, 519, DateTimeKind.Local).AddTicks(7847));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 23, 20, 40, 36, 519, DateTimeKind.Local).AddTicks(7848));
        }
    }
}
