using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamGroupTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroups_Group_GroupId",
                table: "TeamGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroups_Team_TeamId",
                table: "TeamGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamGroups",
                table: "TeamGroups");

            migrationBuilder.RenameTable(
                name: "TeamGroups",
                newName: "TeamGroup");

            migrationBuilder.RenameIndex(
                name: "IX_TeamGroups_GroupId",
                table: "TeamGroup",
                newName: "IX_TeamGroup_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamGroup",
                table: "TeamGroup",
                columns: new[] { "TeamId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroup_Group_GroupId",
                table: "TeamGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroup_Team_TeamId",
                table: "TeamGroup",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroup_Group_GroupId",
                table: "TeamGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroup_Team_TeamId",
                table: "TeamGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamGroup",
                table: "TeamGroup");

            migrationBuilder.RenameTable(
                name: "TeamGroup",
                newName: "TeamGroups");

            migrationBuilder.RenameIndex(
                name: "IX_TeamGroup_GroupId",
                table: "TeamGroups",
                newName: "IX_TeamGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamGroups",
                table: "TeamGroups",
                columns: new[] { "TeamId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroups_Group_GroupId",
                table: "TeamGroups",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroups_Team_TeamId",
                table: "TeamGroups",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
