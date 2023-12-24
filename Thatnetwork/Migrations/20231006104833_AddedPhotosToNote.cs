using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thatnetwork.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotosToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "Photos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_NoteId",
                table: "Photos",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Notes_NoteId",
                table: "Photos",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Notes_NoteId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_NoteId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Photos");
        }
    }
}
