

using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public enum EventType
    {
        Goal,
        Foul,
        Timeout,
        YellowCard,
        RedCard
    }

    public class Event : BaseEntity
    {
        // public int Id { get; set; }
        public EventType Type { get; set; }
        public string Team { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public int AuthorId { get; set; }

        // Foreign keys
        public int MatchId { get; set; }
        public Match Match { get; set; } = new Match();

    }
}
