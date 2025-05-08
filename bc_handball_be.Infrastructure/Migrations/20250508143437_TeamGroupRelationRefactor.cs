using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamGroupRelationRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Group_GroupId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_GroupId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Team");

            migrationBuilder.CreateTable(
                name: "TeamGroups",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGroups", x => new { x.TeamId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_TeamGroups_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamGroups_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_GroupId",
                table: "TeamGroups",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamGroups");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Team",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_GroupId",
                table: "Team",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Group_GroupId",
                table: "Team",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
