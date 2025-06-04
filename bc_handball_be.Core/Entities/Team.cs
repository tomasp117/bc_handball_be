using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class Team : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Player> Players { get; set; } = new List<Player>();

        public Coach? Coach { get; set; }

        // Foreign keys
        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int TournamentInstanceId { get; set; }
        public TournamentInstance TournamentInstance { get; set; } = null!;

        public bool? IsPlaceholder { get; set; } = false;

        //public int? GroupId { get; set; }
        //public Group? Group { get; set; } = null!;

        public ICollection<TeamGroup> TeamGroups { get; set; } = new List<TeamGroup>();

        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();

        public ICollection<Lineup> Lineups { get; set; } = new List<Lineup>();
    }
}
