using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thatnetwork.Migrations
{
    /// <inheritdoc />
    public partial class clubwereupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Notes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                table: "Clubs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ClubId",
                table: "Notes",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_AvatarId",
                table: "Clubs",
                column: "AvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_Photos_AvatarId",
                table: "Clubs",
                column: "AvatarId",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Clubs_ClubId",
                table: "Notes",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Photos_AvatarId",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Clubs_ClubId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_ClubId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_AvatarId",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Clubs");
        }
    }
}
