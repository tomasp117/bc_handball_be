using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class finalGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinalGroup",
                table: "Group",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalGroup",
                table: "Group");
        }
    }
}
