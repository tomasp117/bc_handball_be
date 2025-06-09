

using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    //public enum EventType
    //{
    //    Goal,
    //    Foul,
    //    Timeout,
    //    YellowCard,
    //    RedCard
    //}

    public class Event : BaseEntity
    {
        // public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Team { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;


        // Foreign keys
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
       
        public int? AuthorId { get; set; }
        public Player? Author { get; set; } = null!;

    }
}
