using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntryData",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LevelOfKnowledge = table.Column<int>(type: "int", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryData", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.id);
                    table.ForeignKey(
                        name: "FK_Images_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageWord",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    Valueid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserInput = table.Column<int>(type: "int", nullable: false),
                    WasChecked = table.Column<bool>(type: "bit", nullable: false),
                    ImageDataid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageWord", x => x.id);
                    table.ForeignKey(
                        name: "FK_ImageWord_EntryData_Valueid",
                        column: x => x.Valueid,
                        principalTable: "EntryData",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageWord_Images_ImageDataid",
                        column: x => x.ImageDataid,
                        principalTable: "Images",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_CourseId",
                table: "Images",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageWord_ImageDataid",
                table: "ImageWord",
                column: "ImageDataid");

            migrationBuilder.CreateIndex(
                name: "IX_ImageWord_Valueid",
                table: "ImageWord",
                column: "Valueid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageWord");

            migrationBuilder.DropTable(
                name: "EntryData");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
