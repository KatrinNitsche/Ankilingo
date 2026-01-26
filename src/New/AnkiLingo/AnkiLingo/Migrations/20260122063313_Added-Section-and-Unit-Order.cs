using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class AddedSectionandUnitOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Sections");
        }
    }
}
