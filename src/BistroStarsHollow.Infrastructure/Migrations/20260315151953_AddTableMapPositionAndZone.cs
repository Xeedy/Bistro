using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BistroStarsHollow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMapPositionAndZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MapX",
                table: "BistroTables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MapY",
                table: "BistroTables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Zone",
                table: "BistroTables",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapX",
                table: "BistroTables");

            migrationBuilder.DropColumn(
                name: "MapY",
                table: "BistroTables");

            migrationBuilder.DropColumn(
                name: "Zone",
                table: "BistroTables");
        }
    }
}
