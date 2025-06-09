using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coach_Category_CategoryId",
                table: "Coach");

            migrationBuilder.DropIndex(
                name: "IX_Coach_CategoryId",
                table: "Coach");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Coach");

            migrationBuilder.DropColumn(
                name: "GoalkeeperVoteId",
                table: "Coach");

            migrationBuilder.DropColumn(
                name: "PlayerVoteId",
                table: "Coach");

            migrationBuilder.DropColumn(
                name: "VoitingOpen",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Event_AuthorId",
                table: "Event",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Player_AuthorId",
                table: "Event",
                column: "AuthorId",
                principalTable: "Player",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Player_AuthorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_AuthorId",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Coach",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalkeeperVoteId",
                table: "Coach",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerVoteId",
                table: "Coach",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VoitingOpen",
                table: "Category",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Coach_CategoryId",
                table: "Coach",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coach_Category_CategoryId",
                table: "Coach",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
