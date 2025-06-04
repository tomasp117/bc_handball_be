
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.Xml;

namespace bc_handball_be.Core.Entities
{
    public enum MatchState
    {
        None,      // msNone - Výchozí stav 
        Pending,   // msPending - Probíhá
        Done,      // msDone - Zápas byl odehrán
        Generated  // msGenerated - Vytvořený zápas bez známých týmů (např. pro playoff)
    }

    public class Match : BaseEntity
    {
        // public int Id { get; set; }
        public DateTime Time { get; set; }
        public string TimePlayed { get; set; } = string.Empty;
        public string Playground { get; set; } = string.Empty;
        //public string Score { get; set; } = "0:0";
        public int? HomeScore { get; set; } = 0;
        public int? AwayScore { get; set; } = 0;
        public MatchState State { get; set; } = MatchState.None;

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public Category? Category { get; set; } = null!;

        public ICollection<Lineup> Lineups { get; set; } = new List<Lineup>();

        // Foreign keys
        // Teams
        public int? HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; } = null!;

        public int? AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; } = null!;

        // Referees
        public int? MainRefereeId { get; set; }
        public Referee? MainReferee { get; set; } = null!;

        public int? AssistantRefereeId { get; set; }
        public Referee? AssistantReferee { get; set; } = null!;

        // Group
        public int? GroupId { get; set; }
        public Group? Group { get; set; } = null!;

    }
}
