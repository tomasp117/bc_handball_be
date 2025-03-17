using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;


namespace bc_handball_be.Core.Entities
{
    public class Category : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool VoitingOpen { get; set; }

        // Navigation properties
        public List<Match>? Matches { get; set; } = new List<Match>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Coach> Voting { get; set; } = new List<Coach>();
        public List<Player> Stats { get; set; } = new List<Player>();

        // Foreign keys
        public int TournamentInstanceId { get; set; }
        public TournamentInstance TournamentInstance { get; set; } = null!;


    }
}
