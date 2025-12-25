using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClubRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ICO",
                schema: "handball_is",
                table: "Club",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "handball_is",
                table: "Club",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateTable(
                name: "ClubRegistration",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    PackageACount = table.Column<int>(type: "integer", nullable: false),
                    PackageBCount = table.Column<int>(type: "integer", nullable: false),
                    CalculatedFee = table.Column<float>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ApprovedDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    DeniedDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    DenialReason = table.Column<string>(type: "text", nullable: true),
                    ClubId = table.Column<int>(type: "integer", nullable: false),
                    TournamentInstanceId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubRegistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubRegistration_Club_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "handball_is",
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ClubRegistration_TournamentInstance_TournamentInstanceId",
                        column: x => x.TournamentInstanceId,
                        principalSchema: "handball_is",
                        principalTable: "TournamentInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ClubRegistrationCategory",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    TeamCount = table.Column<int>(type: "integer", nullable: false),
                    ClubRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubRegistrationCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubRegistrationCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "handball_is",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ClubRegistrationCategory_ClubRegistration_ClubRegistrationId",
                        column: x => x.ClubRegistrationId,
                        principalSchema: "handball_is",
                        principalTable: "ClubRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ClubRegistration_ClubId",
                schema: "handball_is",
                table: "ClubRegistration",
                column: "ClubId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ClubRegistration_TournamentInstanceId",
                schema: "handball_is",
                table: "ClubRegistration",
                column: "TournamentInstanceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ClubRegistrationCategory_CategoryId",
                schema: "handball_is",
                table: "ClubRegistrationCategory",
                column: "CategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ClubRegistrationCategory_ClubRegistrationId",
                schema: "handball_is",
                table: "ClubRegistrationCategory",
                column: "ClubRegistrationId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ClubRegistrationCategory", schema: "handball_is");

            migrationBuilder.DropTable(name: "ClubRegistration", schema: "handball_is");

            migrationBuilder.DropColumn(name: "ICO", schema: "handball_is", table: "Club");

            migrationBuilder.DropColumn(name: "Status", schema: "handball_is", table: "Club");
        }
    }
}
