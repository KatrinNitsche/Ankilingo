using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            // Drop the existing Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Courses");

            // Add the new Id column with the desired type
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Add the primary key constraint back
            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
