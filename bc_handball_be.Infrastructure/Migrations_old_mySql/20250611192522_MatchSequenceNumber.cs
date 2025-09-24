using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchSequenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Match",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Match");
        }
    }
}
