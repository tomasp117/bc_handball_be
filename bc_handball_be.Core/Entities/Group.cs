

using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class Group : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();

        // Foreign keys
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
