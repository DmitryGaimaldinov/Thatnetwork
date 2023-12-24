using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thatnetwork.Migrations
{
    /// <inheritdoc />
    public partial class ChallengeAvatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                table: "Marathons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marathons_AvatarId",
                table: "Marathons",
                column: "AvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marathons_Photos_AvatarId",
                table: "Marathons",
                column: "AvatarId",
                principalTable: "Photos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marathons_Photos_AvatarId",
                table: "Marathons");

            migrationBuilder.DropIndex(
                name: "IX_Marathons_AvatarId",
                table: "Marathons");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Marathons");
        }
    }
}
