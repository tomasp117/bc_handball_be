using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupToTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
