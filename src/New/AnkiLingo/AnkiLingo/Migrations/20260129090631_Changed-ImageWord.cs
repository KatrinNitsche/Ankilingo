using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class ChangedImageWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageWords_Courses_CourseId",
                table: "ImageWords");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageWords_EntryData_Valueid",
                table: "ImageWords");

            migrationBuilder.DropTable(
                name: "EntryData");

            migrationBuilder.DropIndex(
                name: "IX_ImageWords_CourseId",
                table: "ImageWords");

            migrationBuilder.DropIndex(
                name: "IX_ImageWords_Valueid",
                table: "ImageWords");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ImageWords");

            migrationBuilder.DropColumn(
                name: "UserInput",
                table: "ImageWords");

            migrationBuilder.DropColumn(
                name: "Valueid",
                table: "ImageWords");

            migrationBuilder.DropColumn(
                name: "WasChecked",
                table: "ImageWords");

            migrationBuilder.AddColumn<string>(
                name: "EntryText",
                table: "ImageWords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryText",
                table: "ImageWords");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "ImageWords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UserInput",
                table: "ImageWords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Valueid",
                table: "ImageWords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "WasChecked",
                table: "ImageWords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EntryData",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LevelOfKnowledge = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryData", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageWords_CourseId",
                table: "ImageWords",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageWords_Valueid",
                table: "ImageWords",
                column: "Valueid");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageWords_Courses_CourseId",
                table: "ImageWords",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageWords_EntryData_Valueid",
                table: "ImageWords",
                column: "Valueid",
                principalTable: "EntryData",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
