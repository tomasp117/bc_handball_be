

using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class Tournament : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<TournamentInstance> Editions { get; set; } = new List<TournamentInstance>();
    }
}
