using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitKap.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UserCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9733));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9744));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9745));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 6, 15, 22, 14, 180, DateTimeKind.Local).AddTicks(2620));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 6, 15, 22, 14, 180, DateTimeKind.Local).AddTicks(2631));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 6, 15, 22, 14, 180, DateTimeKind.Local).AddTicks(2632));
        }
    }
}
