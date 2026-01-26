using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnkiLingo.Migrations
{
    /// <inheritdoc />
    public partial class AddedLinkCourseImageImageWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageWord_EntryData_Valueid",
                table: "ImageWord");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageWord_Images_ImageDataid",
                table: "ImageWord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageWord",
                table: "ImageWord");

            migrationBuilder.RenameTable(
                name: "ImageWord",
                newName: "ImageWords");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Images",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ImageDataid",
                table: "ImageWords",
                newName: "ImageDataId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ImageWords",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ImageWord_Valueid",
                table: "ImageWords",
                newName: "IX_ImageWords_Valueid");

            migrationBuilder.RenameIndex(
                name: "IX_ImageWord_ImageDataid",
                table: "ImageWords",
                newName: "IX_ImageWords_ImageDataId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "ImageWords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageWords",
                table: "ImageWords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImageWords_CourseId",
                table: "ImageWords",
                column: "CourseId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ImageWords_Images_ImageDataId",
                table: "ImageWords",
                column: "ImageDataId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageWords_Courses_CourseId",
                table: "ImageWords");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageWords_EntryData_Valueid",
                table: "ImageWords");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageWords_Images_ImageDataId",
                table: "ImageWords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageWords",
                table: "ImageWords");

            migrationBuilder.DropIndex(
                name: "IX_ImageWords_CourseId",
                table: "ImageWords");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ImageWords");

            migrationBuilder.RenameTable(
                name: "ImageWords",
                newName: "ImageWord");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Images",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ImageDataId",
                table: "ImageWord",
                newName: "ImageDataid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ImageWord",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_ImageWords_Valueid",
                table: "ImageWord",
                newName: "IX_ImageWord_Valueid");

            migrationBuilder.RenameIndex(
                name: "IX_ImageWords_ImageDataId",
                table: "ImageWord",
                newName: "IX_ImageWord_ImageDataid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageWord",
                table: "ImageWord",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageWord_EntryData_Valueid",
                table: "ImageWord",
                column: "Valueid",
                principalTable: "EntryData",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageWord_Images_ImageDataid",
                table: "ImageWord",
                column: "ImageDataid",
                principalTable: "Images",
                principalColumn: "id");
        }
    }
}
