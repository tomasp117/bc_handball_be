using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class TournamentInstance : BaseEntity
    {
        // public int Id { get; set; }
        public int EditionNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();


        // Foreign keys
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
    }
}
