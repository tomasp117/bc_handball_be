using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities.Actors.sub
{
    public class Referee : BasePersonRole
    {
        public char License { get; set; }

        // Foreign keys
        public ICollection<Match> MainRefereeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AssistantRefereeMatches { get; set; } = new List<Match>();
    }
}
