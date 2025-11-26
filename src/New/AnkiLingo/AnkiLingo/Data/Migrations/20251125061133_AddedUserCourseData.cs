using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserCourseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCourseData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    LevelOfKnowledge = table.Column<int>(type: "int", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourseData");
        }
    }
}
