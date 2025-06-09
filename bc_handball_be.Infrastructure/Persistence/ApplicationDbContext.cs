using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using Microsoft.EntityFrameworkCore;

namespace bc_handball_be.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentInstance> TournamentInstances { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Recorder> Recorders { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Referee> Referees { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<ClubAdmin> ClubAdmins { get; set; }
        public DbSet<TeamGroup> TeamGroups { get; set; }
        public DbSet<Lineup> Lineups { get; set; }
        public DbSet<LineupPlayer> LineupPlayers { get; set; }


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tournament>().ToTable("Tournament");
            modelBuilder.Entity<TournamentInstance>().ToTable("TournamentInstance");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<Team>().ToTable("Team");
            modelBuilder.Entity<TeamGroup>().ToTable("TeamGroup");
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Match>().ToTable("Match");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Club>().ToTable("Club");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Coach>().ToTable("Coach");
            modelBuilder.Entity<Recorder>().ToTable("Recorder");
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Referee>().ToTable("Referee");
            modelBuilder.Entity<Login>().ToTable("Login");
            modelBuilder.Entity<ClubAdmin>().ToTable("ClubAdmin");
            modelBuilder.Entity<Lineup>().ToTable("Lineup");
            modelBuilder.Entity<LineupPlayer>().ToTable("LineupPlayer");

            // 1:N Tournament - TournamentInstance
            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.Editions)
                .WithOne(ti => ti.Tournament)
                .HasForeignKey(ti => ti.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N TournamentInstance - Category
            modelBuilder.Entity<TournamentInstance>()
                .HasMany(ti => ti.Categories)
                .WithOne(c => c.TournamentInstance)
                .HasForeignKey(c => c.TournamentInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Category - Group
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Groups)
                .WithOne(g => g.Category)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Group - Match
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Matches)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Group - Team (Tým může patřit do jedné skupiny)
            /*modelBuilder.Entity<Team>()
                .HasOne(t => t.Group)
                .WithMany(g => g.Teams)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);*/

            // M:N Team - Group přes TeamGroup
            modelBuilder.Entity<TeamGroup>()
                .HasKey(tg => new { tg.TeamId, tg.GroupId });

            modelBuilder.Entity<TeamGroup>()
                .HasOne(tg => tg.Team)
                .WithMany(t => t.TeamGroups)
                .HasForeignKey(tg => tg.TeamId);

            modelBuilder.Entity<TeamGroup>()
                .HasOne(tg => tg.Group)
                .WithMany(g => g.TeamGroups)
                .HasForeignKey(tg => tg.GroupId);


            // 1:N Team - Player
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1:1 Team - Coach
            modelBuilder.Entity<Coach>()
                .HasOne(c => c.Team)
                .WithOne(t => t.Coach)
                .HasForeignKey<Coach>(c => c.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Match - Event
            modelBuilder.Entity<Match>()
                .HasMany(m => m.Events)
                .WithOne(e => e.Match)
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Club - Team
            modelBuilder.Entity<Club>()
                .HasMany(c => c.Teams)
                .WithOne(t => t.Club)
                .HasForeignKey(t => t.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Category - Player
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Stats)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //// 1:N Category - Coach
            //modelBuilder.Entity<Category>()
            //    .HasMany(c => c.Voting)
            //    .WithOne(co => co.Category)
            //    .HasForeignKey(co => co.CategoryId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // 1:1 HomeTeam and AwayTeam - Match
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1:1 Person - Login
            modelBuilder.Entity<Person>()
                .HasOne(p => p.Login)
                .WithOne(l => l.Person)
                .HasForeignKey<Login>(l => l.PersonId)
                .OnDelete(DeleteBehavior.Cascade);


            // Roles
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.Person)
                .WithOne()
                .HasForeignKey<Admin>(a => a.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Coach>()
                .HasOne(c => c.Person)
                .WithMany()
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recorder>()
                .HasOne(r => r.Person)
                .WithOne()
                .HasForeignKey<Recorder>(r => r.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Referee>()
                 .HasOne(r => r.Person)
                 .WithOne()
                 .HasForeignKey<Referee>(r => r.PersonId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubAdmin>()
                .HasOne(ca => ca.Person)
                .WithOne()
                .HasForeignKey<ClubAdmin>(ca => ca.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Person)
                .WithMany()
                .HasForeignKey(p => p.PersonId)
                .OnDelete(DeleteBehavior.Cascade);


            // 1:1 Club - ClubAdmin
            modelBuilder.Entity<ClubAdmin>()
                .HasOne(ca => ca.Club)
                .WithOne(c => c.ClubAdmin)
                .HasForeignKey<ClubAdmin>(ca => ca.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 MainReferee and AssistantReferee in Match
            modelBuilder.Entity<Match>()
                .HasOne(m => m.MainReferee)
                .WithMany(r => r.MainRefereeMatches)
                .HasForeignKey(m => m.MainRefereeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AssistantReferee)
                .WithMany(r => r.AssistantRefereeMatches)
                .HasForeignKey(m => m.AssistantRefereeId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1:N Match - Lineup
            modelBuilder.Entity<Match>()
                .HasMany(m => m.Lineups)
                .WithOne(l => l.Match)
                .HasForeignKey(l => l.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Lineup - LineupPlayer
            modelBuilder.Entity<Lineup>()
                .HasMany(l => l.Players)
                .WithOne(lp => lp.Lineup)
                .HasForeignKey(lp => lp.LineupId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Team - Lineup
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Lineups)
                .WithOne(l => l.Team)
                .HasForeignKey(l => l.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N Player - LineupPlayer
            modelBuilder.Entity<Player>()
                .HasMany(p => p.LineupPlayers)
                .WithOne(lp => lp.Player)
                .HasForeignKey(lp => lp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);


            // TPT inheritance
            //modelBuilder.Entity<Person>().ToTable("Person");
            //modelBuilder.Entity<Admin>().ToTable("Admin");
            //modelBuilder.Entity<Coach>().ToTable("Coach");
            //modelBuilder.Entity<Recorder>().ToTable("Recorder");
            //modelBuilder.Entity<Referee>().ToTable("Referee");

            // Enum conversions
            //modelBuilder.Entity<Person>().Property(p => p.Role).HasConversion<string>();
            modelBuilder.Entity<Match>().Property(m => m.State).HasConversion<string>();

            // Ignore navigation properties in database
            //modelBuilder.Entity<Team>().Ignore(t => t.Group);
            //modelBuilder.Entity<Group>().Ignore(g => g.Teams);
            modelBuilder.Entity<Match>().Ignore(m => m.Category);
            modelBuilder.Entity<Category>().Ignore(c => c.Matches);

        }

    }
}
