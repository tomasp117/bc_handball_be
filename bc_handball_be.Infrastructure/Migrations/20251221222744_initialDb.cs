using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace bc_handball_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "handball_is");

            migrationBuilder.CreateTable(
                name: "Club",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    IsPlaceholder = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournament",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admin_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubAdmin",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClubId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubAdmin_Club_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "handball_is",
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubAdmin_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Login",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Login_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recorder",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recorder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recorder_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Referee",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    License = table.Column<char>(type: "character(1)", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referee_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentInstance",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EditionNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TournamentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentInstance_Tournament_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "handball_is",
                        principalTable: "Tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TournamentInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_TournamentInstance_TournamentInstanceId",
                        column: x => x.TournamentInstanceId,
                        principalSchema: "handball_is",
                        principalTable: "TournamentInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phase = table.Column<string>(type: "text", nullable: true),
                    FinalGroup = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "handball_is",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    TournamentInstanceId = table.Column<int>(type: "integer", nullable: false),
                    IsPlaceholder = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "handball_is",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Team_Club_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "handball_is",
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Team_TournamentInstance_TournamentInstanceId",
                        column: x => x.TournamentInstanceId,
                        principalSchema: "handball_is",
                        principalTable: "TournamentInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coach",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    License = table.Column<char>(type: "character(1)", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coach", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coach_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coach_Team_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimePlayed = table.Column<string>(type: "text", nullable: false),
                    Playground = table.Column<string>(type: "text", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    State = table.Column<string>(type: "text", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: true),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: true),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: true),
                    MainRefereeId = table.Column<int>(type: "integer", nullable: true),
                    AssistantRefereeId = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Match_Group_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "handball_is",
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Referee_AssistantRefereeId",
                        column: x => x.AssistantRefereeId,
                        principalSchema: "handball_is",
                        principalTable: "Referee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Match_Referee_MainRefereeId",
                        column: x => x.MainRefereeId,
                        principalSchema: "handball_is",
                        principalTable: "Referee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Match_Team_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Match_Team_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    GoalCount = table.Column<int>(type: "integer", nullable: false),
                    SevenMeterGoalCount = table.Column<int>(type: "integer", nullable: false),
                    SevenMeterMissCount = table.Column<int>(type: "integer", nullable: false),
                    TwoMinPenaltyCount = table.Column<int>(type: "integer", nullable: false),
                    RedCardCount = table.Column<int>(type: "integer", nullable: false),
                    YellowCardCount = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "handball_is",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Player_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "handball_is",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Player_Team_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TeamGroup",
                schema: "handball_is",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGroup", x => new { x.TeamId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_TeamGroup_Group_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "handball_is",
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamGroup_Team_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lineup",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lineup_Match_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "handball_is",
                        principalTable: "Match",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lineup_Team_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "handball_is",
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Team = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Match_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "handball_is",
                        principalTable: "Match",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Player_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "handball_is",
                        principalTable: "Player",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LineupPlayer",
                schema: "handball_is",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LineupId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineupPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineupPlayer_Lineup_LineupId",
                        column: x => x.LineupId,
                        principalSchema: "handball_is",
                        principalTable: "Lineup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineupPlayer_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "handball_is",
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_PersonId",
                schema: "handball_is",
                table: "Admin",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_TournamentInstanceId",
                schema: "handball_is",
                table: "Category",
                column: "TournamentInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubAdmin_ClubId",
                schema: "handball_is",
                table: "ClubAdmin",
                column: "ClubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClubAdmin_PersonId",
                schema: "handball_is",
                table: "ClubAdmin",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coach_PersonId",
                schema: "handball_is",
                table: "Coach",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Coach_TeamId",
                schema: "handball_is",
                table: "Coach",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_AuthorId",
                schema: "handball_is",
                table: "Event",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_MatchId",
                schema: "handball_is",
                table: "Event",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_CategoryId",
                schema: "handball_is",
                table: "Group",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineup_MatchId",
                schema: "handball_is",
                table: "Lineup",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineup_TeamId",
                schema: "handball_is",
                table: "Lineup",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayer_LineupId",
                schema: "handball_is",
                table: "LineupPlayer",
                column: "LineupId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayer_PlayerId",
                schema: "handball_is",
                table: "LineupPlayer",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Login_PersonId",
                schema: "handball_is",
                table: "Login",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_AssistantRefereeId",
                schema: "handball_is",
                table: "Match",
                column: "AssistantRefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwayTeamId",
                schema: "handball_is",
                table: "Match",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_GroupId",
                schema: "handball_is",
                table: "Match",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeTeamId",
                schema: "handball_is",
                table: "Match",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_MainRefereeId",
                schema: "handball_is",
                table: "Match",
                column: "MainRefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_CategoryId",
                schema: "handball_is",
                table: "Player",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_PersonId",
                schema: "handball_is",
                table: "Player",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_TeamId",
                schema: "handball_is",
                table: "Player",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorder_PersonId",
                schema: "handball_is",
                table: "Recorder",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referee_PersonId",
                schema: "handball_is",
                table: "Referee",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_CategoryId",
                schema: "handball_is",
                table: "Team",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ClubId",
                schema: "handball_is",
                table: "Team",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TournamentInstanceId",
                schema: "handball_is",
                table: "Team",
                column: "TournamentInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroup_GroupId",
                schema: "handball_is",
                table: "TeamGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentInstance_TournamentId",
                schema: "handball_is",
                table: "TournamentInstance",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "ClubAdmin",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Coach",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "LineupPlayer",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Login",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Recorder",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "TeamGroup",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Lineup",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Player",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Match",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Group",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Referee",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Team",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Person",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Club",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "TournamentInstance",
                schema: "handball_is");

            migrationBuilder.DropTable(
                name: "Tournament",
                schema: "handball_is");
        }
    }
}
