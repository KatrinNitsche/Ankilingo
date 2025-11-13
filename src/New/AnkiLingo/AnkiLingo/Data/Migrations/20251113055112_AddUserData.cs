using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class AddUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreakLength = table.Column<int>(type: "int", nullable: false),
                    GemsCount = table.Column<int>(type: "int", nullable: false),
                    CurrentCourse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XPCount = table.Column<int>(type: "int", nullable: false),
                    LastStudy = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
