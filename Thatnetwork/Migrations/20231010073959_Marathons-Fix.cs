using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thatnetwork.Migrations
{
    /// <inheritdoc />
    public partial class MarathonsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaraphonUser");

            migrationBuilder.DropTable(
                name: "Maraphons");

            migrationBuilder.CreateTable(
                name: "Marathons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Tag = table.Column<string>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marathons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marathons_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MarathonUser",
                columns: table => new
                {
                    ParticipantsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TakenMaraphonesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarathonUser", x => new { x.ParticipantsId, x.TakenMaraphonesId });
                    table.ForeignKey(
                        name: "FK_MarathonUser_Marathons_TakenMaraphonesId",
                        column: x => x.TakenMaraphonesId,
                        principalTable: "Marathons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarathonUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Marathons_CreatorId",
                table: "Marathons",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Marathons_Tag",
                table: "Marathons",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarathonUser_TakenMaraphonesId",
                table: "MarathonUser",
                column: "TakenMaraphonesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarathonUser");

            migrationBuilder.DropTable(
                name: "Marathons");

            migrationBuilder.CreateTable(
                name: "Maraphons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tag = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maraphons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maraphons_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaraphonUser",
                columns: table => new
                {
                    ParticipantsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TakenMaraphonesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaraphonUser", x => new { x.ParticipantsId, x.TakenMaraphonesId });
                    table.ForeignKey(
                        name: "FK_MaraphonUser_Maraphons_TakenMaraphonesId",
                        column: x => x.TakenMaraphonesId,
                        principalTable: "Maraphons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaraphonUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Maraphons_CreatorId",
                table: "Maraphons",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Maraphons_Tag",
                table: "Maraphons",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaraphonUser_TakenMaraphonesId",
                table: "MaraphonUser",
                column: "TakenMaraphonesId");
        }
    }
}
