using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitKap.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialAppUserIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActived",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActived",
                table: "AspNetUsers");
        }
    }
}
