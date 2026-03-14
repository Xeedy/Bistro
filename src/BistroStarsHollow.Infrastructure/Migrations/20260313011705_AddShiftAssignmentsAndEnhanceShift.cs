using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BistroStarsHollow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftAssignmentsAndEnhanceShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shifts_Date",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_UserId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shifts");

            migrationBuilder.AddColumn<int>(
                name: "DayStatus",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredStaffCount",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShiftAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Date",
                table: "Shifts",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_ShiftId_UserId",
                table: "ShiftAssignments",
                columns: new[] { "ShiftId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_Date",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DayStatus",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "RequiredStaffCount",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Shifts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Shifts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Date",
                table: "Shifts",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_UserId",
                table: "Shifts",
                column: "UserId");
        }
    }
}
