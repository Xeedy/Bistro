using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BistroStarsHollow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBeerPriceSmall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceSmall",
                table: "Beers",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceSmall",
                table: "Beers");
        }
    }
}
